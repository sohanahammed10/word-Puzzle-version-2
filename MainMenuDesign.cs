using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// MainMenu এর নতুন ৩D ডিজাইন। পুরোটা কোড দিয়ে আঁকা, কোনো আলাদা ছবি লাগে না, Inspector এ কিছু করতে হয় না।
// ওপরে ব্যাক বাটন আর দ্বীপের ছবি, তার নিচে একটাই PLAY বাটন (সেভ থাকলে CONTINUE),
// তারপর কাচের ৩D প্যানেলের ভেতরে RANKING বার আর ৬টা বোতাম।
// লাগে: Neo, UIKit, UIShapes, MenuArt, RankingPanel, PlayerData, AudioManager।
// পুরোনো Play, Continue, Back, Grid, Title ইত্যাদি নিজে নিজেই লুকিয়ে যায় (মোছা হয় না)।
public class MainMenuDesign : MonoBehaviour
{
    public string gameSceneName = "GameScene";
    public string homeSceneName = "SampleScene";
    public Sprite heroSprite;          // না দিলে সিনের পুরোনো "Background" ছবিটাই নেয়

    const float BottomH = 1136f;       // নিচের অংশের উচ্চতা (PLAY থেকে প্যানেলের তলা পর্যন্ত)

    RankingPanel ranking;
    QuizPanel quiz;
    RectTransform root;
    RectTransform top;
    RectTransform bottom;
    Image playGlow;
    GameObject toast;
    Sprite hero;

    // বোতামে চাপলে একটু ছোট হয়ে আবার ফিরে আসে
    public class PressScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        Vector3 target = Vector3.one;

        void Update()
        {
            transform.localScale = Vector3.Lerp(transform.localScale, target, 1f - Mathf.Exp(-20f * Time.unscaledDeltaTime));
        }

