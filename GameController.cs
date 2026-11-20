using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// নতুন ধাঁচের লেভেল পাতা। পুরোটা কোড দিয়ে আঁকা হয়, Inspector এ কিছু করতে হয় না।
// সিনের পুরোনো LevelBackground, LetterArea, AnswerArea, PlayButton, ScorePanel, BackButton নিজে নিজেই লুকিয়ে যায়।
// লাগে: Neo, UIKit, UIShapes, AudioManager, PlayerData, GrammarData।
// গান বা শব্দ চাইলে Assets/Resources এ correct, wrong, win, lose নামে ফাইল রাখুন (না থাকলেও চলে)।
public class GameController : MonoBehaviour
{
    // পুরোনো ঘরগুলো রাখা হলো যাতে সিনের সংযোগ নষ্ট না হয়, এখন আর লাগে না
    public GameObject tilePrefab;
    public Transform answerArea;
    public Transform letterArea;
    public TMP_Text levelText;
    public GameObject playButton;
    public int startLevel = 0;

    const int MaxHearts = 5;
    const int HintCost = 20;
    const int ReviveCost = 30;

    static readonly string[] tierNames =
    {
        "EASY", "BASIC", "NORMAL", "MEDIUM", "ADVANCED",
        "HARD", "EXPERT", "MASTER", "GENIUS", "LEGEND"
    };

    static readonly string[][] tiers = new string[][]
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

    // ---------- ১০ লেভেল পর পর নতুন রঙের থিম ----------
    class Theme
    {
        public Color top, bottom, accent, second;
    }

    static Theme Th(Color top, Color bottom, Color accent, Color second)
    {
        Theme t = new Theme();
        t.top = top;
        t.bottom = bottom;
        t.accent = accent;
        t.second = second;
        return t;
    }

    static readonly Theme[] themes =
    {
        Th(Neo.C(14, 78, 62), Neo.C(6, 30, 38), Neo.C(90, 225, 125), Neo.C(255, 230, 120)),     // EASY: বনের সবুজ
        Th(Neo.C(8, 84, 110), Neo.C(4, 28, 62), Neo.C(70, 215, 195), Neo.C(120, 200, 255)),     // BASIC: সাগরের নীলাভ সবুজ
        Th(Neo.C(24, 84, 170), Neo.C(10, 26, 90), Neo.C(90, 175, 255), Neo.C(190, 230, 255)),   // NORMAL: আকাশী নীল
        Th(Neo.C(48, 48, 150), Neo.C(16, 14, 66), Neo.C(140, 140, 255), Neo.C(255, 170, 220)),  // MEDIUM: গোধূলির নীল
        Th(Neo.C(86, 34, 150), Neo.C(30, 10, 76), Neo.C(195, 115, 255), Neo.C(120, 220, 255)),  // ADVANCED: বেগুনি
        Th(Neo.C(140, 28, 106), Neo.C(54, 10, 58), Neo.C(255, 115, 205), Neo.C(255, 200, 120)), // HARD: গোলাপি
        Th(Neo.C(150, 46, 44), Neo.C(60, 16, 46), Neo.C(255, 125, 90), Neo.C(255, 210, 110)),   // EXPERT: সূর্যাস্তের লাল
        Th(Neo.C(134, 78, 18), Neo.C(54, 28, 14), Neo.C(255, 175, 65), Neo.C(255, 120, 80)),    // MASTER: অ্যাম্বার
        Th(Neo.C(26, 30, 92), Neo.C(6, 8, 34), Neo.C(65, 235, 255), Neo.C(190, 110, 255)),      // GENIUS: মহাকাশ
        Th(Neo.C(92, 70, 12), Neo.C(16, 14, 10), Neo.C(255, 218, 95), Neo.C(255, 150, 60)),     // LEGEND: রাজকীয় সোনালি
    };

    class Tile
    {
        public char ch;
        public RectTransform rt;
        public Image face;
        public Vector2 home;
        public bool used;
        public bool shaking;
    }

    class Slot
    {
        public RectTransform node;
        public Image filled;
        public TMP_Text label;
    }

    // ---------- খেলার অবস্থা ----------
    int level;
    string target;
    int nextIndex;
    int hearts;
    int mistakes;
    bool inputLocked = true;
    bool introDone;
    int runId;
    float slotSize;
    float tileSize;
    readonly List<Tile> tiles = new List<Tile>();
    readonly List<Slot> slots = new List<Slot>();

    // ---------- আঁকা অংশ ----------
    RectTransform root;
    RectTransform bgRoot;
    Image bgImage;
    Image glowA;
    Image glowB;
    RectTransform upper;
    RectTransform lower;
    RectTransform board;
    GameObject toast;

    TMP_Text levelLabel;
    TMP_Text tierLabel;
    TMP_Text coinsLabel;
    TMP_Text starsLabel;
    TMP_Text lvLabel;
    TMP_Text xpLabel;
    Image xpFill;
    RectTransform[] heartNodes;
    Image[] heartImgs;

    class Deco
    {
        public RectTransform rt;
        public Image img;
        public Color baseCol;
        public float size, x, y, vx, vy, sway, swayF, phase, spin, rot, twinkle, twinkleF;
    }

    readonly List<Deco> decos = new List<Deco>();
    RectTransform decoRoot;
    RectTransform fxRoot;
    int appliedTier = -1;
    bool busy;
    readonly Sprite[] bgSprites = new Sprite[10];

    // ====================================================== শুরু
    void Start()
    {
        HideOldUi();
        BuildUi();
        level = startLevel > 0 ? startLevel : PlayerData.SavedLevel;
        StartCoroutine(RunLevel());
    }

