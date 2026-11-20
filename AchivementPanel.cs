using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Achievement পাতা। MainMenu সিনে নিজে নিজেই তৈরি হয়, Achievement বাটন নিজেই খুঁজে নেয়।
// Inspector এ কিছু করতে হয় না। Neo, UIKit, UIShapes স্ক্রিপ্ট লাগে।
public class AchievementPanel : MonoBehaviour
{
    class Ach
    {
        public string title, desc, icon;
        public Color iconTop, iconBottom, accent;
        public int target;
        public bool streak;
    }

    class BarAnim
    {
        public RectTransform rt;
        public float maxW, frac, h;
    }

    static Ach A(string title, string desc, string icon, Color top, Color bottom, Color accent, int target, bool streak)
    {
        Ach a = new Ach();
        a.title = title;
        a.desc = desc;
        a.icon = icon;
        a.iconTop = top;
        a.iconBottom = bottom;
        a.accent = accent;
        a.target = target;
        a.streak = streak;
        return a;
    }

    // PlayerData.Achievements এর সাথে মিল: লেভেল ১০/২৫/৫০/১০০ আর টানা নির্ভুল ৫/১০/২০
    static readonly Ach[] Items =
    {
        A("Word Rookie", "Complete 10 levels", "star", Neo.C(255, 214, 120), Neo.C(236, 120, 40), Neo.C(255, 160, 70), 10, false),
        A("Word Explorer", "Complete 25 levels", "medal", Neo.C(120, 255, 230), Neo.C(20, 160, 170), Neo.C(60, 220, 200), 25, false),
        A("Word Master", "Complete 50 levels", "trophy", Neo.C(214, 170, 255), Neo.C(110, 60, 220), Neo.C(170, 110, 255), 50, false),
        A("Word Legend", "Complete 100 levels", "crown", Neo.C(255, 240, 150), Neo.C(240, 160, 20), Neo.C(255, 205, 80), 100, false),
        A("On Fire", "5 perfect levels in a row", "flame", Neo.C(255, 235, 110), Neo.C(240, 60, 30), Neo.C(255, 120, 60), 5, true),
        A("Unstoppable", "10 perfect levels in a row", "bolt", Neo.C(160, 235, 255), Neo.C(40, 110, 240), Neo.C(70, 170, 255), 10, true),
        A("Flawless", "20 perfect levels in a row", "diamond", Neo.C(255, 190, 240), Neo.C(210, 50, 160), Neo.C(255, 100, 190), 20, true),
    };

    Canvas canvas;
    GameObject root;
    CanvasGroup group;
    RectTransform window;
    readonly List<BarAnim> bars = new List<BarAnim>();
    readonly List<Image> glows = new List<Image>();
    float openTime;
    bool built;

