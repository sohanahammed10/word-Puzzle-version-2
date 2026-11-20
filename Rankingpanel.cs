using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ---------- র‍্যাংকিং এর তথ্য ----------
// এখন ইন্টারনেটের সার্ভার নেই, তাই অন্য খেলোয়াড়দের নাম আর পয়েন্ট নমুনা (Demo)।
// তোমার নাম, অ্যাভাটার আর পয়েন্ট (Coins) আসল, পয়েন্ট অনুযায়ী ঠিক জায়গায় বসে যায়।
// পরে সার্ভার বানালে শুধু Build() এর ভেতরের নমুনা অংশ বদলাতে হবে।
public struct RankEntry
{
    public int rank;
    public string name;
    public int points;
    public bool me;
    public int avatar;
}

public static class RankingData
{
    static readonly string[] Names =
    {
        "Rafi", "Maria", "Hiroshi", "Fatima", "Carlos", "Anika", "Liam", "Nusrat", "Olivia", "Arjun",
        "Sofia", "Tanvir", "Emma", "Yuki", "Omar", "Priya", "Noah", "Lina", "Sakib", "Mia",
        "Ethan", "Zara", "Imran", "Chloe", "Ravi", "Lucas", "Nabila", "Jack", "Ayesha", "Leo"
    };

    public static int PlayerPoints
    {
        get { return PlayerData.Coins; }
    }

    // সব খেলোয়াড়ের তালিকা (১ নম্বর থেকে), me = তোমার নিজের সারি
    public static List<RankEntry> Build(out RankEntry me)
    {
        List<RankEntry> all = new List<RankEntry>();
        for (int i = 0; i < Names.Length; i++)
        {
            RankEntry bot = new RankEntry();
            bot.name = Names[i];
            bot.points = 6400 - i * 170 - (i * 53) % 41;
            bot.avatar = (i * 7 + 3) % AvatarArt.Count;
            all.Add(bot);
        }

        int pp = PlayerPoints;
        me = new RankEntry();
        me.name = PlayerData.Name;
        me.points = pp;
        me.me = true;
        me.avatar = PlayerData.AvatarIndex < 0 ? 0 : PlayerData.AvatarIndex;

        int pos = 0;
        while (pos < all.Count && all[pos].points >= pp) pos++;
        all.Insert(pos, me);

        for (int i = 0; i < all.Count; i++)
        {
            RankEntry e = all[i];
            e.rank = i + 1;
            all[i] = e;
            if (e.me) me = e;
        }
        return all;
    }
}

// ---------- র‍্যাংকিং পাতা ----------
// গ্লাস ৩D স্টাইল: ওপরে ৩ জনের পোডিয়াম, নিচে স্ক্রল করা তালিকা, সবার নিচে সবসময় "YOUR RANK" বার।
// MainMenuDesign নিজেই এটা যোগ করে নেয়। Ranking বাটন চাপলে Open() চলে।
// লাগে: Neo, UIKit, UIShapes, AvatarArt, PlayerData।
public class RankingPanel : MonoBehaviour
{
    class Reveal
    {
        public RectTransform rt;
        public CanvasGroup g;
        public float delay;
    }

    const float RowStep = 102f;
    const float ViewH = 640f;

    GameObject root;
    RectTransform window;
    CanvasGroup group;
    ScrollRect scroll;
    RectTransform content;
    float contentH;
    int myRank;
    float openTime;
    readonly List<Image> pulses = new List<Image>();
    readonly List<Reveal> reveals = new List<Reveal>();

    // ---------- রং ও আকার ----------
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

    // ১ম, ২য়, ৩য় এর ধাতুর রং
    static readonly Color[] MetalTop = { C(255, 232, 130), C(242, 246, 255), C(246, 184, 126) };
    static readonly Color[] MetalBottom = { C(236, 138, 22), C(128, 144, 190), C(152, 80, 30) };
    static readonly Color[] MetalRing = { C(255, 214, 90), C(215, 225, 245), C(232, 152, 92) };
    static readonly Color[] MetalText = { C(70, 38, 4), C(24, 32, 72), C(62, 28, 8) };

    // পোডিয়ামের মাপ: [১ম, ২য়, ৩য়]
    static readonly float[] PdX = { 340f, 80f, 600f };
    static readonly float[] PdH = { 190f, 150f, 130f };
    static readonly float[] PdD = { 150f, 122f, 122f };
    static readonly float[] PdDelay = { 0.60f, 0.45f, 0.30f };