        public void OnPointerDown(PointerEventData e) { target = new Vector3(0.95f, 0.95f, 1f); }
        public void OnPointerUp(PointerEventData e) { target = Vector3.one; }
        public void OnPointerExit(PointerEventData e) { target = Vector3.one; }
    }

    // ====================================================== শুরু
    // Awake এ আঁকি, যাতে Word Map, Word Book, Challenge, Achievement পাতাগুলো (যারা Start এ বাটন খোঁজে) নতুন বাটন পেয়ে যায়।
    void Awake()
    {
        ranking = gameObject.AddComponent<RankingPanel>();   // Ranking পাতা নিজেই যোগ হয়
        quiz = gameObject.AddComponent<QuizPanel>();         // Quiz পাতাও নিজেই যোগ হয়

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("MainMenuDesign: সিনে Canvas পাওয়া যায়নি");
            return;
        }

        PrepareScene();
        BuildUi(canvas);
    }

    // পুরোনো বাটনের নাম বদলে দিই (যাতে পাতাগুলো ভুল করে ওগুলো না ধরে), আর পুরোনো অংশ লুকাই
    void PrepareScene()
    {
        string[] tokens = { "map", "book", "challenge", "achie", "achiv" };
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int n = 0;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (!buttons[i].gameObject.scene.IsValid()) continue;
            string nm = buttons[i].name.ToLowerInvariant();
            for (int k = 0; k < tokens.Length; k++)
            {
                if (nm.Contains(tokens[k]))
                {
                    buttons[i].gameObject.name = "OldBtn" + n;
                    n++;
                    break;
                }
            }
        }

        hero = heroSprite;
        RectTransform[] all = FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        string[] hide = { "Background", "Title", "Subtitle", "Topsub", "PlayMenuButton", "ContinueButton", "Grid", "BackButton" };
        for (int i = 0; i < all.Length; i++)
        {
            RectTransform r = all[i];
            if (!r.gameObject.scene.IsValid()) continue;
            for (int k = 0; k < hide.Length; k++)
            {
                if (r.name != hide[k]) continue;
                if (hero == null && r.name == "Background")
                {
                    Image im = r.GetComponent<Image>();
                    if (im != null) hero = im.sprite;
                }
                r.gameObject.SetActive(false);
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

    // কেন্দ্র ধরে বসানো ঘর
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

    // পর্দার ওপর বা নিচে (বা মাঝখানে) লাগানো ১০৮০ চওড়া ফ্রেম
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

    // চাপা যায় এমন বোতাম (নামটা গুরুত্বপূর্ণ: পাতাগুলো নাম দেখে বাটন খোঁজে)
    static RectTransform Btn(Transform parent, string name, float cx, float cy, float w, float h, UnityAction act)
    {
        RectTransform n = Node(parent, name, cx, cy, w, h);
        UIShapes.MakeButton(n, act);
        n.gameObject.AddComponent<PressScale>();
        return n;
    }

    static readonly UnityAction NoAction = () => { };

    // ====================================================== পুরো UI
    void BuildUi(Canvas canvas)
    {
        root = UIShapes.Stretch(canvas.rootCanvas.transform, "MenuRoot");
        Image bg = root.gameObject.AddComponent<Image>();
        bg.sprite = Neo.VerticalGradient(C(12, 18, 70), C(5, 9, 38));
        bg.raycastTarget = false;

        BuildHero();

        top = Frame(root, "Top", 0.5f, 1f, 1080f, 220f);
        bottom = Frame(root, "Bottom", 0.5f, 0f, 1080f, BottomH);

        BuildBack();
        BuildTitle();
        BuildPanel();
        BuildPlay();
        BuildRanking();
        BuildSix();
    }

    // ---------- দ্বীপের ছবি: পর্দার ওপর থেকে PLAY এর নিচ পর্যন্ত, নিচে মিলিয়ে যায় ----------
    void BuildHero()
    {
        RectTransform heroBox = UIShapes.Stretch(root, "Hero");
        heroBox.anchorMin = Vector2.zero;
        heroBox.anchorMax = Vector2.one;
        heroBox.offsetMin = new Vector2(0f, BottomH - 180f);
        heroBox.offsetMax = Vector2.zero;
        heroBox.gameObject.AddComponent<RectMask2D>();

        if (hero != null)
        {
            GameObject g = new GameObject("Art", typeof(RectTransform));
            RectTransform rt = g.GetComponent<RectTransform>();
            rt.SetParent(heroBox, false);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(1080f, 700f);
            Image im = g.AddComponent<Image>();
            im.sprite = hero;
            im.raycastTarget = false;
            AspectRatioFitter fit = g.AddComponent<AspectRatioFitter>();
            fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fit.aspectRatio = hero.rect.width / Mathf.Max(1f, hero.rect.height);
        }

        // নিচে নেভি রঙে মিলিয়ে যাওয়া
        GameObject f = new GameObject("Fade", typeof(RectTransform));
        RectTransform fr = f.GetComponent<RectTransform>();
        fr.SetParent(heroBox, false);
        fr.anchorMin = new Vector2(0f, 0f);
        fr.anchorMax = new Vector2(1f, 0f);
        fr.pivot = new Vector2(0.5f, 0f);
        fr.sizeDelta = new Vector2(0f, 420f);
        fr.anchoredPosition = Vector2.zero;
        Image fi = f.AddComponent<Image>();
        fi.sprite = Neo.VerticalGradient(C(6, 10, 42, 0f), C(6, 10, 42, 0.96f));
        fi.raycastTarget = false;
    }

    // ---------- ওপরের বেগুনি ব্যাক বাটন ----------
    void BuildBack()
    {
        RectTransform b = Btn(top, "BackBtn", 36f + 52f, 56f + 52f, 104f, 104f, GoBack);
        Neo.Surface(b, "Face", 0, 0, 104, 104,
            St(52, C(196, 120, 255), C(104, 32, 206), C(255, 255, 255, 0.85f), C(170, 100, 255, 0.7f), 4f, 0.55f, 0.55f, 18f, 8f, 1f));
        UIKit.Icon(b, 24, 24, 56, "back", Color.white);
    }

    // ---------- শিরোনাম (৩D) ----------
    void BuildTitle()
    {
        TMP_Text sub = UIKit.Label(bottom, "YOUR ADVENTURE BEGINS", 0, -190, 1080, 64, 46, C(255, 178, 40), TextAlignmentOptions.Center, true);
        sub.characterSpacing = 6f;

        Color[] depth = { C(26, 44, 120), C(34, 58, 150), C(44, 72, 184) };
        for (int i = 0; i < depth.Length; i++)
        {
            TMP_Text d = UIKit.Label(bottom, "WORD PUZZLE", 0, -146f + (depth.Length - i) * 3f + 4f, 1080, 150, 118, depth[i], TextAlignmentOptions.Center);
            d.characterSpacing = 3f;
        }
        TMP_Text t = UIKit.Label(bottom, "WORD PUZZLE", 0, -146, 1080, 150, 118, C(140, 170, 255), TextAlignmentOptions.Center, true);
        t.characterSpacing = 3f;
    }

    // ---------- কাচের ৩D প্যানেল (Ranking থেকে ৬টা বোতাম সব এর ভেতরে) ----------
    void BuildPanel()
    {
        Neo.Glow(bottom, "PanelGlow", -10, 85, 1100, C(110, 140, 255), 0.35f);
        Neo.Surface(bottom, "GlassPanel", 34, 202, 1012, 867,
            St(62, C(255, 255, 255, 0.20f), C(255, 255, 255, 0.07f), C(255, 255, 255, 0.80f), C(255, 255, 255, 0.18f),
               4f, 0.10f, 0.60f, 60f, 26f, 0.5f));
    }

    // ---------- একটাই PLAY / CONTINUE বাটন ----------
    void BuildPlay()
    {
        bool cont = PlayerData.SavedLevel > 0;
        Color dark = C(64, 36, 6);

        playGlow = Neo.Glow(bottom, "PlayGlow", 540f - 560f, 98f - 280f, 1120f, C(255, 200, 60), 0.35f);

        RectTransform b = Btn(bottom, "PlayButton", 540f, 16f + 82.5f, 840f, 165f, StartGame);
        Neo.Surface(b, "Face", 0, 0, 840, 165,
            St(82.5f, C(255, 238, 112), C(246, 172, 20), C(255, 222, 100), C(208, 126, 12), 7f, 0.60f, 0.55f, 30f, 14f, 0.75f));

        if (!cont)
        {
            UIKit.Icon(b, 222, 38, 88, "play", dark);
            UIKit.Label(b, "PLAY", 334, 0, 330, 165, 112, dark, TextAlignmentOptions.Left);
        }
        else
        {
            UIKit.Icon(b, 150, 30, 84, "play", dark);
            UIKit.Label(b, "CONTINUE", 262, 6, 520, 110, 86, dark, TextAlignmentOptions.Left);
            UIKit.Label(b, "LEVEL " + (PlayerData.SavedLevel + 1), 262, 108, 520, 44, 34, C(64, 36, 6, 0.8f), TextAlignmentOptions.Left);
        }
    }

    // ---------- সবুজ RANKING বার ----------
    void BuildRanking()
    {
        RankEntry me;
        RankingData.Build(out me);

        RectTransform b = Btn(bottom, "RankingButton", 540f, 224f + 69f, 898f, 138f, OpenRanking);
        Neo.Surface(b, "Face", 0, 0, 898, 138,
            St(69, C(104, 240, 134), C(24, 168, 80), C(200, 255, 214, 0.9f), C(40, 190, 100, 0.7f), 6f, 0.60f, 0.50f, 28f, 12f, 0.75f));

        // ফাঁপা তারা: সাদা তারার ওপর ছোট সবুজ তারা
        UIKit.Icon(b, 44, 34, 70, "star", Color.white);
        UIKit.Icon(b, 60, 50, 38, "star", C(34, 176, 84));
        UIKit.Label(b, "RANKING", 140, 0, 460, 138, 62, Color.white, TextAlignmentOptions.Left, true);
        UIKit.Label(b, "#" + me.rank, 560, 0, 230, 138, 64, Color.white, TextAlignmentOptions.Right, true);
        UIKit.Icon(b, 806, 45, 48, "chevron", Color.white);
    }

    // ---------- ৬টা বোতাম: Quiz, Battle / Challenge, Achievement / Word Book, Word Map ----------
    void BuildSix()
    {
        // নাম, লেখা, আইকন, ওপরের রং, নিচের রং, লেখার লাইন সংখ্যা
        string[] names = { "QuizButton", "BattleButton", "Challenge", "Achivment", "BookButton", "MapButton" };
        string[] labels = { "QUIZ", "MULTIPLAYER\nBATTLE", "CHALLENGE", "ACHIEVEMENT", "WORD BOOK", "WORD MAP" };
        string[] icons = { "quiz", "swords", "target", "trophy", "book", "map" };
        Color[] tops =
        {
            C(196, 98, 255), C(255, 108, 96), C(255, 176, 66), C(96, 232, 255), C(116, 238, 86), C(88, 176, 255)
        };
        Color[] bottoms =
        {
            C(112, 32, 204), C(214, 22, 34), C(236, 102, 12), C(22, 152, 216), C(34, 164, 34), C(22, 84, 224)
        };
        UnityAction[] actions =
        {
            QuizPressed, BattlePressed, NoAction, NoAction, NoAction, NoAction
        };

        for (int i = 0; i < 6; i++)
        {
            float cx = (i % 2 == 0) ? 306f : 774f;
            float cy = 482f + (i / 2) * 196f;
            RectTransform b = Btn(bottom, names[i], cx, cy, 452f, 172f, actions[i]);

            Neo.Surface(b, "Face", 0, 0, 452, 172,
                St(86, tops[i], bottoms[i], C(255, 230, 124), C(214, 142, 24), 7f, 0.55f, 0.55f, 26f, 12f, 0.75f));

            Image ic = UIKit.Box(b, "Icon", (452f - 124f) * 0.5f, -26f, 124, 124).gameObject.AddComponent<Image>();
            ic.sprite = MenuArt.Get(icons[i]);
            ic.preserveAspect = true;
            ic.raycastTarget = false;

            bool two = labels[i].Contains("\n");
            UIKit.Label(b, labels[i], 0, two ? 80f : 98f, 452, two ? 86f : 62f, two ? 36f : 44f, Color.white,
                        TextAlignmentOptions.Center, true);
        }
    }

    // ====================================================== কাজ
    void StartGame()
    {
        AudioManager.PlayClick();
        SceneManager.LoadScene(gameSceneName);   // সেভ না থাকলে ০ নম্বর থেকে, থাকলে সেভ করা লেভেল থেকে
    }

    void OpenRanking()
    {
        AudioManager.PlayClick();
        if (ranking != null) ranking.Open();
    }

    void GoBack()
    {
        AudioManager.PlayClick();
        SceneManager.LoadScene(homeSceneName);
    }

    void QuizPressed()
    {
        AudioManager.PlayClick();
        if (quiz != null) quiz.Open();
    }

    void BattlePressed()
    {
        AudioManager.PlayClick();
        ToastMsg("Multiplayer Battle is coming soon");
    }

    void ToastMsg(string msg)
    {
        if (toast != null) Destroy(toast);
        RectTransform host = Frame(root, "ToastHost", 0.5f, 0.5f, 760f, 100f);
        toast = host.gameObject;
        Neo.Surface(host, "Bg", 0, 0, 760, 100,
            St(50, C(44, 52, 130, 0.97f), C(24, 28, 80, 0.97f), C(255, 255, 255, 0.6f), C(255, 255, 255, 0.15f),
               2f, 0.3f, 0.5f, 26f, 10f, 1f));
        UIKit.Label(host, msg, 20, 0, 720, 100, 34, Color.white, TextAlignmentOptions.Center);
        StartCoroutine(KillAfter(toast, 1.6f));
    }

    IEnumerator KillAfter(GameObject g, float sec)
    {
        yield return new WaitForSecondsRealtime(sec);
        if (g != null) Destroy(g);
    }

    void Update()
    {
        if (playGlow == null) return;
        Color c = playGlow.color;
        c.a = 0.55f + 0.30f * Mathf.Sin(Time.unscaledTime * 2.4f);
        playGlow.color = c;
    }
}