using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// গ্রামার কুইজ: বেগুনি ৩D গ্লাস স্টাইল। MainMenuDesign এর Quiz বাটন চাপলে Open() চলে।
// তিনটা মোড: Role Spotter (Subject/Verb/Object ট্যাপ), Fill the Blank (a/an আর ক্রিয়ার রূপ), Word Type (noun, verb...)।
// প্রশ্ন আসে খেলোয়াড়ের শেখা শব্দের Grammar Lesson থেকে (কম শিখলে প্রথম ১০ শব্দ)। ভুল উত্তরে ব্যাখ্যা সেটিংসের ভাষায় আসে।
// লাগে: Neo, UIKit, UIShapes, GrammarData, GrammarLang, MainMenuDesign (PressScale), PlayerData, AudioManager।
public class QuizPanel : MonoBehaviour
{
    class Question
    {
        public int level;
        public string prompt, sentence, bigWord, hint, explainLine, explainBody;
        public string[] opts;
        public int answer;
        public bool grid;
        public int cardH;
    }

    const int Total = 10;
    const int MaxHearts = 3;
    const float TimePer = 20f;

    static readonly string[][] Tiers = new string[][]
    {
        new string[] { "GO", "BE", "TO", "NO", "SO", "ME", "WE", "HE", "IT", "UP" },
        new string[] { "MAN", "CUP", "CAT", "DOG", "SUN", "PEN", "BAG", "HAT", "BUS", "EGG" },
        new string[] { "BOOK", "FISH", "TREE", "BALL", "MILK", "DOOR", "LAMP", "RAIN", "STAR", "CAKE" },
        new string[] { "APPLE", "HOUSE", "WATER", "MOUSE", "CHAIR", "TABLE", "MANGO", "BREAD", "CLOCK", "PLANT" },
        new string[] { "BOTTLE", "GARDEN", "ORANGE", "BANANA", "PLANET", "MONKEY", "FLOWER", "POCKET", "MARKET", "SCHOOL" },
        new string[] { "KITCHEN", "TEACHER", "LIBRARY", "BALLOON", "PICTURE", "MORNING", "HOLIDAY", "CHICKEN", "BLANKET", "CAPTAIN" },
        new string[] { "COMPUTER", "ELEPHANT", "HOSPITAL", "KEYBOARD", "NOTEBOOK", "BUILDING", "LANGUAGE", "MOUNTAIN", "SANDWICH", "UMBRELLA" },
        new string[] { "BEAUTIFUL", "BUTTERFLY", "IMPORTANT", "KNOWLEDGE", "ADVENTURE", "CHOCOLATE", "DANGEROUS", "EDUCATION", "FURNITURE", "PINEAPPLE" },
        new string[] { "TELEVISION", "PLAYGROUND", "BASKETBALL", "UNDERSTAND", "TOOTHBRUSH", "DICTIONARY", "FRIENDSHIP", "EXPERIENCE", "STRAWBERRY", "BACKGROUND" },
        new string[] { "WATERMELON", "RESTAURANT", "LIGHTHOUSE", "BLACKBOARD", "GOVERNMENT", "CROCODILES", "PASSENGERS", "NEWSPAPERS", "CHALLENGES", "DISCOVERED" },
    };

    // ব্যাখ্যার ছোট টিপ: [ধরন][ভাষা]। {x} এর জায়গায় শব্দ বসে।
    static readonly string[][] Tips =
    {
        new string[]
        {
            "With he, she, it or one thing, the verb takes -s: {x}.",
            "he, she, it বা একজন/একটার সাথে ক্রিয়ায় s বসে: {x}।",
            "he, she, it या एक के साथ क्रिया में s लगता है: {x}।",
            "مع he وshe وit أو المفرد نضيف s إلى الفعل: {x}."
        },
        new string[]
        {
            "With I, we, they or many, the verb stays in its base form: {x}.",
            "I, we, they বা একাধিকের সাথে ক্রিয়া মূল রূপেই থাকে: {x}।",
            "I, we, they या एक से अधिक के साथ क्रिया मूल रूप में रहती है: {x}।",
            "مع I وwe وthey أو الجمع يبقى الفعل في صورته الأساسية: {x}."
        },
        new string[]
        {
            "Use 'an' before a vowel sound and 'a' before a consonant sound: {x}.",
            "স্বরধ্বনির আগে an, ব্যঞ্জনধ্বনির আগে a বসে: {x}।",
            "स्वर की ध्वनि से पहले an, व्यंजन की ध्वनि से पहले a लगता है: {x}।",
            "نستخدم an قبل الصوت المتحرك وa قبل الصوت الساكن: {x}."
        },
    };

    static string Tip(int key, int lang, string x)
    {
        return Tips[key][Mathf.Clamp(lang, 0, 3)].Replace("{x}", x);
    }

    // ---------- অবস্থা ----------
    GameObject root;
    RectTransform window;
    CanvasGroup group;
    RectTransform scr;
    float openTime;

    int screenKind;        // 0 হাব, 1 প্রশ্ন, 2 ফলাফল
    int mode;
    int qIndex;
    int correct;
    int hearts;
    float timeLeft;
    bool locked = true;
    Question q;
    readonly List<int> used = new List<int>();

    Image timerFill;
    TMP_Text timerLabel;
    Image[] heartImgs;
    Image[][] skins;

    RectTransform[] decoRt;
    float[] decoSpeed;
    float[] decoBaseX;
    float[] decoPhase;

    // ====================================================== ছোট সাহায্যকারী
    static Color C(int r, int g, int b, float a = 1f)
    {
        return Neo.C(r, g, b, a);
    }