    // ---------- খোলা আর বন্ধ ----------
    public void Open()
    {
        if (root != null) Destroy(root);   // প্রতিবার নতুন করে আঁকি, যাতে পয়েন্ট ঠিক থাকে
        Build();
        root.SetActive(true);
        root.transform.SetAsLastSibling();
        openTime = Time.unscaledTime;
        group.alpha = 0f;
        ScrollToMe();
    }

    public void Close()
    {
        if (root != null) root.SetActive(false);
    }

    // ---------- আঁকা ----------
    void Build()
    {
        pulses.Clear();
        reveals.Clear();

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();

        RectTransform rootRt = UIShapes.Stretch(canvas.rootCanvas.transform, "RankingRoot");
        root = rootRt.gameObject;
        Image back = root.AddComponent<Image>();
        back.sprite = Neo.VerticalGradient(C(12, 16, 52, 0.95f), C(44, 14, 92, 0.97f));
        back.raycastTarget = true;
        Button bb = root.AddComponent<Button>();
        bb.transition = Selectable.Transition.None;
        bb.targetGraphic = back;
        bb.onClick.AddListener(Close);
        group = root.AddComponent<CanvasGroup>();

        Neo.Glow(rootRt, "GlowA", -300, 120, 900, C(255, 190, 70), 0.50f);
        Neo.Glow(rootRt, "GlowB", 480, 1050, 900, C(150, 90, 255), 0.45f);

        GameObject wgo = new GameObject("Window", typeof(RectTransform));
        window = wgo.GetComponent<RectTransform>();
        window.SetParent(rootRt, false);
        window.anchorMin = new Vector2(0.5f, 0.5f);
        window.anchorMax = new Vector2(0.5f, 0.5f);
        window.pivot = new Vector2(0.5f, 0.5f);
        window.sizeDelta = new Vector2(900f, 1580f);
        window.anchoredPosition = Vector2.zero;

        // জানালার ভেতরে চাপলে পেছনের পর্দা বন্ধ হবে না
        RectTransform block = UIKit.Box(window, "Block", 0, 0, 900, 1580);
        Image bi = block.gameObject.AddComponent<Image>();
        bi.color = new Color(0f, 0f, 0f, 0f);
        bi.raycastTarget = true;

        Neo.Surface(window, "Glass", 0, 0, 900, 1580,
            St(56, C(255, 255, 255, 0.17f), C(255, 255, 255, 0.06f), C(255, 255, 255, 0.45f), C(255, 255, 255, 0.10f),
               2.5f, 0f, 0.55f, 70f, 30f, 0.5f));

        // ওপরের সোনালি কাপ আর নাম
        Neo.Glow(window, "EmblemGlow", 270, -150, 360, C(255, 200, 70), 0.75f);
        Neo.Surface(window, "Emblem", 375, -75, 150, 150,
            St(75, C(255, 226, 120), C(240, 140, 20), C(255, 255, 255, 0.8f), C(255, 200, 80, 0.6f), 4f, 0.6f, 0.5f, 24f, 12f, 1f));
        Neo.Icon(window, 402, -52, 96, "trophy", C(255, 255, 235), C(190, 110, 10));
        TMP_Text title = UIKit.Label(window, "RANKING", 0, 100, 900, 80, 62, Color.white, TextAlignmentOptions.Center);
        title.characterSpacing = 11f;

        // বন্ধ করার বোতাম
        Neo.Surface(window, "CloseBtn", 776, 36, 84, 84,
            St(42, C(255, 255, 255, 0.24f), C(255, 255, 255, 0.08f), C(255, 255, 255, 0.5f), C(255, 255, 255, 0.12f),
               2f, 0f, 0.35f, 16f, 8f, 1f));
        Neo.Icon(window, 795, 55, 46, "x", C(255, 255, 255), C(205, 215, 255));
        UIKit.Hit(window, 776, 36, 84, 84, Close);

        // তথ্য
        RankEntry me;
        List<RankEntry> all = RankingData.Build(out me);
        myRank = me.rank;

        TMP_Text sub = UIKit.Label(window, "GLOBAL LEADERBOARD", 60, 190, 500, 44, 26, C(255, 255, 255, 0.75f), TextAlignmentOptions.Left);
        sub.characterSpacing = 6f;
        Chip(window, "DEMO", 700, 190, 140, 44, C(190, 130, 255), C(110, 60, 220), Color.white, 22);

        // পোডিয়াম (৩য় আগে, ১ম সবার শেষে এসে দাঁড়ায়)
        for (int p = 2; p >= 0; p--)
        {
            if (p < all.Count) BuildPodium(p, all[p]);
        }

        // তালিকার মাথার লেখা
        TMP_Text h1 = UIKit.Label(window, "PLAYER", 60, 708, 300, 34, 22, C(190, 200, 245, 0.75f), TextAlignmentOptions.Left);
        h1.characterSpacing = 5f;
        TMP_Text h2 = UIKit.Label(window, "COINS", 560, 708, 280, 34, 22, C(190, 200, 245, 0.75f), TextAlignmentOptions.Right);
        h2.characterSpacing = 5f;

        // ৪ নম্বর থেকে নিচের সারি
        int rows = Mathf.Max(0, all.Count - 3);
        contentH = rows * RowStep + 20f;
        scroll = MakeScroll(window, 35, 748, 830, ViewH, contentH, out content);
        for (int i = 0; i < rows; i++)
        {
            BuildRow(i, all[i + 3]);
        }

        BuildMeBar(me);

        TMP_Text foot = UIKit.Label(window, "Demo list - online players will come later", 45, 1538, 810, 30, 22,
                                    C(170, 205, 250), TextAlignmentOptions.Center);
        foot.color = C(170, 205, 250, 0.8f);

        root.SetActive(false);
    }

