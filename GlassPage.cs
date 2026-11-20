using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// তিনটা পাতার মা-ক্লাস। AchievementPanel এর মতোই কাজ করে:
// বাটন নাম দিয়ে নিজে খুঁজে নেয়, জানালা আঁকে, খোলা-বন্ধের অ্যানিমেশন দেয়।
public abstract class GlassPage : MonoBehaviour
{
    class BarAnim
    {
        public RectTransform rt;
        public float maxW, frac, h;
    }

    protected Canvas canvas;
    protected GameObject root;
    protected RectTransform window;
    protected CanvasGroup group;
    protected readonly List<Image> pulses = new List<Image>();

    readonly List<BarAnim> bars = new List<BarAnim>();
    float openTime;
    bool built;
    GameObject toast;

    // বাটনের নামে এই শব্দ থাকলে সেটাই খোলার বাটন (ছোট হাতের)
    protected abstract string[] ButtonTokens { get; }
    protected abstract string PageTitle { get; }
    protected abstract string EmblemIcon { get; }   // Neo এর আইকন: star, medal, trophy, crown, flame, bolt, diamond
    protected abstract Color Accent { get; }
    protected abstract void BuildContent();
    protected virtual void OnOpened() { }

    // ---------- নিজে নিজে তৈরি ----------
    IEnumerator Start()
    {
        Button btn = GlassKit.FindIn<Button>(b => GlassKit.NameHas(b.name, ButtonTokens));
        if (btn == null)
        {
            Debug.LogWarning(GetType().Name + ": বাটন পাওয়া যায়নি (নামে " + string.Join("/", ButtonTokens) + " থাকতে হবে)");
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
        OnOpened();
    }

    protected void Close()
    {
        if (root != null) root.SetActive(false);
    }

    // ---------- ফ্রেম আঁকা ----------
    void Build()
    {
        built = true;
        bars.Clear();
        pulses.Clear();

        RectTransform rootRt = UIShapes.Stretch(canvas.rootCanvas.transform, name + "Root");
        root = rootRt.gameObject;
        Image back = root.AddComponent<Image>();
        back.sprite = Neo.VerticalGradient(Neo.C(12, 16, 52, 0.93f), Neo.C(44, 14, 92, 0.96f));
        back.raycastTarget = true;
        Button bb = root.AddComponent<Button>();
        bb.transition = Selectable.Transition.None;
        bb.targetGraphic = back;
        bb.onClick.AddListener(Close);
        group = root.AddComponent<CanvasGroup>();

        Neo.Glow(rootRt, "GlowA", -300, 120, 900, Accent, 0.55f);
        Neo.Glow(rootRt, "GlowB", 480, 1050, 900, Neo.C(40, 190, 255), 0.42f);

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

        Neo.Surface(window, "Glass", 0, 0, 900, 1580, GlassKit.WindowStyle());

        // ওপরের গোল চাকতি
        Neo.Glow(window, "EmblemGlow", 270, -150, 360, Neo.C(255, 200, 70), 0.75f);
        Neo.Surface(window, "Emblem", 375, -75, 150, 150,
            GlassKit.St(75, Neo.C(255, 226, 120), Neo.C(240, 140, 20), Neo.C(255, 255, 255, 0.8f),
                        Neo.C(255, 200, 80, 0.6f), 4f, 0.6f, 0.5f, 24f, 12f, 1f));
        Neo.Icon(window, 402, -52, 96, EmblemIcon, Neo.C(255, 255, 235), Neo.C(190, 110, 10));

        TMP_Text title = UIKit.Label(window, PageTitle, 0, 100, 900, 80, 62, Color.white, TextAlignmentOptions.Center);
        title.characterSpacing = 11f;

        // বন্ধ করার বোতাম
        Neo.Surface(window, "CloseBtn", 776, 36, 84, 84,
            GlassKit.St(42, Neo.C(255, 255, 255, 0.24f), Neo.C(255, 255, 255, 0.08f),
                        Neo.C(255, 255, 255, 0.5f), Neo.C(255, 255, 255, 0.12f), 2f, 0f, 0.35f, 16f, 8f, 1f));
        Neo.Icon(window, 795, 55, 46, "x", Neo.C(255, 255, 255), Neo.C(205, 215, 255));
        UIKit.Hit(window, 776, 36, 84, 84, Close);

        BuildContent();

        root.SetActive(false);
    }

    // ---------- সাহায্যকারী ----------
    // ওপরের সারাংশ: বাঁয়ে লেখা, ডানে লেখা, নিচে প্রগ্রেস বার (y = ২০৫ থেকে ২৯০)
    protected void Summary(string left, string right, float frac)
    {
        UIKit.Label(window, left, 60, 205, 500, 50, 34, Neo.C(255, 255, 255, 0.92f), TextAlignmentOptions.Left);
        UIKit.Label(window, right, 440, 205, 400, 50, 34, Neo.C(255, 214, 110), TextAlignmentOptions.Right);
        Neo.Bar(window, "SumTrack", 60, 262, 780, 26, Neo.C(8, 12, 40, 0.9f), Neo.C(30, 36, 86, 0.9f), 0f, true);
        Image fill = Neo.Bar(window, "SumFill", 60, 262, 780, 26, Neo.C(255, 226, 110), Neo.C(240, 140, 20), 0.6f, false);
        AddBar(fill, 780f, 26f, frac);
    }

    // খোলার সময় ভরে ওঠা বার
    protected void AddBar(Image fill, float maxW, float h, float frac)
    {
        BarAnim b = new BarAnim();
        b.rt = fill.rectTransform;
        b.maxW = maxW;
        b.h = h;
        b.frac = frac;
        bars.Add(b);
    }

    // নিচে ছোট বার্তা, ১.৬ সেকেন্ড পর মিলিয়ে যায়
    protected void Toast(string msg)
    {
        if (toast != null) Destroy(toast);
        RectTransform t = UIKit.Box(window, "Toast", 100, 1470, 700, 84);
        toast = t.gameObject;
        Neo.Surface(t, "Bg", 0, 0, 700, 84,
            GlassKit.St(42, Neo.C(40, 46, 110, 0.96f), Neo.C(24, 26, 70, 0.96f),
                        Neo.C(255, 255, 255, 0.6f), Neo.C(255, 255, 255, 0.15f), 2f, 0.3f, 0.5f, 24f, 10f, 1f));
        UIKit.Label(t, msg, 20, 0, 660, 84, 30, Color.white, TextAlignmentOptions.Center);
        StartCoroutine(HideToast(toast));
    }

    IEnumerator HideToast(GameObject g)
    {
        yield return new WaitForSecondsRealtime(1.6f);
        if (g != null) Destroy(g);
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
            if (b.rt == null) continue;
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
        for (int i = 0; i < pulses.Count; i++)
        {
            if (pulses[i] == null) continue;
            Color c = pulses[i].color;
            c.a = pulse;
            pulses[i].color = c;
        }
    }
}