    static Neo.Style St(float radius, Color top, Color bottom, Color edgeTop, Color edgeBottom, float edgeW,
                        float gloss, float shadowA, float shadowBlur, float shadowDy, float scale)
    {
        Neo.Style s = new Neo.Style();
        s.radius = radius;
        s.top = top;
        s.bottom = bottom;
        s.edgeTop = edgeTop;
        s.edgeBottom = edgeBottom;
        s.edgeW = edgeW;
        s.gloss = gloss;
        s.shadowA = shadowA;
        s.shadowBlur = shadowBlur;
        s.shadowDy = shadowDy;
        s.scale = scale;
        s.inner = false;
        return s;
    }

    static RectTransform Node(Transform parent, string name, float cx, float cy, float w, float h)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(w, h);
        rt.anchoredPosition = new Vector2(cx, -cy);
        return rt;
    }

    static RectTransform Btn(Transform parent, string name, float cx, float cy, float w, float h, UnityAction act)
    {
        RectTransform n = Node(parent, name, cx, cy, w, h);
        UIShapes.MakeButton(n, act);
        n.gameObject.AddComponent<MainMenuDesign.PressScale>();
        return n;
    }

    static void Chip(Transform p, string text, float x, float y, float w, float h,
                     Color top, Color bottom, Color textColor, float size)
    {
        Neo.Surface(p, "Chip", x, y, w, h,
            St(h * 0.5f, top, bottom, C(255, 255, 255, 0.7f), C(255, 255, 255, 0.15f), 2.5f, 0.5f, 0.45f, 14f, 6f, 1f));
        UIKit.Label(p, text, x, y, w, h, size, textColor, TextAlignmentOptions.Center);
    }

    static TMP_Text Fit(Transform p, string text, float x, float y, float w, float h, float size, Color col,
                        TextAlignmentOptions align, bool outline)
    {
        TMP_Text t = UIKit.Label(p, text, x, y, w, h, size, col, align, outline);
        t.enableAutoSizing = true;
        t.fontSizeMin = 18f;
        t.fontSizeMax = size;
        return t;
    }

    static void SetBar(Image fill, float maxW, float h, float frac)
    {
        float w = frac <= 0f ? 0f : Mathf.Max(h, maxW * frac);
        fill.rectTransform.sizeDelta = new Vector2(w, h);
        fill.gameObject.SetActive(w >= 1f);
    }

    static float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        float u = t - 1f;
        return 1f + c3 * u * u * u + c1 * u * u;
    }

    static string WordAt(int level)
    {
        return Tiers[level / 10][level % 10];
    }

    static int Pool
    {
        get { return Mathf.Clamp(Mathf.Max(10, PlayerData.CompletedLevels), 10, 100); }
    }

    // ---------- হার্টের ছবি (কোড দিয়ে আঁকা) ----------
    static Sprite heartFull;
    static Sprite heartEmpty;

    static Sprite Heart(bool full)
    {
        if (full)
        {
            if (heartFull == null) heartFull = MakeHeart(true);
            return heartFull;
        }
        if (heartEmpty == null) heartEmpty = MakeHeart(false);
        return heartEmpty;
    }

    static bool InHeart(float x, float y)
    {
        float a = x * x + y * y - 1f;
        return a * a * a - x * x * y * y * y <= 0f;
    }

    static Sprite MakeHeart(bool full)
    {
        const int N = 96;
        Color32[] px = new Color32[N * N];
        Color topC = full ? new Color(1f, 0.50f, 0.58f) : new Color(0.55f, 0.50f, 0.70f);
        Color botC = full ? new Color(0.84f, 0.10f, 0.25f) : new Color(0.28f, 0.22f, 0.45f);
        for (int j = 0; j < N; j++)
        {
            for (int i = 0; i < N; i++)
            {
                float cov = 0f;
                float covIn = 0f;
                for (int sy = 0; sy < 3; sy++)
                {
                    for (int sx = 0; sx < 3; sx++)
                    {
                        float u = ((i + (sx + 0.5f) / 3f) / N - 0.5f) * 2.8f;
                        float v = ((j + (sy + 0.5f) / 3f) / N - 0.5f) * 2.8f + 0.1f;
                        if (InHeart(u, v)) cov += 1f;
                        if (InHeart(u * 1.16f, (v - 0.1f) * 1.16f + 0.1f)) covIn += 1f;
                    }
                }
                cov /= 9f;
                covIn /= 9f;
                if (cov <= 0f)
                {
                    px[j * N + i] = new Color32(0, 0, 0, 0);
                    continue;
                }
                float u0 = ((i + 0.5f) / N - 0.5f) * 2.8f;
                float v0 = ((j + 0.5f) / N - 0.5f) * 2.8f + 0.1f;
                Color c = Color.Lerp(botC, topC, j / (float)(N - 1));
                float dx = u0 + 0.45f;
                float dy = v0 - 0.62f;
                c = Color.Lerp(c, Color.white, Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy) / 0.32f) * 0.6f);
                float k = 1f - 0.35f * Mathf.Clamp01(cov - covIn);
                px[j * N + i] = new Color32((byte)(Mathf.Clamp01(c.r * k) * 255f), (byte)(Mathf.Clamp01(c.g * k) * 255f),
                                            (byte)(Mathf.Clamp01(c.b * k) * 255f), (byte)(cov * 255f));
            }
        }
        Texture2D tex = new Texture2D(N, N, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.SetPixels32(px);
        tex.Apply(false, true);
        return Sprite.Create(tex, new Rect(0, 0, N, N), new Vector2(0.5f, 0.5f), 100f);
    }

    // ====================================================== খোলা আর বন্ধ
    public void Open()
    {
        if (root != null) Destroy(root);
        GrammarLang.EnsureFont(GrammarLang.Current);
        Build();
        root.SetActive(true);
        root.transform.SetAsLastSibling();
        openTime = Time.unscaledTime;
        group.alpha = 0f;
        ShowHub();
    }

    public void Close()
    {
        if (root != null) root.SetActive(false);
    }

    void OnX()
    {
        AudioManager.PlayClick();
        if (screenKind == 0) Close();
        else ShowHub();
    }

    // ====================================================== ফ্রেম (বেগুনি ৩D গ্লাস)
    void Build()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();

        RectTransform rootRt = UIShapes.Stretch(canvas.rootCanvas.transform, "QuizRoot");
        root = rootRt.gameObject;
        Image back = root.AddComponent<Image>();
        back.sprite = Neo.VerticalGradient(C(52, 18, 112, 0.97f), C(14, 6, 52, 0.98f));
        back.raycastTarget = true;
        group = root.AddComponent<CanvasGroup>();

        Neo.Glow(rootRt, "GlowA", -300, 120, 900, C(204, 96, 255), 0.55f);
        Neo.Glow(rootRt, "GlowB", 480, 1050, 900, C(110, 96, 255), 0.45f);
        BuildDeco(rootRt);

        GameObject wgo = new GameObject("Window", typeof(RectTransform));
        window = wgo.GetComponent<RectTransform>();
        window.SetParent(rootRt, false);
        window.anchorMin = new Vector2(0.5f, 0.5f);
        window.anchorMax = new Vector2(0.5f, 0.5f);
        window.pivot = new Vector2(0.5f, 0.5f);
        window.sizeDelta = new Vector2(900f, 1580f);
        window.anchoredPosition = Vector2.zero;

        Neo.Surface(window, "Glass", 0, 0, 900, 1580,
            St(56, C(214, 160, 255, 0.22f), C(140, 80, 236, 0.09f), C(255, 255, 255, 0.45f), C(255, 255, 255, 0.10f),
               2.5f, 0f, 0.55f, 70f, 30f, 0.5f));

        // ওপরের গোল চাকতি আর নাম
        Neo.Glow(window, "EmblemGlow", 270, -150, 360, C(214, 130, 255), 0.75f);
        Neo.Surface(window, "Emblem", 375, -75, 150, 150,
            St(75, C(226, 170, 255), C(124, 56, 224), C(255, 255, 255, 0.85f), C(200, 140, 255, 0.6f), 4f, 0.6f, 0.5f, 24f, 12f, 1f));
        UIKit.Label(window, "?", 375, -75, 150, 150, 110, Color.white, TextAlignmentOptions.Center, true);
        TMP_Text title = UIKit.Label(window, "QUIZ", 0, 100, 900, 80, 62, Color.white, TextAlignmentOptions.Center);
        title.characterSpacing = 11f;

        // বন্ধ করার বোতাম
        Neo.Surface(window, "CloseBtn", 776, 36, 84, 84,
            St(42, C(255, 255, 255, 0.26f), C(255, 255, 255, 0.08f), C(255, 255, 255, 0.5f), C(255, 255, 255, 0.12f),
               2f, 0f, 0.35f, 16f, 8f, 1f));
        Neo.Icon(window, 795, 55, 46, "x", C(255, 255, 255), C(215, 205, 255));
        UIKit.Hit(window, 776, 36, 84, 84, OnX);

        root.SetActive(false);
    }

    // পেছনে ভাসমান বেগুনি বৃত্ত আর প্রশ্নচিহ্ন
    void BuildDeco(RectTransform parent)
    {
        System.Random r = new System.Random(11);
        int n = 14;
        decoRt = new RectTransform[n];
        decoSpeed = new float[n];
        decoBaseX = new float[n];
        decoPhase = new float[n];
        for (int i = 0; i < n; i++)
        {
            float x = (float)r.NextDouble() * 1000f;
            float y = (float)r.NextDouble() * 1900f;
            if (i < 9)
            {
                float d = 40f + (float)r.NextDouble() * 150f;
                Color col = (i % 2 == 0) ? C(204, 120, 255, 0.12f) : C(130, 120, 255, 0.12f);
                Image im = UIKit.Round(parent, "Orb", x, y, d, d, d * 0.5f, col);
                decoRt[i] = im.rectTransform;
            }
            else
            {
                float size = 150f + (float)r.NextDouble() * 100f;
                TMP_Text t = UIKit.Label(parent, "?", x, y, size, size * 1.2f, size, C(214, 150, 255, 0.08f), TextAlignmentOptions.Center);
                decoRt[i] = (RectTransform)t.rectTransform.parent;
            }
            decoBaseX[i] = x;
            decoSpeed[i] = 10f + (float)r.NextDouble() * 22f;
            decoPhase[i] = (float)r.NextDouble() * 6.28f;
        }
    }

    void NewScreen()
    {
        if (scr != null) Destroy(scr.gameObject);
        scr = UIKit.Box(window, "Screen", 0, 0, 900, 1580);
        timerFill = null;
        timerLabel = null;
    }

    // ====================================================== হাব: তিনটা মোড
    static readonly string[] ModeTitles = { "ROLE SPOTTER", "FILL THE BLANK", "WORD TYPE" };
    static readonly string[] ModeSubs =
    {
        "Tap the Subject, Verb or Object of a sentence",
        "Pick the right word for the blank",
        "Is it a noun, a verb, an adjective...?"
    };

    void ShowHub()
    {
        NewScreen();
        screenKind = 0;
        locked = true;

        UIKit.Label(scr, "Questions come from the words you learned", 60, 192, 780, 44, 27, C(236, 216, 255, 0.9f), TextAlignmentOptions.Center);

        for (int i = 0; i < 3; i++) BuildModeCard(i, 262f + i * 300f);

        Chip(scr, "10 QUESTIONS", 60, 1180, 240, 56, C(214, 160, 255), C(124, 60, 224), Color.white, 22);
        Chip(scr, "3 HEARTS", 330, 1180, 240, 56, C(255, 130, 150), C(214, 30, 70), Color.white, 22);
        Chip(scr, "20 SEC EACH", 600, 1180, 240, 56, C(255, 226, 120), C(240, 150, 20), C(60, 34, 4), 22);
        UIKit.Label(scr, "+5 coins and +3 XP for every right answer", 60, 1264, 780, 46, 28, C(236, 216, 255, 0.9f), TextAlignmentOptions.Center);
        UIKit.Label(scr, "Words learned so far: " + PlayerData.CompletedLevels, 60, 1316, 780, 46, 28, C(214, 170, 255, 0.85f), TextAlignmentOptions.Center);
    }

    void BuildModeCard(int i, float y)
    {
        float x = 45f, w = 810f, h = 270f;
        Neo.Surface(scr, "Card", x, y, w, h,
            St(40, C(176, 108, 255, 0.60f), C(84, 40, 190, 0.58f), C(255, 255, 255, 0.50f), C(226, 176, 255, 0.18f),
               2.5f, 0.35f, 0.50f, 36f, 14f, 0.75f));

        // গোল ব্যাজ আর চিহ্ন
        Neo.Surface(scr, "Badge", x + 30, y + 70, 130, 130,
            St(65, C(236, 196, 255), C(130, 64, 230), C(255, 255, 255, 0.85f), C(255, 255, 255, 0.35f), 4f, 0.5f, 0.5f, 16f, 8f, 1f));
        string glyph;
        float gs;
        if (i == 0)
        {
            glyph = "<color=#2F7BE0>S</color><color=#17A84F>V</color><color=#E0791B>O</color>";
            gs = 50f;
        }
        else if (i == 1)
        {
            glyph = "[ _ ]";
            gs = 54f;
        }
        else
        {
            glyph = "Aa";
            gs = 64f;
        }
        UIKit.Label(scr, glyph, x + 30, y + 70, 130, 130, gs, C(86, 36, 170), TextAlignmentOptions.Center);

        Fit(scr, ModeTitles[i], x + 190, y + 30, 360, 56, 42, Color.white, TextAlignmentOptions.Left, true);
        UIKit.Label(scr, ModeSubs[i], x + 190, y + 90, 350, 100, 27, C(236, 216, 255, 0.95f), TextAlignmentOptions.Left);
        int best = PlayerPrefs.GetInt("quiz_best_" + i, 0);
        Chip(scr, "BEST  " + best + "/" + Total, x + 190, y + 202, 230, 52, C(255, 230, 120), C(240, 150, 20), C(60, 34, 4), 24);

        // START বোতাম
        Neo.Surface(scr, "Start", x + w - 230, y + 96, 200, 84,
            St(42, C(255, 238, 112), C(246, 172, 20), C(255, 222, 100), C(208, 126, 12), 5f, 0.6f, 0.5f, 18f, 8f, 0.75f));
        UIKit.Label(scr, "START", x + w - 230, y + 96, 200, 84, 38, C(64, 36, 6), TextAlignmentOptions.Center);

        int idx = i;
        UIKit.Hit(scr, x, y, w, h, () => StartMode(idx));
    }

    void StartMode(int m)
    {
        AudioManager.PlayClick();
        mode = m;
        qIndex = 0;
        correct = 0;
        hearts = MaxHearts;
        used.Clear();
        NextQuestion();
    }

    void NextQuestion()
    {
        if (qIndex >= Total || hearts <= 0)
        {
            ShowResult();
            return;
        }
        q = Generate(mode);
        ShowQuestion();
    }

    // ====================================================== প্রশ্ন বানানো
    Question Generate(int m)
    {
        Question res = null;
        for (int t = 0; t < 60 && res == null; t++)
        {
            int level = (m == 2) ? PickTypeLevel() : PickLevel();
            if (m == 0) res = MakeRole(level);
            else if (m == 1) res = MakeFill(level);
            else res = MakeType(level);
        }
        if (res == null) res = MakeType(PickLevel());
        used.Add(res.level);
        return res;
    }

    int PickLevel()
    {
        int pool = Pool;
        for (int t = 0; t < 40; t++)
        {
            int l = Random.Range(0, pool);
            if (!used.Contains(l)) return l;
        }
        return Random.Range(0, pool);
    }

    // বিভিন্ন ধরনের শব্দ যেন আসে, তাই আগে ধরন বাছি, তারপর শব্দ
    int PickTypeLevel()
    {
        Dictionary<string, List<int>> d = new Dictionary<string, List<int>>();
        int pool = Pool;
        for (int l = 0; l < pool; l++)
        {
            if (used.Contains(l)) continue;
            string t = GrammarData.Get(l, WordAt(l)).type;
            if (!d.ContainsKey(t)) d[t] = new List<int>();
            d[t].Add(l);
        }
        if (d.Count == 0) return Random.Range(0, pool);
        List<string> keys = new List<string>(d.Keys);
        List<int> lst = d[keys[Random.Range(0, keys.Count)]];
        return lst[Random.Range(0, lst.Count)];
    }

    static string PlainSentence(GrammarLesson L, bool mark)
    {
        string s = "";
        for (int i = 0; i < L.segs.Length; i++)
        {
            s += GrammarData.SegText(L.segs[i].text, L.word, true, mark);
            if (i < L.segs.Length - 1) s += " ";
        }
        return s;
    }

    static void Shuffle(string[] a, ref int answer)
    {
        string right = a[answer];
        for (int i = a.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string t = a[i];
            a[i] = a[j];
            a[j] = t;
        }
        answer = System.Array.IndexOf(a, right);
    }

    // ---------- মোড ১: Role Spotter ----------
    Question MakeRole(int level)
    {
        GrammarLesson L = GrammarData.Get(level, WordAt(level));
        int lang = GrammarLang.Current;

        List<char> roles = new List<char>();
        for (int i = 0; i < L.segs.Length; i++)
        {
            char tg = L.segs[i].tag;
            if ((tg == 'S' || tg == 'V' || tg == 'O') && !roles.Contains(tg)) roles.Add(tg);
        }
        if (roles.Count == 0) return null;
        char role = roles[Random.Range(0, roles.Count)];
        int ri = role == 'S' ? 0 : (role == 'V' ? 1 : 2);

        Question r = new Question();
        r.level = level;
        r.grid = false;
        r.cardH = 250;
        r.prompt = "Tap the <b><color=#" + GrammarData.Hex(role) + ">" + GrammarData.RoleName(role) + "</color></b> of the sentence";
        r.sentence = PlainSentence(L, false);
        string nat = GrammarLang.RoleNames(lang)[ri];
        r.hint = GrammarLang.RoleDefs(lang)[ri] + (nat.Length > 0 ? "  (" + nat + ")" : "");

        r.opts = new string[L.segs.Length];
        for (int i = 0; i < L.segs.Length; i++)
        {
            r.opts[i] = L.segs[i].text;
            if (L.segs[i].tag == role && r.answer == 0 && i >= 0) r.answer = i;
        }
        // প্রথম মিল পাওয়া অংশই উত্তর
        for (int i = 0; i < L.segs.Length; i++)
        {
            if (L.segs[i].tag == role)
            {
                r.answer = i;
                break;
            }
        }

        r.explainLine = GrammarData.Sentence(L, true);
        r.explainBody = "<b><color=#" + GrammarData.Hex(role) + ">" + GrammarData.RoleName(role) + "</color></b>"
                        + (nat.Length > 0 ? " (" + nat + ")" : "") + " = " + L.segs[r.answer].text + "\n" + GrammarLang.RoleDefs(lang)[ri];
        return r;
    }

    // ---------- মোড ২: Fill the Blank ----------
    // ক্রিয়ার মূল রূপ (বর্তমান কালের ক্রিয়া না হলে null)
    static string BaseOf(string v)
    {
        string w = v.ToLowerInvariant();
        if (w == "had" || w == "is" || w == "are" || w == "am" || w.EndsWith("ed")) return null;
        if (w == "has") return "have";
        if (w.EndsWith("ies")) return w.Substring(0, w.Length - 3) + "y";
        if (w.EndsWith("oes") || w.EndsWith("ches") || w.EndsWith("shes") || w.EndsWith("sses") || w.EndsWith("xes") || w.EndsWith("zes"))
            return w.Substring(0, w.Length - 2);
        if (w.EndsWith("s") && !w.EndsWith("ss") && !w.EndsWith("us")) return w.Substring(0, w.Length - 1);
        return w;
    }

    static string ThirdOf(string b)
    {
        if (b == "have") return "has";
        if (b.EndsWith("y") && b.Length > 1 && "aeiou".IndexOf(b[b.Length - 2]) < 0) return b.Substring(0, b.Length - 1) + "ies";
        if (b.EndsWith("s") || b.EndsWith("sh") || b.EndsWith("ch") || b.EndsWith("x") || b.EndsWith("z") || b.EndsWith("o")) return b + "es";
        return b + "s";
    }

    static string Ing(string b)
    {
        if (b == "cut" || b == "run" || b == "get" || b == "sit" || b == "stop" || b == "put") return b + b[b.Length - 1] + "ing";
        if (b.EndsWith("ie")) return b.Substring(0, b.Length - 2) + "ying";
        if (b.EndsWith("e") && !b.EndsWith("ee") && !b.EndsWith("ye") && !b.EndsWith("oe")) return b.Substring(0, b.Length - 1) + "ing";
        return b + "ing";
    }

    static string Blank(bool good)
    {
        return good ? "" : "<color=#FFD66E><u>_____</u></color>";
    }

    Question MakeFill(int level)
    {
        GrammarLesson L = GrammarData.Get(level, WordAt(level));
        int lang = GrammarLang.Current;
        GrammarSeg[] segs = L.segs;

        List<int> verbs = new List<int>();
        List<int> arts = new List<int>();
        for (int i = 0; i < segs.Length; i++)
        {
            string t = segs[i].text;
            if (segs[i].tag == 'V' && t.IndexOf(' ') < 0 && BaseOf(t) != null) verbs.Add(i);
            string tl = t.ToLowerInvariant();
            if (tl.StartsWith("a ") || tl.StartsWith("an ")) arts.Add(i);
        }
        if (verbs.Count == 0 && arts.Count == 0) return null;

        Question r = new Question();
        r.level = level;
        r.grid = true;
        r.cardH = 250;
        r.prompt = "Choose the correct word";

        bool useVerb = verbs.Count > 0 && (arts.Count == 0 || Random.value < 0.5f);
        string blanked = "";
        string filled = "";
        string right;
        string[] opts;

        if (useVerb)
        {
            int vi = verbs[Random.Range(0, verbs.Count)];
            string v = segs[vi].text;
            string b = BaseOf(v);
            bool isThird = v.ToLowerInvariant() != b;
            string other = isThird ? b : ThirdOf(b);
            opts = new string[] { v, other, Ing(b), "to " + b };
            right = v;
            for (int i = 0; i < segs.Length; i++)
            {
                string part = (i == vi) ? null : segs[i].text;
                blanked += (part ?? Blank(false)) + (i < segs.Length - 1 ? " " : "");
                filled += (part ?? "<color=#7DFFA0><u>" + v + "</u></color>") + (i < segs.Length - 1 ? " " : "");
            }
            r.explainBody = Tip(isThird ? 0 : 1, lang, v);
        }
        else
        {
            int ai = arts[Random.Range(0, arts.Count)];
            string t = segs[ai].text;
            int sp = t.IndexOf(' ');
            string art = t.Substring(0, sp);
            string rest = t.Substring(sp + 1);
            bool cap = char.IsUpper(art[0]);
            string[] baseOpts = { "a", "an", "two", "many" };
            opts = new string[4];
            for (int k = 0; k < 4; k++) opts[k] = cap ? char.ToUpperInvariant(baseOpts[k][0]) + baseOpts[k].Substring(1) : baseOpts[k];
            right = art;
            for (int i = 0; i < segs.Length; i++)
            {
                string bl = segs[i].text;
                string fl = segs[i].text;
                if (i == ai)
                {
                    bl = Blank(false) + " " + rest;
                    fl = "<color=#7DFFA0><u>" + art + "</u></color> " + rest;
                }
                blanked += bl + (i < segs.Length - 1 ? " " : "");
                filled += fl + (i < segs.Length - 1 ? " " : "");
            }
            int sp2 = rest.IndexOf(' ');
            r.explainBody = Tip(2, lang, art.ToLowerInvariant() + " " + (sp2 > 0 ? rest.Substring(0, sp2) : rest));
        }

        int ans = System.Array.IndexOf(opts, right);
        if (ans < 0) return null;
        Shuffle(opts, ref ans);
        r.opts = opts;
        r.answer = ans;
        r.sentence = blanked;
        r.explainLine = filled;
        return r;
    }

    // ---------- মোড ৩: Word Type ----------
    Question MakeType(int level)
    {
        GrammarLesson L = GrammarData.Get(level, WordAt(level));
        int lang = GrammarLang.Current;

        Question r = new Question();
        r.level = level;
        r.grid = true;
        r.cardH = 290;
        r.prompt = "What type of word is this?";
        r.bigWord = L.word;
        r.sentence = PlainSentence(L, true);

        List<string> all = new List<string> { "NOUN", "VERB", "ADJECTIVE", "ADVERB", "PRONOUN", "PREPOSITION", "DETERMINER" };
        all.Remove(L.type);
        string[] opts = new string[4];
        opts[0] = L.type;
        for (int k = 1; k < 4; k++)
        {
            int j = Random.Range(0, all.Count);
            opts[k] = all[j];
            all.RemoveAt(j);
        }
        int ans = 0;
        Shuffle(opts, ref ans);
        r.opts = opts;
        r.answer = ans;

        string meaning = GrammarLang.Meaning(level, lang);
        r.explainLine = "<b>" + L.word + "</b>" + (meaning.Length > 0 ? "  <color=#FFD66E>- " + meaning + "</color>" : "") + "  =  " + L.type;
        r.explainBody = GrammarLang.Note(level, L, lang);
        return r;
    }

    // ====================================================== প্রশ্নের পর্দা
    void ShowQuestion()
    {
        NewScreen();
        screenKind = 1;
        locked = false;
        timeLeft = TimePer;

        // হার্ট, কত নম্বর প্রশ্ন, বাকি সময়
        heartImgs = new Image[MaxHearts];
        for (int i = 0; i < MaxHearts; i++)
        {
            Image im = UIKit.Box(scr, "Heart", 60 + i * 66, 196, 60, 60).gameObject.AddComponent<Image>();
            im.sprite = Heart(i < hearts);
            im.preserveAspect = true;
            im.raycastTarget = false;
            heartImgs[i] = im;
        }
        UIKit.Label(scr, (qIndex + 1) + " / " + Total, 300, 190, 300, 70, 40, Color.white, TextAlignmentOptions.Center);
        timerLabel = UIKit.Label(scr, Mathf.CeilToInt(TimePer).ToString(), 680, 190, 160, 70, 40, C(255, 226, 120), TextAlignmentOptions.Right);
        Neo.Bar(scr, "TimerTrack", 60, 272, 780, 26, C(10, 6, 40, 0.9f), C(46, 30, 100, 0.9f), 0f, true);
        timerFill = Neo.Bar(scr, "TimerFill", 60, 272, 780, 26, C(255, 226, 110), C(240, 140, 20), 0.6f, false);
        SetBar(timerFill, 780f, 26f, 1f);

        // প্রশ্নের কার্ড
        float cy = 322f;
        float ch = q.cardH;
        Neo.Surface(scr, "QCard", 45, cy, 810, ch,
            St(38, C(176, 108, 255, 0.55f), C(84, 40, 190, 0.55f), C(255, 255, 255, 0.45f), C(226, 176, 255, 0.15f),
               2.5f, 0.35f, 0.45f, 32f, 12f, 0.75f));

        if (q.bigWord == null && !q.grid)
        {
            // Role Spotter
            UIKit.Label(scr, q.prompt, 70, cy + 24, 760, 60, 38, Color.white, TextAlignmentOptions.Center);
            UIKit.Label(scr, q.sentence, 70, cy + 88, 760, 74, 46, C(255, 240, 200), TextAlignmentOptions.Center);
            UIKit.Label(scr, q.hint, 70, cy + 168, 760, 70, 26, C(236, 216, 255, 0.92f), TextAlignmentOptions.Center);
        }
        else if (q.bigWord == null)
        {
            // Fill the Blank
            UIKit.Label(scr, q.prompt, 70, cy + 22, 760, 50, 30, C(236, 216, 255, 0.92f), TextAlignmentOptions.Center);
            UIKit.Label(scr, q.sentence, 70, cy + 76, 760, 150, 52, Color.white, TextAlignmentOptions.Center);
        }
        else
        {
            // Word Type
            UIKit.Label(scr, q.prompt, 70, cy + 22, 760, 50, 30, C(236, 216, 255, 0.92f), TextAlignmentOptions.Center);
            Fit(scr, q.bigWord, 70, cy + 72, 760, 110, 86, C(255, 232, 130), TextAlignmentOptions.Center, true);
            UIKit.Label(scr, q.sentence, 70, cy + 196, 760, 70, 34, Color.white, TextAlignmentOptions.Center);
        }

        BuildAnswers(cy + ch + 30f);
    }

    void BuildAnswers(float y0)
    {
        int n = q.opts.Length;
        skins = new Image[n][];
        for (int i = 0; i < n; i++)
        {
            float w, h, x, y;
            if (q.grid)
            {
                w = 396f;
                h = 120f;
                x = 45f + (i % 2) * 414f;
                y = y0 + (i / 2) * 138f;
            }
            else
            {
                w = 810f;
                h = 104f;
                x = 45f;
                y = y0 + i * 122f;
            }
            int idx = i;
            RectTransform b = Btn(scr, "Ans" + i, x + w * 0.5f, y + h * 0.5f, w, h, () => Pick(idx));
            skins[i] = new Image[3];
            skins[i][0] = Neo.Surface(b, "Idle", 0, 0, w, h,
                St(h * 0.42f, C(255, 255, 255, 0.30f), C(190, 140, 255, 0.14f), C(255, 255, 255, 0.70f), C(206, 150, 255, 0.35f),
                   3f, 0.4f, 0.45f, 22f, 10f, 0.75f));
            skins[i][1] = Neo.Surface(b, "Good", 0, 0, w, h,
                St(h * 0.42f, C(116, 244, 146), C(26, 170, 78), C(255, 255, 255, 0.85f), C(80, 220, 130, 0.5f), 4f, 0.55f, 0.5f, 22f, 10f, 0.75f));
            skins[i][2] = Neo.Surface(b, "Bad", 0, 0, w, h,
                St(h * 0.42f, C(255, 124, 112), C(210, 28, 54), C(255, 255, 255, 0.85f), C(255, 100, 100, 0.5f), 4f, 0.55f, 0.5f, 22f, 10f, 0.75f));
            skins[i][1].gameObject.SetActive(false);
            skins[i][2].gameObject.SetActive(false);
            Fit(b, q.opts[i], 16, 0, w - 32, h, q.grid ? 40f : 36f, Color.white, TextAlignmentOptions.Center, true);
        }
    }

    void Pick(int idx)
    {
        if (locked || screenKind != 1) return;
        AudioManager.PlayClick();
        Reveal(idx, idx == q.answer);
    }

    // picked = -1 হলে সময় শেষ
    void Reveal(int picked, bool ok)
    {
        locked = true;
        skins[q.answer][0].gameObject.SetActive(false);
        skins[q.answer][1].gameObject.SetActive(true);
        if (!ok && picked >= 0)
        {
            skins[picked][0].gameObject.SetActive(false);
            skins[picked][2].gameObject.SetActive(true);
        }

        if (ok)
        {
            correct++;
        }
        else
        {
            hearts = Mathf.Max(0, hearts - 1);
            AudioManager.Vibrate();
            for (int i = 0; i < heartImgs.Length; i++) heartImgs[i].sprite = Heart(i < hearts);
        }
        ShowFeedback(ok, picked < 0);
    }

    void ShowFeedback(bool ok, bool timeout)
    {
        float x = 45f, y = 1090f, w = 810f, h = 440f;
        Color top = ok ? C(60, 200, 120, 0.92f) : C(214, 60, 90, 0.92f);
        Color bot = ok ? C(20, 120, 80, 0.94f) : C(130, 24, 70, 0.94f);
        Neo.Surface(scr, "Feedback", x, y, w, h,
            St(40, top, bot, C(255, 255, 255, 0.7f), C(255, 255, 255, 0.2f), 3f, 0.4f, 0.55f, 36f, 14f, 0.75f));

        string head = ok ? "CORRECT!" : (timeout ? "TIME UP" : "NOT QUITE");
        TMP_Text ht = UIKit.Label(scr, head, x, y + 18, w, 70, 54, Color.white, TextAlignmentOptions.Center, true);
        ht.characterSpacing = 4f;
        UIKit.Label(scr, q.explainLine, x + 30, y + 92, w - 60, 90, 36, Color.white, TextAlignmentOptions.Center);
        UIKit.Label(scr, q.explainBody, x + 30, y + 186, w - 60, 140, 28, C(255, 255, 255, 0.95f), TextAlignmentOptions.Center);

        bool last = qIndex + 1 >= Total || hearts <= 0;
        Neo.Surface(scr, "NextBtn", x + w * 0.5f - 190f, y + h - 98f, 380, 78,
            St(39, C(255, 238, 112), C(246, 172, 20), C(255, 222, 100), C(208, 126, 12), 5f, 0.6f, 0.5f, 16f, 8f, 0.75f));
        UIKit.Label(scr, last ? "RESULT" : "NEXT", x + w * 0.5f - 190f, y + h - 98f, 380, 78, 38, C(64, 36, 6), TextAlignmentOptions.Center);
        UIKit.Hit(scr, x + w * 0.5f - 190f, y + h - 98f, 380, 78, () =>
        {
            AudioManager.PlayClick();
            qIndex++;
            NextQuestion();
        });
    }

    // ====================================================== ফলাফল
    void ShowResult()
    {
        NewScreen();
        screenKind = 2;
        locked = true;

        int stars = correct >= 9 ? 3 : (correct >= 7 ? 2 : (correct >= 4 ? 1 : 0));
        int coins = correct * 5;
        int xp = correct * 3;
        PlayerData.Coins += coins;
        PlayerData.Xp += xp;
        PlayerData.Stars += stars;
        int best = PlayerPrefs.GetInt("quiz_best_" + mode, 0);
        if (correct > best)
        {
            PlayerPrefs.SetInt("quiz_best_" + mode, correct);
            PlayerPrefs.Save();
        }

        bool dead = hearts <= 0;
        TMP_Text title = UIKit.Label(scr, dead ? "OUT OF HEARTS" : "QUIZ COMPLETE", 0, 196, 900, 80, 52,
                                     dead ? C(255, 130, 150) : C(255, 226, 110), TextAlignmentOptions.Center);
        title.characterSpacing = 6f;
        UIKit.Label(scr, correct + " / " + Total, 0, 280, 900, 150, 120, Color.white, TextAlignmentOptions.Center, true);

        float sz = 150f, gap = 24f;
        float sx = (900f - (3 * sz + 2 * gap)) * 0.5f;
        for (int i = 0; i < 3; i++)
        {
            bool on = i < stars;
            Neo.Icon(scr, sx + i * (sz + gap), 450, sz, "star",
                     on ? C(255, 240, 150) : C(150, 130, 190), on ? C(240, 150, 20) : C(84, 64, 130));
        }

        Chip(scr, "+" + coins + " COINS", 70, 650, 240, 62, C(255, 230, 120), C(240, 150, 20), C(60, 34, 4), 26);
        Chip(scr, "+" + xp + " XP", 330, 650, 240, 62, C(150, 210, 255), C(60, 130, 235), Color.white, 26);
        Chip(scr, "+" + stars + " STARS", 590, 650, 240, 62, C(236, 180, 255), C(150, 70, 230), Color.white, 26);

        string msg = correct >= 9 ? "Excellent! You know your grammar!" : (correct >= 7 ? "Great job! Keep going!" : (correct >= 4 ? "Good try! Practice makes perfect." : "Don't give up! Try again."));
        UIKit.Label(scr, msg, 60, 750, 780, 60, 34, C(236, 216, 255, 0.95f), TextAlignmentOptions.Center);

        // বোতাম
        Neo.Surface(scr, "Again", 100, 880, 700, 120,
            St(60, C(255, 238, 112), C(246, 172, 20), C(255, 222, 100), C(208, 126, 12), 6f, 0.6f, 0.55f, 26f, 12f, 0.75f));
        UIKit.Label(scr, "PLAY AGAIN", 100, 880, 700, 120, 46, C(64, 36, 6), TextAlignmentOptions.Center);
        UIKit.Hit(scr, 100, 880, 700, 120, () => StartMode(mode));

        Neo.Surface(scr, "Back", 100, 1030, 700, 110,
            St(55, C(255, 255, 255, 0.30f), C(190, 140, 255, 0.14f), C(255, 255, 255, 0.70f), C(206, 150, 255, 0.35f), 3f, 0.4f, 0.45f, 22f, 10f, 0.75f));
        UIKit.Label(scr, "BACK TO QUIZ", 100, 1030, 700, 110, 40, Color.white, TextAlignmentOptions.Center, true);
        UIKit.Hit(scr, 100, 1030, 700, 110, () =>
        {
            AudioManager.PlayClick();
            ShowHub();
        });
    }

    // ====================================================== নড়াচড়া
    void Update()
    {
        if (root == null || !root.activeSelf) return;

        float t = Time.unscaledTime - openTime;
        window.localScale = Vector3.one * Mathf.Lerp(0.9f, 1f, EaseOutBack(Mathf.Clamp01(t / 0.45f)));
        group.alpha = Mathf.Clamp01(t / 0.25f);

        float dt = Time.unscaledDeltaTime;
        float tm = Time.unscaledTime;
        if (decoRt != null)
        {
            for (int i = 0; i < decoRt.Length; i++)
            {
                if (decoRt[i] == null) continue;
                Vector2 p = decoRt[i].anchoredPosition;
                float size = decoRt[i].sizeDelta.y;
                p.y += decoSpeed[i] * dt;
                if (p.y > size) p.y = -(1920f + size);
                p.x = decoBaseX[i] + Mathf.Sin(tm * 0.4f + decoPhase[i]) * 30f;
                decoRt[i].anchoredPosition = p;
            }
        }

        // সময়ের বার
        if (screenKind == 1 && !locked && timerFill != null)
        {
            timeLeft -= dt;
            SetBar(timerFill, 780f, 26f, Mathf.Max(0f, timeLeft) / TimePer);
            timerFill.color = timeLeft < 5f ? new Color(1f, 0.55f, 0.55f) : Color.white;
            if (timerLabel != null) timerLabel.text = Mathf.Max(0, Mathf.CeilToInt(timeLeft)).ToString();
            if (timeLeft <= 0f) Reveal(-1, false);
        }
    }
}