    // ---------- সাহায্যকারী আকার ----------
    static void Chip(Transform p, string text, float x, float y, float w, float h,
                     Color top, Color bottom, Color textColor, float size)
    {
        Neo.Surface(p, "Chip", x, y, w, h,
            St(h * 0.5f, top, bottom, C(255, 255, 255, 0.7f), C(255, 255, 255, 0.15f), 2.5f, 0.5f, 0.45f, 14f, 6f, 1f));
        UIKit.Label(p, text, x, y, w, h, size, textColor, TextAlignmentOptions.Center);
    }

    static void Coin(Transform p, float x, float y, float d)
    {
        UIKit.Round(p, "CoinOuter", x, y, d, d, d * 0.5f, C(120, 72, 10));
        UIKit.Round(p, "CoinInner", x + d * 0.1f, y + d * 0.1f, d * 0.8f, d * 0.8f, d * 0.4f, C(255, 206, 60));
        UIKit.Round(p, "CoinShine", x + d * 0.28f, y + d * 0.22f, d * 0.22f, d * 0.22f, d * 0.11f, C(255, 245, 190, 0.8f));
    }

    static TMP_Text NameLabel(Transform p, string text, float x, float y, float w, float h, float size,
                              Color col, TextAlignmentOptions align, bool outline)
    {
        TMP_Text t = UIKit.Label(p, text, x, y, w, h, size, col, align, outline);
        t.enableAutoSizing = true;
        t.fontSizeMin = 18f;
        t.fontSizeMax = size;
        return t;
    }

    static string Pts(int v)
    {
        return v.ToString("#,0", System.Globalization.CultureInfo.InvariantCulture);
    }

    static Image Avatar(Transform p, float x, float y, float d, int index)
    {
        Image im = UIKit.Box(p, "Avatar", x, y, d, d).gameObject.AddComponent<Image>();
        im.sprite = AvatarArt.Get(index);
        im.preserveAspect = true;
        im.raycastTarget = false;
        return im;
    }

    // x, y = বাম-উপর কোণ থেকে দূরত্ব
    static ScrollRect MakeScroll(Transform parent, float x, float y, float w, float h, float contentHeight, out RectTransform cont)
    {
        RectTransform view = UIKit.Box(parent, "ScrollView", x, y, w, h);
        Image hit = view.gameObject.AddComponent<Image>();
        hit.color = new Color(0f, 0f, 0f, 0f);
        hit.raycastTarget = true;
        view.gameObject.AddComponent<RectMask2D>();

        cont = UIKit.Box(view, "Content", 0, 0, w, contentHeight);

        ScrollRect sr = view.gameObject.AddComponent<ScrollRect>();
        sr.content = cont;
        sr.viewport = view;
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Elastic;
        sr.scrollSensitivity = 40f;
        sr.decelerationRate = 0.135f;
        return sr;
    }