    void HideOldUi()
    {
        string[] names = { "LevelBackground", "LeaveText", "LetterArea", "AnswerArea", "PlayButton", "ScorePanel", "BackButton" };
        RectTransform[] all = FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            RectTransform r = all[i];
            if (!r.gameObject.scene.IsValid()) continue;
            for (int k = 0; k < names.Length; k++)
            {
                if (r.name == names[k]) r.gameObject.SetActive(false);
            }
        }
    }

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

    // কেন্দ্র ধরে বসানো ঘর (cx, cy = মা-র বাম-উপর কোণ থেকে মাঝখানের দূরত্ব)
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

    // পর্দার ওপর বা নিচে লাগানো ১০৮০ চওড়া ফ্রেম
    static RectTransform Frame(Transform parent, string name, float ax, float ay, float w, float h)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(ax, ay);
        rt.anchorMax = new Vector2(ax, ay);
        rt.pivot = new Vector2(ax, ay);
        rt.sizeDelta = new Vector2(w, h);
        rt.anchoredPosition = Vector2.zero;
        return rt;
    }

    static void Chip(Transform p, string text, float x, float y, float w, float h,
                     Color top, Color bottom, Color textColor, float size)
    {
        Neo.Surface(p, "Chip", x, y, w, h,
            St(h * 0.5f, top, bottom, C(255, 255, 255, 0.7f), C(255, 255, 255, 0.15f), 2.5f, 0.5f, 0.45f, 14f, 6f, 1f));
        UIKit.Label(p, text, x, y, w, h, size, textColor, TextAlignmentOptions.Center);
    }

    static void Button3D(Transform p, string text, float x, float y, float w, float h,
                         Color top, Color bottom, Color textColor, float size, UnityAction act)
    {
        Neo.Surface(p, "Btn", x, y, w, h,
            St(h * 0.5f, top, bottom, C(255, 255, 255, 0.75f), Neo.Mix(bottom, 1f, 60f), 3f, 0.55f, 0.5f, 22f, 10f, 0.75f));
        UIKit.Label(p, text, x, y, w, h, size, textColor, TextAlignmentOptions.Center);
        UIKit.Hit(p, x, y, w, h, act);
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

    static float EaseOutCubic(float t)
    {
        float u = 1f - t;
        return 1f - u * u * u;
    }

    static readonly Dictionary<string, AudioClip> sfxCache = new Dictionary<string, AudioClip>();

    static void Sfx(string clipName)
    {
        AudioClip c;
        if (!sfxCache.TryGetValue(clipName, out c))
        {
            c = Resources.Load<AudioClip>(clipName);
            sfxCache[clipName] = c;
        }
        AudioManager.PlaySfx(c);
    }

    // ====================================================== হার্টের ছবি (কোড দিয়ে আঁকা)
    static Sprite heartFull;
    static Sprite heartEmpty;

    static Sprite HeartSprite(bool full)
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
        Color topC = full ? new Color(1f, 0.50f, 0.58f) : new Color(0.45f, 0.48f, 0.62f);
        Color botC = full ? new Color(0.84f, 0.10f, 0.25f) : new Color(0.24f, 0.26f, 0.40f);

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
                float t = j / (float)(N - 1);
                Color c = Color.Lerp(botC, topC, t);

                float dx = u0 + 0.45f;
                float dy = v0 - 0.62f;
                float hl = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy) / 0.32f);
                c = Color.Lerp(c, Color.white, hl * 0.6f);

                float ring = Mathf.Clamp01(cov - covIn);
                float k = 1f - 0.35f * ring;
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

    // ====================================================== পুরো UI আঁকা
    void BuildUi()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        root = UIShapes.Stretch(canvas.rootCanvas.transform, "GameUI");

        BuildBackground();
        upper = Frame(root, "Upper", 0.5f, 1f, 1080f, 1500f);
        lower = Frame(root, "Lower", 0.5f, 0f, 1080f, 330f);
        BuildHud();
        BuildButtons();
    }

    // ---------- পটভূমি: গ্র্যাডিয়েন্ট, আলো আর প্রতিটা টিয়ারের নিজস্ব সাজ ----------
    void BuildBackground()
    {
        bgRoot = UIShapes.Stretch(root, "Bg");
        bgImage = bgRoot.gameObject.AddComponent<Image>();
        bgImage.raycastTarget = false;

        glowA = Neo.Glow(bgRoot, "GlowA", -300, 100, 1000, Color.white, 0.55f);
        glowB = Neo.Glow(bgRoot, "GlowB", 380, 1000, 1000, Color.white, 0.45f);
    }

    void ApplyTheme(int tier)
    {
        tier = Mathf.Clamp(tier, 0, 9);
        if (tier == appliedTier) return;
        appliedTier = tier;
        Theme th = themes[tier];

        if (bgSprites[tier] == null) bgSprites[tier] = Neo.VerticalGradient(th.top, th.bottom);
        bgImage.sprite = bgSprites[tier];
        glowA.color = th.accent;
        glowB.color = th.second;

        Canvas.ForceUpdateCanvases();
        if (decoRoot != null) Destroy(decoRoot.gameObject);
        decos.Clear();
        decoRoot = UIShapes.Stretch(bgRoot, "Deco");
        BuildDeco(tier, th);
    }

    static Color A(Color c, float a)
    {
        c.a = a;
        return c;
    }

    static float Rf(System.Random r, float a, float b)
    {
        return a + (float)r.NextDouble() * (b - a);
    }

    Deco NewDeco(float w, float h, float x, float y)
    {
        Deco d = new Deco();
        d.size = Mathf.Max(w, h);
        d.x = x;
        d.y = y;
        d.rt = Node(decoRoot, "Deco", x, y, w, h);
        decos.Add(d);
        return d;
    }

    Deco DotDeco(float dia, float x, float y, Color c)
    {
        Deco d = NewDeco(dia, dia, x, y);
        d.img = UIKit.Round(d.rt, "Dot", 0, 0, dia, dia, dia * 0.5f, c);
        d.baseCol = c;
        return d;
    }

    Deco IconDeco(string icon, float size, float x, float y, Color top, Color bottom, float alpha)
    {
        Deco d = NewDeco(size, size, x, y);
        d.img = Neo.Icon(d.rt, 0, 0, size, icon, top, bottom);
        d.baseCol = new Color(1f, 1f, 1f, alpha);
        d.img.color = d.baseCol;
        return d;
    }

    Deco TintIconDeco(string icon, float size, float x, float y, Color tint)
    {
        Deco d = NewDeco(size, size, x, y);
        d.img = UIKit.Icon(d.rt, 0, 0, size, icon, tint);
        d.baseCol = tint;
        return d;
    }

    Deco HeartDeco(float size, float x, float y, Color tint)
    {
        Deco d = NewDeco(size, size, x, y);
        Image im = UIKit.Box(d.rt, "Heart", 0, 0, size, size).gameObject.AddComponent<Image>();
        im.sprite = HeartSprite(true);
        im.preserveAspect = true;
        im.raycastTarget = false;
        im.color = tint;
        d.img = im;
        d.baseCol = tint;
        return d;
    }

    Deco CloudDeco(float w, float x, float y, float alpha)
    {
        float h = w * 0.42f;
        Deco d = NewDeco(w, h, x, y);
        Color c = new Color(1f, 1f, 1f, alpha);
        UIKit.Round(d.rt, "Base", 0, h * 0.45f, w, h * 0.55f, h * 0.27f, c);
        UIKit.Round(d.rt, "Puff1", w * 0.12f, h * 0.18f, w * 0.34f, w * 0.34f, w * 0.17f, c);
        UIKit.Round(d.rt, "Puff2", w * 0.36f, 0f, w * 0.40f, w * 0.40f, w * 0.20f, c);
        UIKit.Round(d.rt, "Puff3", w * 0.60f, h * 0.22f, w * 0.28f, w * 0.28f, w * 0.14f, c);
        return d;
    }

    static void Drift(Deco d, System.Random r, float a, float b)
    {
        d.sway = Rf(r, a, b);
        d.swayF = Rf(r, 0.4f, 1.1f);
        d.phase = Rf(r, 0f, 6.28f);
    }

    static void Twinkle(Deco d, System.Random r, float amount, float f0, float f1)
    {
        d.twinkle = amount;
        d.twinkleF = Rf(r, f0, f1);
        d.phase = Rf(r, 0f, 6.28f);
    }

    // প্রতিটা টিয়ারের আলাদা সাজ (রঙের সাথে সাথে নকশাও বদলায়)
    void BuildDeco(int tier, Theme th)
    {
        System.Random r = new System.Random(100 + tier);
        float W = Mathf.Max(bgRoot.rect.width, 1080f);
        float H = Mathf.Max(bgRoot.rect.height, 1920f);
        Color ac = th.accent;
        Color sc = th.second;
        Color top = Neo.Mix(ac, 1f, 120f);

        switch (tier)
        {
            case 0: // জোনাকি আর নরম আলোর দাগ
                for (int i = 0; i < 6; i++)
                {
                    Deco d = DotDeco(Rf(r, 200f, 320f), Rf(r, 0f, W), Rf(r, 0f, H), A(ac, 0.06f));
                    d.vx = Rf(r, -6f, 6f);
                    d.vy = Rf(r, 4f, 10f);
                }
                for (int i = 0; i < 26; i++)
                {
                    Deco d = DotDeco(Rf(r, 8f, 18f), Rf(r, 0f, W), Rf(r, 0f, H), A(i % 3 == 0 ? sc : ac, 0.85f));
                    d.vy = Rf(r, 8f, 22f);
                    Drift(d, r, 20f, 50f);
                    Twinkle(d, r, 0.8f, 1.2f, 2.6f);
                }
                break;

            case 1: // ওপরে ওঠা বুদবুদ
                for (int i = 0; i < 18; i++)
                {
                    float dia = Rf(r, 30f, 120f);
                    Deco d = DotDeco(dia, Rf(r, 0f, W), Rf(r, 0f, H), A(Neo.Mix(ac, 0.4f, 160f), 0.14f));
                    UIKit.Round(d.rt, "Hl", dia * 0.2f, dia * 0.16f, dia * 0.22f, dia * 0.22f, dia * 0.11f, A(Color.white, 0.35f));
                    d.vy = Rf(r, 40f, 90f);
                    Drift(d, r, 15f, 35f);
                }
                break;

            case 2: // ভেসে যাওয়া মেঘ আর তারা
                for (int i = 0; i < 7; i++)
                {
                    Deco d = CloudDeco(Rf(r, 300f, 480f), Rf(r, 0f, W), Rf(r, 100f, H - 100f), Rf(r, 0.07f, 0.13f));
                    d.vx = Rf(r, 10f, 28f);
                }
                for (int i = 0; i < 8; i++)
                {
                    Deco d = IconDeco("star", Rf(r, 18f, 36f), Rf(r, 0f, W), Rf(r, 0f, H), top, ac, 0.8f);
                    Twinkle(d, r, 0.9f, 1.5f, 3f);
                }
                break;

            case 3: // মিটমিট করা তারার আকাশ
                for (int i = 0; i < 28; i++)
                {
                    Deco d = IconDeco("star", Rf(r, 18f, 64f), Rf(r, 0f, W), Rf(r, 0f, H),
                                      Neo.C(255, 250, 200), Neo.C(255, 200, 90), Rf(r, 0.35f, 0.8f));
                    d.vx = Rf(r, -4f, 4f);
                    d.vy = Rf(r, 2f, 8f);
                    d.spin = Rf(r, -10f, 10f);
                    Twinkle(d, r, 0.85f, 1f, 3f);
                }
                break;

            case 4: // ঘুরতে ঘুরতে ভাসা হীরা
                for (int i = 0; i < 14; i++)
                {
                    Deco d = IconDeco("diamond", Rf(r, 50f, 150f), Rf(r, 0f, W), Rf(r, 0f, H), top, ac, Rf(r, 0.12f, 0.22f));
                    d.vy = Rf(r, 18f, 45f);
                    d.spin = Rf(r, -25f, 25f);
                    Drift(d, r, 20f, 40f);
                }
                break;

            case 5: // ভেসে ওঠা হৃদয়
                for (int i = 0; i < 14; i++)
                {
                    Deco d = HeartDeco(Rf(r, 50f, 130f), Rf(r, 0f, W), Rf(r, 0f, H), A(i % 2 == 0 ? ac : sc, Rf(r, 0.14f, 0.24f)));
                    d.vy = Rf(r, 30f, 60f);
                    d.spin = Rf(r, -15f, 15f);
                    Drift(d, r, 25f, 50f);
                }
                break;

            case 6: // আগুনের ফুলকি আর শিখা
                for (int i = 0; i < 32; i++)
                {
                    Deco d = DotDeco(Rf(r, 6f, 16f), Rf(r, 0f, W), Rf(r, 0f, H), A(i % 2 == 0 ? sc : ac, 0.9f));
                    d.vy = Rf(r, 60f, 140f);
                    Drift(d, r, 15f, 40f);
                    Twinkle(d, r, 0.7f, 3f, 6f);
                }
                for (int i = 0; i < 4; i++)
                {
                    Deco d = IconDeco("flame", Rf(r, 120f, 200f), Rf(r, 0f, W), Rf(r, 0f, H),
                                      Neo.C(255, 235, 130), Neo.C(240, 80, 30), 0.12f);
                    d.vy = Rf(r, 12f, 24f);
                    Drift(d, r, 20f, 35f);
                }
                break;

            case 7: // ধীরে ঘোরা গিয়ার
                for (int i = 0; i < 7; i++)
                {
                    Deco d = TintIconDeco("gear", Rf(r, 160f, 420f), Rf(r, 0f, W), Rf(r, 0f, H),
                                          A(i % 2 == 0 ? ac : sc, Rf(r, 0.07f, 0.12f)));
                    d.spin = (i % 2 == 0 ? 1f : -1f) * Rf(r, 6f, 14f);
                    d.vy = Rf(r, -3f, 3f);
                }
                for (int i = 0; i < 12; i++)
                {
                    Deco d = DotDeco(Rf(r, 6f, 12f), Rf(r, 0f, W), Rf(r, 0f, H), A(sc, 0.5f));
                    d.vy = Rf(r, 10f, 25f);
                    Twinkle(d, r, 0.6f, 1f, 2.5f);
                }
                break;

            case 8: // মহাকাশ: তারা আর উল্কা
                for (int i = 0; i < 26; i++)
                {
                    Deco d = IconDeco("star", Rf(r, 12f, 38f), Rf(r, 0f, W), Rf(r, 0f, H),
                                      Neo.C(220, 250, 255), Neo.C(90, 220, 255), Rf(r, 0.4f, 0.9f));
                    Twinkle(d, r, 0.9f, 1.5f, 3.5f);
                }
                for (int i = 0; i < 5; i++)
                {
                    Deco d = NewDeco(360f, 8f, Rf(r, 0f, W), Rf(r, 0f, H));
                    Color sk = A(Color.white, Rf(r, 0.35f, 0.6f));
                    d.img = UIKit.Round(d.rt, "Streak", 0, 0, 360, 8, 4, sk);
                    d.baseCol = sk;
                    d.rot = 45f;
                    d.rt.localRotation = Quaternion.Euler(0f, 0f, 45f);
                    float v = Rf(r, 260f, 380f);
                    d.vx = -v;
                    d.vy = -v;
                }
                break;

            default: // রাজকীয়: মুকুট, ঝিলিক আর সোনার গুঁড়ো
                for (int i = 0; i < 8; i++)
                {
                    Deco d = IconDeco("crown", Rf(r, 90f, 170f), Rf(r, 0f, W), Rf(r, 0f, H),
                                      Neo.C(255, 240, 160), Neo.C(240, 160, 30), Rf(r, 0.12f, 0.2f));
                    d.vy = Rf(r, 12f, 28f);
                    d.spin = Rf(r, -6f, 6f);
                    Drift(d, r, 15f, 30f);
                }
                for (int i = 0; i < 22; i++)
                {
                    Deco d = IconDeco("star", Rf(r, 14f, 30f), Rf(r, 0f, W), Rf(r, 0f, H),
                                      Neo.C(255, 250, 210), Neo.C(255, 205, 80), Rf(r, 0.5f, 0.9f));
                    Twinkle(d, r, 0.9f, 1.5f, 3.5f);
                }
                for (int i = 0; i < 18; i++)
                {
                    Deco d = DotDeco(Rf(r, 5f, 12f), Rf(r, 0f, W), Rf(r, 0f, H), A(Neo.C(255, 215, 90), 0.7f));
                    d.vy = -Rf(r, 25f, 60f);
                    Drift(d, r, 10f, 25f);
                }
                break;
        }
    }

    // ---------- ওপরের অংশ: Back, লেভেল, কয়েন, তারা, XP, হার্ট ----------
    void BuildHud()
    {
        // Back
        Neo.Surface(upper, "BackBtn", 36, 40, 92, 92,
            St(46, C(255, 255, 255, 0.26f), C(255, 255, 255, 0.08f), C(255, 255, 255, 0.55f), C(255, 255, 255, 0.12f),
               2f, 0f, 0.35f, 16f, 8f, 1f));
        UIKit.Icon(upper, 58, 62, 48, "back", Color.white);
        UIKit.Hit(upper, 36, 40, 92, 92, GoBack);

        // লেভেল আর টিয়ারের নাম
        levelLabel = UIKit.Label(upper, "LEVEL 1", 150, 26, 780, 90, 68, Color.white, TextAlignmentOptions.Center);
        levelLabel.characterSpacing = 8f;
        tierLabel = UIKit.Label(upper, "EASY", 150, 112, 780, 44, 30, Color.white, TextAlignmentOptions.Center);
        tierLabel.characterSpacing = 10f;

        // কয়েন
        Neo.Surface(upper, "CoinChip", 790, 32, 260, 60,
            St(30, C(255, 255, 255, 0.20f), C(255, 255, 255, 0.07f), C(255, 255, 255, 0.45f), C(255, 255, 255, 0.10f),
               2f, 0f, 0.3f, 12f, 5f, 1f));
        UIKit.Round(upper, "CoinOuter", 806, 48, 28, 28, 14, C(255, 200, 45));
        UIKit.Round(upper, "CoinInner", 810, 52, 20, 20, 10, C(255, 228, 120));
        coinsLabel = UIKit.Label(upper, "0", 846, 32, 196, 60, 34, Color.white, TextAlignmentOptions.Left);

        // তারা
        Neo.Surface(upper, "StarChip", 790, 102, 260, 52,
            St(26, C(255, 255, 255, 0.20f), C(255, 255, 255, 0.07f), C(255, 255, 255, 0.45f), C(255, 255, 255, 0.10f),
               2f, 0f, 0.3f, 12f, 5f, 1f));
        Neo.Icon(upper, 806, 110, 36, "star", C(255, 236, 140), C(240, 150, 20));
        starsLabel = UIKit.Label(upper, "0", 852, 102, 190, 52, 32, Color.white, TextAlignmentOptions.Left);

        // XP: লেভেল ব্যাজ আর বার
        Neo.Surface(upper, "LvBadge", 36, 176, 100, 100,
            St(50, C(255, 226, 120), C(240, 140, 20), C(255, 255, 255, 0.8f), C(255, 200, 80, 0.6f), 4f, 0.6f, 0.5f, 18f, 8f, 1f));
        lvLabel = UIKit.Label(upper, "LV", 36, 176, 100, 100, 50, C(70, 38, 4), TextAlignmentOptions.Center);

        Neo.Bar(upper, "XpTrack", 156, 214, 700, 40, C(8, 12, 40, 0.9f), C(30, 36, 86, 0.9f), 0f, true);
        xpFill = Neo.Bar(upper, "XpFill", 156, 214, 700, 40, C(255, 226, 110), C(240, 140, 20), 0.6f, false);
        xpLabel = UIKit.Label(upper, "0 / 100 XP", 156, 214, 700, 40, 26, Color.white, TextAlignmentOptions.Center, true);

        // ৫টা হার্ট
        heartNodes = new RectTransform[MaxHearts];
        heartImgs = new Image[MaxHearts];
        float hs = 96f;
        float hg = 16f;
        float hx0 = 36f;
        for (int i = 0; i < MaxHearts; i++)
        {
            RectTransform n = Node(upper, "Heart", hx0 + i * (hs + hg) + hs * 0.5f, 310f + hs * 0.5f, hs, hs);
            Image im = UIKit.Box(n, "Img", 0, 0, hs, hs).gameObject.AddComponent<Image>();
            im.sprite = HeartSprite(true);
            im.raycastTarget = false;
            heartNodes[i] = n;
            heartImgs[i] = im;
        }
    }

    // ---------- নিচের তিনটা বোতাম: RESET, HINT, GRAMMAR LESSON ----------
    void BuildButtons()
    {
        float y = 100f;
        float h = 150f;

        // RESET
        float rx = 40f, rw = 250f;
        Neo.Surface(lower, "ResetBtn", rx, y, rw, h,
            St(44, C(120, 160, 255), C(50, 80, 200), C(255, 255, 255, 0.75f), C(140, 170, 255, 0.6f), 3f, 0.55f, 0.5f, 26f, 12f, 0.75f));
        UIKit.Icon(lower, rx + rw * 0.5f - 34f, y + 20f, 68, "refresh", Color.white);
        UIKit.Label(lower, "RESET", rx, y + 92f, rw, 44, 32, Color.white, TextAlignmentOptions.Center);
        UIKit.Hit(lower, rx, y, rw, h, ResetPressed);

        // HINT
        float hx = 310f, hw = 250f;
        Neo.Surface(lower, "HintBtn", hx, y, hw, h,
            St(44, C(255, 220, 90), C(235, 130, 20), C(255, 255, 255, 0.8f), C(255, 200, 80, 0.6f), 3f, 0.55f, 0.5f, 26f, 12f, 0.75f));
        Neo.Surface(lower, "HintQ", hx + hw * 0.5f - 34f, y + 16f, 68, 68,
            St(34, C(255, 250, 215), C(255, 200, 80), C(255, 255, 255, 0.9f), C(255, 190, 70, 0.7f), 3f, 0.5f, 0.35f, 10f, 5f, 1f));
        UIKit.Label(lower, "?", hx + hw * 0.5f - 34f, y + 16f, 68, 68, 48, C(120, 64, 6), TextAlignmentOptions.Center);
        UIKit.Label(lower, "HINT", hx, y + 92f, hw, 44, 32, C(70, 38, 4), TextAlignmentOptions.Center);
        UIKit.Round(lower, "HintCoin", hx + hw - 58f, y + 12f, 44, 44, 22, C(255, 255, 255, 0.9f));
        UIKit.Round(lower, "HintCoinIn", hx + hw - 54f, y + 16f, 36, 36, 18, C(255, 196, 45));
        UIKit.Label(lower, HintCost.ToString(), hx + hw - 58f, y + 12f, 44, 44, 22, C(70, 38, 4), TextAlignmentOptions.Center);
        UIKit.Hit(lower, hx, y, hw, h, HintPressed);

        // GRAMMAR LESSON
        float gx = 580f, gw = 460f;
        Neo.Surface(lower, "GrammarBtn", gx, y, gw, h,
            St(44, C(200, 125, 255), C(105, 55, 215), C(255, 255, 255, 0.75f), C(190, 140, 255, 0.6f), 3f, 0.55f, 0.5f, 26f, 12f, 0.75f));
        Neo.Surface(lower, "GrammarAa", gx + 34f, y + 35f, 80, 80,
            St(40, C(255, 245, 220), C(255, 205, 120), C(255, 255, 255, 0.9f), C(255, 200, 100, 0.7f), 3f, 0.5f, 0.35f, 10f, 5f, 1f));
        UIKit.Label(lower, "Aa", gx + 34f, y + 35f, 80, 80, 40, C(110, 50, 190), TextAlignmentOptions.Center);
        UIKit.Label(lower, "GRAMMAR\nLESSON", gx + 130f, y + 25f, gw - 150f, 100, 36, Color.white, TextAlignmentOptions.Left);
        UIKit.Hit(lower, gx, y, gw, h, GrammarPressed);
    }

    // ====================================================== লেভেল চালানো
    IEnumerator RunLevel()
    {
        inputLocked = true;
        int id = ++runId;
        ClearBoard();

        if (level >= tiers.Length * 10)
        {
            ShowWin();
            yield break;
        }

        int tier = level / 10;
        ApplyTheme(tier);
        RefreshHud();

        // প্রতি ১০ লেভেলের শুরুতে নতুন স্টেজের নাম আর PLAY
        if (level % 10 == 0)
        {
            introDone = false;
            ShowIntro(tier);
            while (!introDone) yield return null;
            if (id != runId) yield break;
        }

        SetupBoard();
    }

    void SetupBoard()
    {
        target = tiers[level / 10][level % 10];
        nextIndex = 0;
        mistakes = 0;
        hearts = MaxHearts;
        busy = false;
        BuildBoard();
        RefreshHud();
        inputLocked = false;
    }

    void ClearBoard()
    {
        if (board != null) Destroy(board.gameObject);
        board = null;
        tiles.Clear();
        slots.Clear();
    }

    void BuildBoard()
    {
        board = Frame(root, "Board", 0.5f, 0.5f, 1080f, 820f);
        board.anchoredPosition = new Vector2(0f, -140f);
        int n = target.Length;
        Theme th = themes[level / 10];

        UIKit.Label(board, "Tap the letters in the right order", 0, 0, 1080, 48, 30,
                    C(255, 255, 255, 0.7f), TextAlignmentOptions.Center);

        // উত্তরের ঘর
        slotSize = Mathf.Min(120f, (1000f - (n - 1) * 10f) / n);
        float tw = n * slotSize + (n - 1) * 10f;
        float sx0 = (1080f - tw) * 0.5f;
        float sy = 60f;
        for (int i = 0; i < n; i++)
        {
            Slot s = new Slot();
            float cx = sx0 + i * (slotSize + 10f) + slotSize * 0.5f;
            s.node = Node(board, "Slot", cx, sy + slotSize * 0.5f, slotSize, slotSize);

            Neo.Style inset = St(slotSize * 0.22f, C(10, 14, 50, 0.55f), C(36, 42, 100, 0.45f),
                                 C(255, 255, 255, 0.28f), C(255, 255, 255, 0.06f), 2.5f, 0f, 0f, 0f, 0f, 1f);
            inset.inner = true;
            Neo.Surface(s.node, "Face", 0, 0, slotSize, slotSize, inset);

            s.filled = Neo.Surface(s.node, "Filled", 0, 0, slotSize, slotSize,
                St(slotSize * 0.22f, Neo.Mix(th.accent, 0.55f, 100f), Neo.Mix(th.accent, 0.55f, 0f),
                   C(255, 255, 255, 0.8f), th.accent, 3.5f, 0.5f, 0.5f, 14f, 6f, 1f));
            s.filled.gameObject.SetActive(false);
            s.label = UIKit.Label(s.node, "", 0, 0, slotSize, slotSize, slotSize * 0.58f, Color.white,
                                  TextAlignmentOptions.Center, true);
            slots.Add(s);
        }

        // এলোমেলো অক্ষর
        char[] letters = target.ToCharArray();
        int tries = 0;
        do
        {
            for (int i = letters.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                char tmp = letters[i];
                letters[i] = letters[j];
                letters[j] = tmp;
            }
            tries++;
        }
        while (letters.Length > 1 && new string(letters) == target && tries < 20);

        int r1 = n <= 5 ? n : (n + 1) / 2;
        int r2 = n - r1;
        int m = Mathf.Max(r1, r2);
        tileSize = Mathf.Min(160f, (1000f - (m - 1) * 20f) / m);
        float ty0 = 300f;
        for (int i = 0; i < n; i++)
        {
            int row = i < r1 ? 0 : 1;
            int col = row == 0 ? i : i - r1;
            int cnt = row == 0 ? r1 : r2;
            float rowW = cnt * tileSize + (cnt - 1) * 20f;
            float x0 = (1080f - rowW) * 0.5f;
            float cx = x0 + col * (tileSize + 20f) + tileSize * 0.5f;
            float cy = ty0 + row * (tileSize + 30f) + tileSize * 0.5f;
            MakeTile(letters[i], cx, cy, i);
        }
        if (fxRoot != null) fxRoot.SetAsLastSibling();
    }

    void MakeTile(char ch, float cx, float cy, int order)
    {
        Tile t = new Tile();
        t.ch = ch;
        t.rt = Node(board, "Tile", cx, cy, tileSize, tileSize);
        t.home = t.rt.anchoredPosition;
        t.face = Neo.Surface(t.rt, "Face", 0, 0, tileSize, tileSize,
            St(tileSize * 0.24f, C(255, 255, 255), C(204, 214, 246), C(255, 255, 255, 0.95f), C(160, 176, 228, 0.85f),
               3f, 0.35f, 0.5f, 18f, 9f, 1f));
        UIKit.Label(t.rt, ch.ToString(), 0, 0, tileSize, tileSize, tileSize * 0.58f, C(26, 32, 90), TextAlignmentOptions.Center);
        UIKit.Hit(t.rt, 0, 0, tileSize, tileSize, () => OnTile(t));
        t.rt.localScale = Vector3.zero;
        StartCoroutine(PopScale(t.rt, 0.05f * order));
        tiles.Add(t);
    }

    // ====================================================== খেলার নিয়ম
    void OnTile(Tile t)
    {
        if (inputLocked || t.used) return;
        AudioManager.PlayClick();
        if (t.ch == target[nextIndex]) Place(t);
        else Wrong(t);
    }

    void Place(Tile t)
    {
        t.used = true;
        int idx = nextIndex;
        nextIndex++;
        bool last = nextIndex >= target.Length;
        if (last) inputLocked = true;
        Sfx("correct");
        StartCoroutine(Fly(t, idx, last, runId));
    }

    void Wrong(Tile t)
    {
        mistakes++;
        hearts = Mathf.Max(0, hearts - 1);
        AudioManager.Vibrate();
        Sfx("wrong");
        StartCoroutine(Shake(t));
        StartCoroutine(Tint(t.face));
        if (hearts < MaxHearts) StartCoroutine(Punch(heartNodes[hearts], 1.5f));
        RefreshHearts();

        if (hearts <= 0)
        {
            inputLocked = true;
            StartCoroutine(Fail(runId));
        }
    }

    void FillSlot(int i, char ch)
    {
        Slot s = slots[i];
        s.filled.gameObject.SetActive(true);
        s.label.text = ch.ToString();
        StartCoroutine(Punch(s.node, 1.2f));
    }

    void ResetPressed()
    {
        if (busy) return;
        AudioManager.PlayClick();
        runId++;
        inputLocked = true;
        ClearBoard();
        SetupBoard();
    }

    void HintPressed()
    {
        if (inputLocked || nextIndex >= target.Length) return;
        AudioManager.PlayClick();
        if (PlayerData.Coins < HintCost)
        {
            ToastMsg("Not enough coins");
            return;
        }
        Tile pick = null;
        for (int i = 0; i < tiles.Count; i++)
        {
            if (!tiles[i].used && tiles[i].ch == target[nextIndex])
            {
                pick = tiles[i];
                break;
            }
        }
        if (pick == null) return;
        PlayerData.Coins -= HintCost;
        RefreshHud();
        Place(pick);
    }

    void GrammarPressed()
    {
        if (busy) return;
        AudioManager.PlayClick();
        if (target == null) return;
        ShowGrammar(false);
    }

    void GoBack()
    {
        AudioManager.PlayClick();
        SceneManager.LoadScene("MainMenu");
    }

    // ====================================================== লেভেল শেষ
    IEnumerator Complete(int id)
    {
        yield return new WaitForSeconds(0.45f);
        if (id != runId) yield break;
        busy = true;
        Sfx("win");

        bool first = level >= PlayerData.CompletedLevels;
        int stars = mistakes == 0 ? 3 : (mistakes <= 2 ? 2 : 1);
        int lvBefore = PlayerData.PlayerLevel;
        int xpBefore = PlayerData.XpInLevel;

        if (first) PlayerData.LevelCompleted(mistakes);
        PlayerData.SavedLevel = Mathf.Max(PlayerData.SavedLevel, level + 1);

        int lvAfter = PlayerData.PlayerLevel;
        int xpAfter = PlayerData.XpInLevel;
        RefreshHud();

        // আগে উদযাপন (বেলুন, কনফেটি, প্রশংসা), তারপর ফলাফলের পপআপ
        string sub = (first && mistakes == 0 && PlayerData.CurrentStreak >= 2)
            ? PlayerData.CurrentStreak + " PERFECT IN A ROW!" : "";
        Celebrate(PraiseFor(first), sub, mistakes == 0);
        yield return new WaitForSecondsRealtime(1.75f);
        if (id != runId) yield break;

        ShowComplete(stars, first, lvAfter > lvBefore, xpBefore, xpAfter);
        busy = false;
        if (fxRoot != null) fxRoot.SetAsLastSibling();   // বেলুন পপআপের ওপর দিয়েও উড়বে
    }

    // ====================================================== উদযাপন
    static readonly string[] chainPraise =
    {
        "Great!", "Awesome!", "Excellent!", "Amazing!", "Fantastic!",
        "Brilliant!", "Superb!", "Incredible!", "Unstoppable!", "Legendary!"
    };
    static readonly string[] okPraise = { "Nice!", "Well done!", "Good job!" };
    static readonly string[] lowPraise = { "Good!", "Not bad!", "You did it!" };

    // টানা নির্ভুল লেভেল যত বাড়ে, প্রশংসার শব্দ তত বড় হয়
    string PraiseFor(bool first)
    {
        if (mistakes == 0)
        {
            if (!first) return "Perfect!";
            int idx = Mathf.Clamp(PlayerData.CurrentStreak - 1, 0, chainPraise.Length - 1);
            return chainPraise[idx];
        }
        if (mistakes <= 2) return okPraise[Random.Range(0, okPraise.Length)];
        return lowPraise[Random.Range(0, lowPraise.Length)];
    }

    void Celebrate(string praise, string sub, bool perfect)
    {
        if (fxRoot != null) Destroy(fxRoot.gameObject);
        fxRoot = UIShapes.Stretch(root, "Fx");
        float W = Mathf.Max(root.rect.width, 1080f);
        float H = Mathf.Max(root.rect.height, 1920f);

        // কনফেটি
        StartCoroutine(Confetti(fxRoot, perfect ? 110 : 70, W * 0.5f, H * 0.36f));

        // প্রশংসার শব্দ
        RectTransform pn = Node(fxRoot, "Praise", W * 0.5f, H * 0.36f, 1040f, 290f);
        CanvasGroup cg = pn.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        TMP_Text pt = UIKit.Label(pn, praise.ToUpper(), 0, 0, 1040, 200, praise.Length > 9 ? 96f : 124f,
                                  C(255, 226, 110), TextAlignmentOptions.Center, true);
        pt.characterSpacing = 4f;
        if (sub.Length > 0)
        {
            UIKit.Label(pn, sub, 0, 205, 1040, 70, 44, Color.white, TextAlignmentOptions.Center, true);
        }
        StartCoroutine(PraiseAnim(pn, cg));

        StartCoroutine(KillAfter(fxRoot.gameObject, 6f));
    }

    IEnumerator PraiseAnim(RectTransform rt, CanvasGroup g)
    {
        float e = 0f;
        const float pop = 0.45f;
        const float hold = 1.0f;
        const float fade = 0.35f;
        while (e < pop + hold + fade)
        {
            e += Time.unscaledDeltaTime;
            if (rt == null) yield break;
            float s = e < pop ? Mathf.Max(0.01f, EaseOutBack(e / pop)) : 1f;
            float f = e > pop + hold ? (e - pop - hold) / fade : 0f;
            rt.localScale = Vector3.one * s * (1f + f * 0.25f);
            g.alpha = Mathf.Clamp01(1f - f);
            yield return null;
        }
        if (rt != null) Destroy(rt.gameObject);
    }

    // কনফেটি প্রশংসার শব্দের জায়গা থেকে ফেটে চারদিকে ছড়ায়, তারপর নিচে ঝরে
    IEnumerator Confetti(RectTransform parent, int count, float cx, float cy)
    {
        Color[] pal =
        {
            C(255, 90, 110), C(255, 210, 70), C(90, 230, 140), C(90, 180, 255),
            C(200, 120, 255), C(255, 130, 210), C(255, 255, 255)
        };
        RectTransform[] rts = new RectTransform[count];
        float[] x = new float[count];
        float[] y = new float[count];
        float[] vx = new float[count];
        float[] vy = new float[count];
        float[] rot = new float[count];
        float[] spin = new float[count];
        float[] ph = new float[count];

        for (int i = 0; i < count; i++)
        {
            float w = Random.Range(14f, 24f);
            float h = Random.Range(8f, 14f);
            float a = Random.Range(0f, 6.28f);
            float sp = Random.Range(450f, 1200f);
            x[i] = cx;
            y[i] = cy;
            vx[i] = Mathf.Cos(a) * sp;
            vy[i] = Mathf.Sin(a) * sp - 350f;
            rot[i] = Random.Range(0f, 360f);
            spin[i] = Random.Range(-420f, 420f);
            ph[i] = Random.Range(0f, 6.28f);
            rts[i] = Node(parent, "Confetti", x[i], y[i], w, h);
            UIKit.Round(rts[i], "P", 0, 0, w, h, 3f, pal[i % pal.Length]);
        }

        const float life = 3.6f;
        float t = 0f;
        while (t < life)
        {
            float dt = Time.unscaledDeltaTime;
            t += dt;
            float shrink = Mathf.Clamp01((life - t) / 0.5f);
            for (int i = 0; i < count; i++)
            {
                if (rts[i] == null) continue;
                vx[i] -= vx[i] * 1.6f * dt;
                vy[i] = Mathf.Min(vy[i] + 700f * dt, 650f);
                y[i] += vy[i] * dt;
                x[i] += (vx[i] + Mathf.Sin(t * 5f + ph[i]) * 70f) * dt;
                rot[i] += spin[i] * dt;
                rts[i].anchoredPosition = new Vector2(x[i], -y[i]);
                rts[i].localRotation = Quaternion.Euler(0f, 0f, rot[i]);
                rts[i].localScale = Vector3.one * shrink;
            }
            yield return null;
        }
        for (int i = 0; i < count; i++)
        {
            if (rts[i] != null) Destroy(rts[i].gameObject);
        }
    }

    IEnumerator Fail(int id)
    {
        yield return new WaitForSeconds(0.55f);
        if (id != runId) yield break;
        Sfx("lose");
        ShowFail();
    }

    // ====================================================== HUD নতুন করা
    void RefreshHud()
    {
        int tier = Mathf.Clamp(level / 10, 0, 9);
        levelLabel.text = "LEVEL " + (Mathf.Min(level, 99) + 1);
        tierLabel.text = tierNames[tier];
        tierLabel.color = themes[tier].accent;
        coinsLabel.text = PlayerData.Coins.ToString();
        starsLabel.text = PlayerData.Stars.ToString();
        lvLabel.text = "<size=55%>LV</size>\n" + PlayerData.PlayerLevel;
        xpLabel.text = PlayerData.XpInLevel + " / 100 XP";
        SetBar(xpFill, 700f, 40f, PlayerData.XpInLevel / 100f);
        RefreshHearts();
    }

    void RefreshHearts()
    {
        for (int i = 0; i < heartImgs.Length; i++)
        {
            heartImgs[i].sprite = HeartSprite(i < hearts);
        }
    }

    void ToastMsg(string msg)
    {
        if (toast != null) Destroy(toast);
        RectTransform t = UIKit.Box(lower, "Toast", 190, -120, 700, 84);
        toast = t.gameObject;
        Neo.Surface(t, "Bg", 0, 0, 700, 84,
            St(42, C(40, 46, 110, 0.96f), C(24, 26, 70, 0.96f), C(255, 255, 255, 0.6f), C(255, 255, 255, 0.15f),
               2f, 0.3f, 0.5f, 24f, 10f, 1f));
        UIKit.Label(t, msg, 20, 0, 660, 84, 30, Color.white, TextAlignmentOptions.Center);
        StartCoroutine(KillAfter(toast, 1.6f));
    }

    // ====================================================== নড়াচড়া (Coroutine)
    IEnumerator KillAfter(GameObject g, float sec)
    {
        yield return new WaitForSecondsRealtime(sec);
        if (g != null) Destroy(g);
    }

    IEnumerator PopScale(RectTransform rt, float delay)
    {
        if (rt == null) yield break;
        rt.localScale = Vector3.zero;
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);
        float e = 0f;
        const float dur = 0.35f;
        while (e < dur)
        {
            e += Time.unscaledDeltaTime;
            if (rt == null) yield break;
            rt.localScale = Vector3.one * EaseOutBack(Mathf.Clamp01(e / dur));
            yield return null;
        }
        if (rt != null) rt.localScale = Vector3.one;
    }

    IEnumerator Punch(RectTransform rt, float from)
    {
        if (rt == null) yield break;
        float e = 0f;
        const float dur = 0.22f;
        while (e < dur)
        {
            e += Time.unscaledDeltaTime;
            if (rt == null) yield break;
            rt.localScale = Vector3.one * Mathf.Lerp(from, 1f, EaseOutCubic(Mathf.Clamp01(e / dur)));
            yield return null;
        }
        if (rt != null) rt.localScale = Vector3.one;
    }

    IEnumerator Shake(Tile t)
    {
        if (t.shaking) yield break;
        t.shaking = true;
        float e = 0f;
        const float dur = 0.32f;
        while (e < dur)
        {
            e += Time.unscaledDeltaTime;
            if (t.rt == null) yield break;
            float k = 1f - e / dur;
            t.rt.anchoredPosition = t.home + new Vector2(Mathf.Sin(e * 55f) * 16f * k, 0f);
            yield return null;
        }
        if (t.rt != null) t.rt.anchoredPosition = t.home;
        t.shaking = false;
    }

    // ভুল চাপলে লাল ঝলক। শেষে সবসময় সাদায় ফেরে, তাই দ্রুত দুবার চাপলেও লাল আটকে থাকে না।
    IEnumerator Tint(Image img)
    {
        if (img == null) yield break;
        img.color = new Color(1f, 0.45f, 0.45f);
        yield return new WaitForSecondsRealtime(0.3f);
        if (img != null) img.color = Color.white;
    }

    IEnumerator Fly(Tile t, int idx, bool last, int id)
    {
        RectTransform rt = t.rt;
        Vector2 from = rt.anchoredPosition;
        Vector2 to = slots[idx].node.anchoredPosition;
        float s1 = slotSize / tileSize;
        float e = 0f;
        const float dur = 0.22f;
        while (e < dur)
        {
            e += Time.unscaledDeltaTime;
            if (id != runId || rt == null) yield break;
            float k = EaseOutCubic(Mathf.Clamp01(e / dur));
            rt.anchoredPosition = Vector2.Lerp(from, to, k);
            rt.localScale = Vector3.one * Mathf.Lerp(1f, s1, k);
            yield return null;
        }
        if (id != runId || rt == null) yield break;
        Destroy(rt.gameObject);
        FillSlot(idx, t.ch);
        if (last) StartCoroutine(Complete(id));
    }

    IEnumerator AnimateBar(Image fill, float maxW, float h, float from, float to, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        float e = 0f;
        const float dur = 0.8f;
        while (e < dur)
        {
            e += Time.unscaledDeltaTime;
            if (fill == null) yield break;
            SetBar(fill, maxW, h, Mathf.Lerp(from, to, EaseOutCubic(Mathf.Clamp01(e / dur))));
            yield return null;
        }
        if (fill != null) SetBar(fill, maxW, h, to);
    }

    // পটভূমির সাজ নড়ানো
    void Update()
    {
        if (bgRoot == null || decos.Count == 0) return;
        float dt = Time.unscaledDeltaTime;
        float tm = Time.unscaledTime;
        float W = Mathf.Max(bgRoot.rect.width, 1080f);
        float H = Mathf.Max(bgRoot.rect.height, 1920f);

        for (int i = 0; i < decos.Count; i++)
        {
            Deco d = decos[i];
            if (d.rt == null) continue;
            d.x += d.vx * dt;
            d.y -= d.vy * dt;
            float m = d.size;
            if (d.y < -m) d.y = H + m;
            else if (d.y > H + m) d.y = -m;
            if (d.x < -m) d.x = W + m;
            else if (d.x > W + m) d.x = -m;

            float sx = d.sway > 0f ? Mathf.Sin(tm * d.swayF + d.phase) * d.sway : 0f;
            d.rt.anchoredPosition = new Vector2(d.x + sx, -d.y);

            if (d.spin != 0f)
            {
                d.rot += d.spin * dt;
                d.rt.localRotation = Quaternion.Euler(0f, 0f, d.rot);
            }
            if (d.img != null && d.twinkle > 0f)
            {
                float k = 1f - d.twinkle + d.twinkle * (0.5f + 0.5f * Mathf.Sin(tm * d.twinkleF + d.phase));
                Color c = d.baseCol;
                c.a = d.baseCol.a * k;
                d.img.color = c;
            }
        }
    }

    // ====================================================== পপআপ
    RectTransform OpenPopup(float w, float h, Color glow, out GameObject pr)
    {
        RectTransform rr = UIShapes.Stretch(root, "Popup");
        pr = rr.gameObject;
        Image dim = pr.AddComponent<Image>();
        dim.color = new Color(0.02f, 0.03f, 0.12f, 0.72f);
        dim.raycastTarget = true;

        GameObject wgo = new GameObject("Window", typeof(RectTransform));
        RectTransform win = wgo.GetComponent<RectTransform>();
        win.SetParent(rr, false);
        win.anchorMin = new Vector2(0.5f, 0.5f);
        win.anchorMax = new Vector2(0.5f, 0.5f);
        win.pivot = new Vector2(0.5f, 0.5f);
        win.sizeDelta = new Vector2(w, h);
        win.anchoredPosition = Vector2.zero;

        Image g = Neo.Glow(win, "Glow", w * 0.5f - 450f, h * 0.5f - 450f, 900f, Color.white, 0.5f);
        g.color = glow;
        Neo.Surface(win, "Glass", 0, 0, w, h,
            St(56, C(64, 74, 160, 0.95f), C(26, 28, 86, 0.97f), C(255, 255, 255, 0.5f), C(255, 255, 255, 0.12f),
               2.5f, 0.2f, 0.6f, 60f, 26f, 0.5f));
        StartCoroutine(PopScale(win, 0f));
        return win;
    }

    Color Accent()
    {
        return themes[Mathf.Clamp(level / 10, 0, 9)].accent;
    }

    // ---------- নতুন স্টেজের শুরু ----------
    void ShowIntro(int tier)
    {
        GameObject pr;
        Color ac = themes[tier].accent;
        RectTransform w = OpenPopup(860, 760, ac, out pr);

        TMP_Text st = UIKit.Label(w, "STAGE " + (tier + 1), 0, 90, 860, 60, 38, C(255, 255, 255, 0.8f), TextAlignmentOptions.Center);
        st.characterSpacing = 14f;
        TMP_Text nm = UIKit.Label(w, tierNames[tier], 0, 170, 860, 170, 100, ac, TextAlignmentOptions.Center);
        nm.characterSpacing = 6f;
        UIKit.Label(w, "Levels " + (tier * 10 + 1) + " - " + (tier * 10 + 10), 0, 360, 860, 50, 36, Color.white, TextAlignmentOptions.Center);
        UIKit.Label(w, (tier + 2) + "-letter words", 0, 415, 860, 44, 30, C(195, 205, 240), TextAlignmentOptions.Center);

        Button3D(w, "PLAY", 180, 520, 500, 140, C(110, 240, 140), C(30, 175, 90), Color.white, 56, () =>
        {
            AudioManager.PlayClick();
            Destroy(pr);
            introDone = true;
        });
    }

    // ---------- লেভেল সম্পন্ন ----------
    void ShowComplete(int stars, bool first, bool levelUp, int xpBefore, int xpAfter)
    {
        GameObject pr;
        RectTransform w = OpenPopup(860, 1160, Accent(), out pr);

        TMP_Text title = UIKit.Label(w, "LEVEL COMPLETE", 0, 70, 860, 80, 56, C(255, 226, 110), TextAlignmentOptions.Center);
        title.characterSpacing = 6f;
        UIKit.Label(w, target, 0, 150, 860, 90, 64, Color.white, TextAlignmentOptions.Center);

        // তারা
        float sz = 150f;
        float gap = 24f;
        float sx = (860f - (3 * sz + 2 * gap)) * 0.5f;
        for (int i = 0; i < 3; i++)
        {
            bool on = i < stars;
            RectTransform node = Node(w, "Star", sx + i * (sz + gap) + sz * 0.5f, 340f, sz, sz);
            Neo.Icon(node, 0, 0, sz, "star",
                     on ? C(255, 240, 150) : C(120, 126, 160), on ? C(240, 150, 20) : C(60, 64, 100));
            StartCoroutine(PopScale(node, 0.35f + i * 0.25f));
        }

        // পুরস্কার
        if (first)
        {
            Chip(w, "+" + (stars * 10) + " COINS", 130, 470, 280, 64, C(255, 226, 110), C(240, 140, 20), C(40, 28, 8), 30);
            Chip(w, "+20 XP", 450, 470, 280, 64, C(130, 200, 255), C(50, 120, 230), Color.white, 30);
        }
        else
        {
            UIKit.Label(w, "Replay - no new rewards", 0, 475, 860, 54, 30, C(195, 205, 240), TextAlignmentOptions.Center);
        }

        // XP বার
        int lv = PlayerData.PlayerLevel;
        UIKit.Label(w, "LV " + lv, 80, 570, 200, 50, 34, C(255, 214, 110), TextAlignmentOptions.Left);
        if (levelUp)
        {
            Chip(w, "LEVEL UP!", 330, 566, 200, 56, C(110, 240, 140), C(30, 175, 90), Color.white, 28);
        }
        UIKit.Label(w, xpAfter + " / 100 XP", 560, 570, 220, 50, 30, Color.white, TextAlignmentOptions.Right);
        Neo.Bar(w, "XpTrack", 80, 640, 700, 34, C(8, 12, 40, 0.9f), C(30, 36, 86, 0.9f), 0f, true);
        Image fill = Neo.Bar(w, "XpFill", 80, 640, 700, 34, C(255, 226, 110), C(240, 140, 20), 0.6f, false);
        float from = levelUp ? 0f : xpBefore / 100f;
        SetBar(fill, 700f, 34f, from);
        StartCoroutine(AnimateBar(fill, 700f, 34f, from, xpAfter / 100f, 0.5f));

        // বোতাম
        Button3D(w, "GRAMMAR LESSON", 80, 790, 700, 110, C(200, 125, 255), C(105, 55, 215), Color.white, 38, () =>
        {
            AudioManager.PlayClick();
            ShowGrammar(true);
        });
        Button3D(w, level >= 99 ? "FINISH" : "NEXT LEVEL", 80, 930, 700, 140, C(110, 240, 140), C(30, 175, 90), Color.white, 52, () =>
        {
            AudioManager.PlayClick();
            Destroy(pr);
            level++;
            StartCoroutine(RunLevel());
        });
    }

    // ---------- সব হার্ট শেষ ----------
    void ShowFail()
    {
        GameObject pr;
        RectTransform w = OpenPopup(860, 820, C(255, 80, 90), out pr);

        TMP_Text title = UIKit.Label(w, "OUT OF HEARTS", 0, 70, 860, 80, 56, C(255, 120, 130), TextAlignmentOptions.Center);
        title.characterSpacing = 6f;

        Image heart = UIKit.Box(w, "BigHeart", 330, 170, 200, 200).gameObject.AddComponent<Image>();
        heart.sprite = HeartSprite(false);
        heart.preserveAspect = true;
        heart.raycastTarget = false;

        UIKit.Label(w, "Don't give up! Try again or revive.", 0, 395, 860, 50, 30, C(195, 205, 240), TextAlignmentOptions.Center);

        Button3D(w, "TRY AGAIN", 80, 480, 700, 130, C(110, 240, 140), C(30, 175, 90), Color.white, 48, () =>
        {
            AudioManager.PlayClick();
            Destroy(pr);
            ResetPressed();
        });
        Button3D(w, "REVIVE  3 hearts  -" + ReviveCost, 80, 640, 700, 130, C(255, 220, 90), C(235, 130, 20), C(70, 38, 4), 38, () =>
        {
            AudioManager.PlayClick();
            if (PlayerData.Coins < ReviveCost)
            {
                ToastMsg("Not enough coins");
                return;
            }
            PlayerData.Coins -= ReviveCost;
            hearts = 3;
            RefreshHud();
            Destroy(pr);
            inputLocked = false;
        });
    }

    // ---------- ১০০ লেভেল শেষ ----------
    void ShowWin()
    {
        GameObject pr;
        RectTransform w = OpenPopup(860, 860, C(255, 218, 95), out pr);

        Neo.Icon(w, 340, 70, 180, "trophy", C(255, 240, 150), C(240, 150, 20));
        TMP_Text title = UIKit.Label(w, "YOU WIN!", 0, 270, 860, 110, 84, C(255, 226, 110), TextAlignmentOptions.Center);
        title.characterSpacing = 6f;
        UIKit.Label(w, "You finished all 100 levels.", 0, 390, 860, 54, 34, Color.white, TextAlignmentOptions.Center);

        Button3D(w, "PLAY AGAIN", 80, 500, 700, 130, C(110, 240, 140), C(30, 175, 90), Color.white, 46, () =>
        {
            AudioManager.PlayClick();
            Destroy(pr);
            PlayerData.SavedLevel = 0;
            level = 0;
            StartCoroutine(RunLevel());
        });
        Button3D(w, "HOME", 80, 660, 700, 130, C(120, 160, 255), C(50, 80, 200), Color.white, 46, () =>
        {
            AudioManager.PlayClick();
            SceneManager.LoadScene("MainMenu");
        });
    }

    // ---------- GRAMMAR LESSON ----------
    // reveal = false হলে (খেলার সময়) শব্দটা ফাঁকা থাকে, শুধু ধরন আর কাঠামো দেখা যায়। এটাই একটা ইঙ্গিত।
    // সেটিংসে যে ভাষা বাছা, তিন ভূমিকার ব্যাখ্যা, শব্দের অর্থ, বিপরীত শব্দ আর শব্দের নিচের ব্যাখ্যা সেই ভাষায় আসে।
    // উদাহরণ বাক্য সবসময় ইংরেজিতে থাকে।
    void ShowGrammar(bool reveal)
    {
        int lang = GrammarLang.Current;
        GrammarLang.EnsureFont(lang);

        GrammarLesson L = GrammarData.Get(level, target);
        GameObject pr;
        RectTransform w = OpenPopup(920, 1560, Accent(), out pr);
        UnityAction close = () =>
        {
            AudioManager.PlayClick();
            Destroy(pr);
        };

        // ওপরের চাকতি আর নাম
        Neo.Glow(w, "EmblemGlow", 280, -150, 360, C(255, 200, 70), 0.75f);
        Neo.Surface(w, "Emblem", 385, -75, 150, 150,
            St(75, C(255, 226, 120), C(240, 140, 20), C(255, 255, 255, 0.8f), C(255, 200, 80, 0.6f), 4f, 0.6f, 0.5f, 24f, 12f, 1f));
        UIKit.Label(w, "Aa", 385, -75, 150, 150, 70, C(110, 60, 10), TextAlignmentOptions.Center);
        TMP_Text title = UIKit.Label(w, "GRAMMAR LESSON", 0, 100, 920, 80, 58, Color.white, TextAlignmentOptions.Center);
        title.characterSpacing = 8f;

        Neo.Surface(w, "CloseBtn", 800, 36, 84, 84,
            St(42, C(255, 255, 255, 0.24f), C(255, 255, 255, 0.08f), C(255, 255, 255, 0.5f), C(255, 255, 255, 0.12f),
               2f, 0f, 0.35f, 16f, 8f, 1f));
        Neo.Icon(w, 819, 55, 46, "x", C(255, 255, 255), C(205, 215, 255));
        UIKit.Hit(w, 800, 36, 84, 84, close);

        // মূল কাঠামো: SUBJECT + VERB + OBJECT, নিচে প্রতিটার ব্যাখ্যা (বাছা ভাষায়)
        char[] roles = { 'S', 'V', 'O' };
        string[] defs = GrammarLang.RoleDefs(lang);
        string[] natives = GrammarLang.RoleNames(lang);
        for (int i = 0; i < 3; i++)
        {
            Color rc = GrammarData.RoleColor(roles[i]);
            float x = 35f + i * 300f;
            Neo.Surface(w, "Pill", x, 205, 250, 84,
                St(42, Neo.Mix(rc, 0.6f, 80f), Neo.Mix(rc, 0.6f, 0f), C(255, 255, 255, 0.7f), rc, 3f, 0.5f, 0.45f, 14f, 6f, 1f));
            UIKit.Label(w, GrammarData.RoleName(roles[i]), x, 205, 250, 84, 34, C(20, 24, 66), TextAlignmentOptions.Center);
            if (i < 2) UIKit.Label(w, "+", x + 250f, 205, 50, 84, 50, Color.white, TextAlignmentOptions.Center);

            string nat = natives[i].Length > 0 ? " (" + natives[i] + ")" : "";
            UIKit.Label(w, "<b><color=#" + GrammarData.Hex(roles[i]) + ">" + GrammarData.RoleName(roles[i]) + "</color></b>" + nat + "  " + defs[i],
                        60, 315 + i * 58, 820, 52, 29, C(225, 232, 255), TextAlignmentOptions.Left);
        }

        // শব্দটার সম্পর্কে: ধরন, শব্দ, অর্থ, বিপরীত শব্দ, ব্যাখ্যা
        Neo.Surface(w, "About", 35, 505, 850, 335,
            St(36, C(70, 80, 170, 0.55f), C(38, 40, 110, 0.55f), C(255, 255, 255, 0.3f), C(255, 255, 255, 0.08f),
               2.5f, 0.3f, 0.4f, 30f, 12f, 0.75f));
        Chip(w, L.type, 65, 530, 240, 54, C(255, 226, 110), C(240, 140, 20), C(40, 28, 8), 26);

        if (reveal)
        {
            string meaning = GrammarLang.Meaning(level, lang);
            string wordLine = L.word;
            if (meaning.Length > 0) wordLine += "  <size=60%><color=#FFD66E>— " + meaning + "</color></size>";
            TMP_Text wl = UIKit.Label(w, wordLine, 325, 520, 545, 76, 56, Color.white, TextAlignmentOptions.Left);
            wl.enableAutoSizing = true;
            wl.fontSizeMin = 26f;
            wl.fontSizeMax = 56f;

            float ny = 612f;
            string[] ant = GrammarLang.Antonym(L.word, lang);
            if (ant != null)
            {
                string line = "<color=#FFD66E>" + GrammarLang.OppositeLabel(lang) + "</color>  <b>" + ant[0] + "</b>";
                if (ant[1].Length > 0) line += "  <size=85%>— " + ant[1] + "</size>";
                UIKit.Label(w, line, 65, 606, 790, 54, 34, Color.white, TextAlignmentOptions.Left);
                ny = 672f;
            }
            UIKit.Label(w, GrammarLang.Note(level, L, lang), 65, ny, 790, 160, 30, C(215, 224, 250), TextAlignmentOptions.Left);
        }
        else
        {
            UIKit.Label(w, GrammarData.Blanks(L.word.Length), 325, 520, 545, 76, 56, Color.white, TextAlignmentOptions.Left);
            Neo.Icon(w, 65, 640, 56, "lock", C(225, 230, 255), C(150, 158, 205));
            UIKit.Label(w, GrammarLang.LockMsg(lang), 140, 620, 730, 120, 30, C(195, 205, 240), TextAlignmentOptions.Left);
        }

        // উদাহরণ বাক্য আর প্রতিটা অংশের ভূমিকা (সবসময় ইংরেজিতে)
        float ey = 865f;
        int n = L.segs.Length;
        float ch = 180f + n * 64f + 20f;
        Neo.Surface(w, "Example", 35, ey, 850, ch,
            St(36, C(70, 80, 170, 0.55f), C(38, 40, 110, 0.55f), C(255, 255, 255, 0.3f), C(255, 255, 255, 0.08f),
               2.5f, 0.3f, 0.4f, 30f, 12f, 0.75f));
        TMP_Text ex = UIKit.Label(w, "EXAMPLE", 65, ey + 18, 300, 40, 26, C(255, 214, 110), TextAlignmentOptions.Left);
        ex.characterSpacing = 6f;
        UIKit.Label(w, GrammarData.Sentence(L, reveal), 55, ey + 60, 810, 110, 46, Color.white, TextAlignmentOptions.Center);

        for (int i = 0; i < n; i++)
        {
            GrammarSeg sg = L.segs[i];
            Color rc = GrammarData.RoleColor(sg.tag);
            float ry = ey + 180f + i * 64f;
            Chip(w, GrammarData.RoleName(sg.tag), 65, ry, 200, 52, Neo.Mix(rc, 0.6f, 80f), Neo.Mix(rc, 0.6f, 0f), C(24, 28, 70), 24);
            UIKit.Label(w, GrammarData.SegText(sg.text, L.word, reveal, true), 290, ry, 560, 52, 34, Color.white, TextAlignmentOptions.Left);
        }

        Button3D(w, "GOT IT", 200, ey + ch + 30f, 520, 110, C(110, 240, 140), C(30, 175, 90), Color.white, 44, close);
    }
}