    // ---------- নিজে নিজে তৈরি হওয়া ----------
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Spawn(SceneManager.GetActiveScene().name);
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Spawn(scene.name);
    }

    static void Spawn(string sceneName)
    {
        if (sceneName != "MainMenu") return;
        if (FindIn<AchievementPanel>(null) != null) return;
        new GameObject("AchievementPanel").AddComponent<AchievementPanel>();
    }

    static T FindIn<T>(System.Func<T, bool> pick) where T : Component
    {
        T[] all = Resources.FindObjectsOfTypeAll<T>();
        for (int i = 0; i < all.Length; i++)
        {
            T c = all[i];
            if (!c.gameObject.scene.IsValid()) continue;
            if (pick == null || pick(c)) return c;
        }
        return null;
    }

    IEnumerator Start()
    {
        Button btn = FindIn<Button>(b =>
        {
            string n = b.name.ToLowerInvariant();
            return n.Contains("achiv") || n.Contains("achie");
        });
        if (btn == null)
        {
            Debug.LogWarning("AchievementPanel: Achievement বাটন পাওয়া যায়নি (নামে achiv বা achie থাকতে হবে)");
            yield break;
        }
        canvas = btn.GetComponentInParent<Canvas>();
        if (canvas == null) yield break;
        btn.onClick.AddListener(Open);

        yield return new WaitForSeconds(0.6f);   // মেনু খোলার পর একটু থেমে ছবি আঁকে
        if (!built) Build();
    }

    void Open()
    {
        if (canvas == null) return;
        if (!built) Build();
        AudioManager.PlayClick();
        root.SetActive(true);
        root.transform.SetAsLastSibling();
        openTime = Time.unscaledTime;
        group.alpha = 0f;
    }

    void Close()
    {
        if (root != null) root.SetActive(false);
    }

    // ---------- আঁকা ----------
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

    void AddBar(RectTransform rt, float maxW, float h, float frac)
    {
        BarAnim b = new BarAnim();
        b.rt = rt;
        b.maxW = maxW;
        b.h = h;
        b.frac = frac;
        bars.Add(b);
    }

    void Build()
    {
        built = true;
        bars.Clear();
        glows.Clear();

        int completed = PlayerData.CompletedLevels;
        int bestStreak = PlayerData.BestStreak;

        RectTransform rootRt = UIShapes.Stretch(canvas.rootCanvas.transform, "AchievementRoot");
        root = rootRt.gameObject;
        Image back = root.AddComponent<Image>();
        back.sprite = Neo.VerticalGradient(Neo.C(12, 16, 52, 0.93f), Neo.C(44, 14, 92, 0.96f));
        back.raycastTarget = true;
        Button bb = root.AddComponent<Button>();
        bb.transition = Selectable.Transition.None;
        bb.targetGraphic = back;
        bb.onClick.AddListener(Close);
        group = root.AddComponent<CanvasGroup>();

        Neo.Glow(rootRt, "GlowA", -300, 120, 900, Neo.C(150, 90, 255), 0.55f);
        Neo.Glow(rootRt, "GlowB", 480, 1050, 900, Neo.C(40, 190, 255), 0.42f);

        // মূল জানালা (মাঝখানে)
        GameObject wgo = new GameObject("Window", typeof(RectTransform));
        window = wgo.GetComponent<RectTransform>();
        window.SetParent(rootRt, false);
        window.anchorMin = new Vector2(0.5f, 0.5f);
        window.anchorMax = new Vector2(0.5f, 0.5f);
        window.pivot = new Vector2(0.5f, 0.5f);
        window.sizeDelta = new Vector2(900f, 1580f);
        window.anchoredPosition = Vector2.zero;

        // জানালার ভেতরে চাপলে যেন পেছনের পর্দা বন্ধ না হয়
        RectTransform block = UIKit.Box(window, "Block", 0, 0, 900, 1580);
        Image bi = block.gameObject.AddComponent<Image>();
        bi.color = new Color(0f, 0f, 0f, 0f);
        bi.raycastTarget = true;

        Neo.Surface(window, "Glass", 0, 0, 900, 1580,
            St(56, Neo.C(255, 255, 255, 0.17f), Neo.C(255, 255, 255, 0.06f),
               Neo.C(255, 255, 255, 0.45f), Neo.C(255, 255, 255, 0.10f), 2.5f, 0f, 0.55f, 70f, 30f, 0.5f));

        // ওপরের সোনালি কাপের চাকতি
        Neo.Glow(window, "EmblemGlow", 270, -150, 360, Neo.C(255, 200, 70), 0.75f);
        Neo.Surface(window, "Emblem", 375, -75, 150, 150,
            St(75, Neo.C(255, 226, 120), Neo.C(240, 140, 20), Neo.C(255, 255, 255, 0.8f),
               Neo.C(255, 200, 80, 0.6f), 4f, 0.6f, 0.5f, 24f, 12f, 1f));
        Neo.Icon(window, 402, -52, 96, "trophy", Neo.C(255, 255, 235), Neo.C(190, 110, 10));

        TMP_Text title = UIKit.Label(window, "ACHIEVEMENTS", 0, 100, 900, 80, 62, Color.white, TextAlignmentOptions.Center);
        title.characterSpacing = 11f;

        // বন্ধ করার বোতাম
        Neo.Surface(window, "CloseBtn", 776, 36, 84, 84,
            St(42, Neo.C(255, 255, 255, 0.24f), Neo.C(255, 255, 255, 0.08f),
               Neo.C(255, 255, 255, 0.5f), Neo.C(255, 255, 255, 0.12f), 2f, 0f, 0.35f, 16f, 8f, 1f));
        Neo.Icon(window, 795, 55, 46, "x", Neo.C(255, 255, 255), Neo.C(205, 215, 255));
        UIKit.Hit(window, 776, 36, 84, 84, Close);

        // কয়টা পাওয়া হলো
        int unlocked = 0;
        for (int i = 0; i < Items.Length; i++)
        {
            int v = Items[i].streak ? bestStreak : completed;
            if (v >= Items[i].target) unlocked++;
        }
        int pct = Mathf.RoundToInt(100f * unlocked / Items.Length);

        UIKit.Label(window, unlocked + " / " + Items.Length + " Unlocked", 60, 205, 500, 50, 34,
                    Neo.C(255, 255, 255, 0.92f), TextAlignmentOptions.Left);
        UIKit.Label(window, pct + "%", 440, 205, 400, 50, 34, Neo.C(255, 214, 110), TextAlignmentOptions.Right);
        Neo.Bar(window, "SumTrack", 60, 262, 780, 26, Neo.C(8, 12, 40, 0.9f), Neo.C(30, 36, 86, 0.9f), 0f, true);
        Image sumFill = Neo.Bar(window, "SumFill", 60, 262, 780, 26, Neo.C(255, 226, 110), Neo.C(240, 140, 20), 0.6f, false);
        AddBar(sumFill.rectTransform, 780f, 26f, unlocked / (float)Items.Length);

        // ৭টা কার্ড
        for (int i = 0; i < Items.Length; i++)
        {
            BuildCard(i, 45f, 340f + i * 170f, completed, bestStreak);
        }

        root.SetActive(false);
    }

    void BuildCard(int index, float cx, float cy, int completed, int bestStreak)
    {
        Ach a = Items[index];
        int value = a.streak ? bestStreak : completed;
        bool done = value >= a.target;
        float cw = 810f;
        float ch = 152f;

        Color accentEdge = a.accent;
        if (done)
        {
            Image glow = Neo.Glow(window, "CardGlow", cx - 80, cy - 74, 300, a.accent, 0.40f);
            glows.Add(glow);
            Color e1 = accentEdge; e1.a = 0.95f;
            Color e2 = accentEdge; e2.a = 0.35f;
            Neo.Surface(window, "Card", cx, cy, cw, ch,
                St(36, Neo.C(70, 80, 170, 0.92f), Neo.C(38, 40, 110, 0.92f), e1, e2, 2.5f, 0.35f, 0.45f, 36f, 14f, 0.75f));
        }
        else
        {
            Neo.Surface(window, "Card", cx, cy, cw, ch,
                St(36, Neo.C(46, 52, 110, 0.72f), Neo.C(28, 30, 76, 0.72f),
                   Neo.C(255, 255, 255, 0.22f), Neo.C(255, 255, 255, 0.06f), 2.5f, 0.35f, 0.45f, 36f, 14f, 0.75f));
        }

        // গোল ব্যাজ আর আইকন
        float bx = cx + 26f;
        float by = cy + 26f;
        if (done)
        {
            Color ring = a.accent; ring.a = 1f;
            Neo.Surface(window, "Badge", bx, by, 100, 100,
                St(50, Neo.Mix(a.accent, 0.55f, 90f), Neo.Mix(a.accent, 0.55f, 0f),
                   Neo.C(255, 255, 255, 0.7f), ring, 3.5f, 0.5f, 0.5f, 16f, 8f, 1f));
            Neo.Icon(window, bx + 15, by + 12, 70, a.icon, a.iconTop, a.iconBottom);
        }
        else
        {
            Neo.Surface(window, "Badge", bx, by, 100, 100,
                St(50, Neo.C(70, 76, 120), Neo.C(34, 38, 72), Neo.C(255, 255, 255, 0.25f),
                   Neo.C(255, 255, 255, 0.05f), 3f, 0.2f, 0.4f, 14f, 6f, 1f));
            Neo.Icon(window, bx + 17, by + 15, 66, a.icon, Neo.C(150, 156, 190), Neo.C(88, 94, 130));
            Neo.Surface(window, "LockBadge", bx + 66, by + 66, 38, 38,
                St(19, Neo.C(60, 64, 100), Neo.C(26, 28, 56), Neo.C(255, 255, 255, 0.4f),
                   Neo.C(255, 255, 255, 0.1f), 2f, 0.1f, 0.5f, 8f, 4f, 1f));
            Neo.Icon(window, bx + 73, by + 72, 24, "lock", Neo.C(225, 230, 255), Neo.C(150, 158, 205));
        }

        // লেখা
        float textAlpha = done ? 1f : 0.78f;
        UIKit.Label(window, a.title, cx + 150, cy + 14, 460, 52, 38, Neo.C(255, 255, 255, textAlpha), TextAlignmentOptions.Left);
        UIKit.Label(window, a.desc, cx + 150, cy + 56, 460, 44, 25,
                    Neo.C(195, 205, 240, done ? 1f : 0.7f), TextAlignmentOptions.Left);

        // প্রগ্রেস বার
        float bw = 460f;
        Neo.Bar(window, "Track", cx + 150, cy + 104, bw, 18, Neo.C(8, 12, 40, 0.9f), Neo.C(30, 36, 86, 0.9f), 0f, true);
        float frac = Mathf.Min(1f, value / (float)a.target);
        Image fill = Neo.Bar(window, "Fill", cx + 150, cy + 104, bw, 18,
                             Neo.Mix(a.accent, 0.4f, 150f), a.accent, 0.6f, false);
        AddBar(fill.rectTransform, bw, 18f, frac);

        // ডানের অংশ
        if (done)
        {
            Neo.Surface(window, "DoneChip", cx + cw - 160, cy + 48, 132, 56,
                St(28, Neo.C(110, 240, 140), Neo.C(30, 175, 90), Neo.C(255, 255, 255, 0.7f),
                   Neo.C(80, 220, 120, 0.5f), 2.5f, 0.5f, 0.45f, 14f, 6f, 1f));
            Neo.Icon(window, cx + cw - 148, cy + 61, 30, "check", Neo.C(255, 255, 255), Neo.C(205, 255, 220));
            UIKit.Label(window, "DONE", cx + cw - 108, cy + 48, 100, 56, 24, Color.white, TextAlignmentOptions.Left);
        }
        else
        {
            UIKit.Label(window, value + "/" + a.target, cx + cw - 260, cy + 50, 220, 52, 36,
                        Neo.C(255, 255, 255, 0.9f), TextAlignmentOptions.Right);
        }
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

        float p = EaseOutCubic(Mathf.Clamp01((t - 0.25f) / 0.9f));
        for (int i = 0; i < bars.Count; i++)
        {
            BarAnim b = bars[i];
            float w = 0f;
            if (b.frac > 0f)
            {
                float target = Mathf.Max(b.h, b.maxW * b.frac);
                w = Mathf.Max(b.h * Mathf.Clamp01(p * 3f), target * p);
            }
            b.rt.sizeDelta = new Vector2(w, b.h);
            b.rt.gameObject.SetActive(w >= 1f);
        }

        float pulse = 0.7f + 0.3f * Mathf.Sin(Time.unscaledTime * 2.2f);
        for (int i = 0; i < glows.Count; i++)
        {
            Color c = glows[i].color;
            c.a = pulse;
            glows[i].color = c;
        }
    }
}