    // ---------- পোডিয়াম: p = 0 (১ম), 1 (২য়), 2 (৩য়) ----------
    void BuildPodium(int p, RankEntry e)
    {
        // একটা কলামের সব জিনিস এক গ্রুপে, যাতে নিচ থেকে উঠে এসে ফুটে ওঠে
        RectTransform col = UIKit.Box(window, "Podium" + (p + 1), 0, 0, 900, 1580);
        CanvasGroup cg = col.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        Reveal rv = new Reveal();
        rv.rt = col;
        rv.g = cg;
        rv.delay = PdDelay[p];
        reveals.Add(rv);

        float bx = PdX[p];
        float bw = 220f;
        float bh = PdH[p];
        float top = 700f - bh;
        float d = PdD[p];
        float cx = bx + bw * 0.5f;
        float ay = top - d + 30f;
        Color ring = MetalRing[p];
        Color tcol = MetalText[p];

        // পোডিয়ামের ব্লক
        Neo.Surface(col, "Block", bx, top, bw, bh,
            St(34, MetalTop[p], MetalBottom[p], C(255, 255, 255, 0.8f), ring, 3f, 0.55f, 0.6f, 40f, 16f, 0.75f));

        // অ্যাভাটার: আলো, পাড়, ছবি
        if (p == 0)
        {
            Image glow = Neo.Glow(col, "AvGlow", cx - d * 1.1f, ay + d * 0.5f - d * 1.1f, d * 2.2f, ring, 0.75f);
            pulses.Add(glow);
        }
        UIKit.Round(col, "AvShadow", cx - d * 0.5f - 7f, ay - 1f, d + 14f, d + 14f, (d + 14f) * 0.5f, C(0, 0, 0, 0.3f));
        UIKit.Round(col, "AvRing", cx - d * 0.5f - 7f, ay - 7f, d + 14f, d + 14f, (d + 14f) * 0.5f, ring);
        Avatar(col, cx - d * 0.5f, ay, d, e.avatar);

        // ১ম জনের মাথায় মুকুট
        if (p == 0) Neo.Icon(col, cx - 48f, ay - 82f, 96f, "crown", C(255, 240, 160), C(240, 160, 30));

        // ক্রমের ব্যাজ
        Neo.Surface(col, "Badge", cx - 26f, ay + d - 30f, 52, 52,
            St(26, MetalTop[p], MetalBottom[p], C(255, 255, 255, 0.9f), ring, 3f, 0.5f, 0.5f, 10f, 5f, 1f));
        UIKit.Label(col, (p + 1).ToString(), cx - 26f, ay + d - 30f, 52, 52, 32, tcol, TextAlignmentOptions.Center);
        if (e.me) Chip(col, "YOU", cx + 36f, ay + d - 24f, 70, 36, C(110, 240, 140), C(30, 175, 90), Color.white, 20);

        // নাম আর পয়েন্ট
        NameLabel(col, e.name, bx + 10f, top + 38f, bw - 20f, 40f, 30, tcol, TextAlignmentOptions.Center, false);
        float py = top + 84f;
        Coin(col, bx + 32f, py + 5f, 30f);
        UIKit.Label(col, Pts(e.points), bx + 70f, py, bw - 84f, 40, 32, tcol, TextAlignmentOptions.Left);
    }

    // ---------- ৪ নম্বর থেকে নিচের সারি ----------
    void BuildRow(int idx, RankEntry e)
    {
        float x = 10f;
        float y = 10f + idx * RowStep;
        float w = 810f;
        float h = 88f;
        bool me = e.me;

        if (me)
        {
            Image glow = Neo.Glow(content, "MeGlow", x + w * 0.5f - 200f, y + h * 0.5f - 200f, 400f, C(80, 230, 140), 0.5f);
            pulses.Add(glow);
            Neo.Surface(content, "Row", x, y, w, h,
                St(34, C(70, 190, 120, 0.88f), C(24, 110, 66, 0.9f), C(170, 255, 200, 0.95f), C(80, 220, 130, 0.4f),
                   3f, 0.4f, 0.5f, 30f, 12f, 0.75f));
        }
        else
        {
            Neo.Surface(content, "Row", x, y, w, h,
                St(34, C(255, 255, 255, 0.14f), C(255, 255, 255, 0.05f), C(255, 255, 255, 0.35f), C(255, 255, 255, 0.08f),
                   2.5f, 0.3f, 0.4f, 28f, 12f, 0.75f));
        }

        // ক্রমের গোল ব্যাজ
        Color a = me ? C(255, 232, 130) : C(84, 94, 190, 0.95f);
        Color b = me ? C(236, 138, 22) : C(40, 44, 118, 0.95f);
        Neo.Surface(content, "RankBadge", x + 18f, y + 14f, 60, 60,
            St(30, a, b, C(255, 255, 255, me ? 0.85f : 0.5f), C(255, 255, 255, 0.1f), 2.5f, 0.4f, 0.4f, 10f, 5f, 1f));
        float fs = e.rank < 10 ? 34f : (e.rank < 100 ? 30f : 22f);
        UIKit.Label(content, e.rank.ToString(), x + 18f, y + 14f, 60, 60, fs,
                    me ? C(70, 38, 4) : Color.white, TextAlignmentOptions.Center);

        // অ্যাভাটার
        UIKit.Round(content, "AvRing", x + 94f, y + 10f, 68, 68, 34, C(255, 255, 255, me ? 0.9f : 0.5f));
        Avatar(content, x + 98f, y + 14f, 60f, e.avatar);

        // নাম আর পয়েন্ট
        NameLabel(content, me ? e.name + " (You)" : e.name, x + 180f, y, 340f, h, 34, Color.white, TextAlignmentOptions.Left, me);
        Coin(content, x + w - 62f, y + h * 0.5f - 15f, 30f);
        UIKit.Label(content, Pts(e.points), x + w - 72f - 220f, y, 220, h, 38, C(255, 226, 110), TextAlignmentOptions.Right, me);
    }

    // ---------- নিচের স্থায়ী "YOUR RANK" বার ----------
    void BuildMeBar(RankEntry me)
    {
        float x = 35f, y = 1410f, w = 830f, h = 112f;

        Image glow = Neo.Glow(window, "MeBarGlow", x + w * 0.5f - 350f, y + h * 0.5f - 350f, 700f, C(80, 230, 140), 0.45f);
        pulses.Add(glow);
        Neo.Surface(window, "MeBar", x, y, w, h,
            St(40, C(70, 190, 120, 0.9f), C(24, 110, 66, 0.92f), C(170, 255, 200, 0.95f), C(80, 220, 130, 0.4f),
               3f, 0.45f, 0.55f, 34f, 14f, 0.75f));

        TMP_Text lab = UIKit.Label(window, "YOUR RANK", x + 30f, y + 12f, 220, 28, 20, C(214, 255, 228, 0.9f), TextAlignmentOptions.Left);
        lab.characterSpacing = 4f;
        UIKit.Label(window, "#" + me.rank, x + 30f, y + 38f, 200, 64, 56, Color.white, TextAlignmentOptions.Left, true);

        UIKit.Round(window, "MeAvRing", x + 226f, y + 12f, 88, 88, 44, C(255, 255, 255, 0.9f));
        Avatar(window, x + 230f, y + 16f, 80f, me.avatar);

        NameLabel(window, me.name, x + 332f, y, 290f, h, 36, Color.white, TextAlignmentOptions.Left, true);
        Coin(window, x + w - 62f, y + h * 0.5f - 15f, 30f);
        UIKit.Label(window, Pts(me.points), x + w - 72f - 220f, y, 220, h, 40, C(255, 236, 140), TextAlignmentOptions.Right, true);

        // চাপলে তালিকায় নিজের সারিতে চলে যায়
        UIKit.Hit(window, x, y, w, h, () =>
        {
            AudioManager.PlayClick();
            ScrollToMe();
        });
    }

    void ScrollToMe()
    {
        if (scroll == null || content == null) return;
        float max = Mathf.Max(0f, contentH - ViewH);
        float target = myRank > 3 ? Mathf.Clamp((myRank - 4) * RowStep - 200f, 0f, max) : 0f;
        content.anchoredPosition = new Vector2(0f, target);
        scroll.velocity = Vector2.zero;
    }

    // ---------- নড়াচড়া ----------
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

    void Update()
    {
        if (root == null || !root.activeSelf) return;

        float t = Time.unscaledTime - openTime;
        window.localScale = Vector3.one * Mathf.Lerp(0.9f, 1f, EaseOutBack(Mathf.Clamp01(t / 0.45f)));
        group.alpha = Mathf.Clamp01(t / 0.25f);

        for (int i = 0; i < reveals.Count; i++)
        {
            Reveal r = reveals[i];
            if (r.rt == null) continue;
            float k = EaseOutCubic(Mathf.Clamp01((t - r.delay) / 0.5f));
            r.g.alpha = k;
            r.rt.anchoredPosition = new Vector2(0f, -70f * (1f - k));
        }

        float pulse = 0.7f + 0.3f * Mathf.Sin(Time.unscaledTime * 2.2f);
        for (int i = 0; i < pulses.Count; i++)
        {
            if (pulses[i] == null) continue;
            Color c = pulses[i].color;
            c.a = pulse;
            pulses[i].color = c;
        }
    }
}