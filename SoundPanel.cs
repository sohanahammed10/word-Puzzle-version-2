using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Sound Settings মেনু (সাইবারপাংক ৩ডি)। HomeScreen আগের মতোই soundPanel.Open() ডাকে।
// Cyber, Neo, UIKit, UIShapes, AudioManager স্ক্রিপ্ট লাগে।
public class SoundPanel : MonoBehaviour
{
    Canvas canvas;
    GameObject root;
    CanvasGroup group;
    RectTransform window;
    Cyber.Toggle music;
    Cyber.Toggle sfx;
    Slider musicSlider;
    Slider sfxSlider;
    TMP_Text musicPct;
    TMP_Text sfxPct;
    float openTime;
    bool built;

    // ---------- খোলা আর বন্ধ ----------
    public void Open()
    {
        if (canvas == null) canvas = FindCanvas();
        if (canvas == null) return;
        if (!built) Build();
        Refresh();
        root.SetActive(true);
        root.transform.SetAsLastSibling();
        openTime = Time.unscaledTime;
        // নড়াচড়া (Update) শুধু তখনই চলে যখন এই স্ক্রিপ্টের অবজেক্ট চালু থাকে।
        // বন্ধ অবজেক্টের ভেতরে থাকলে নড়াচড়া ছাড়াই সরাসরি পুরোটা দেখায়।
        group.alpha = isActiveAndEnabled ? 0f : 1f;
        window.localScale = Vector3.one;
    }

    public void Close()
    {
        AudioManager.Flush();
        if (root != null) root.SetActive(false);
    }

    Canvas FindCanvas()
    {
        Canvas c = GetComponentInParent<Canvas>();
        if (c != null) return c.rootCanvas;
        Canvas[] all = Resources.FindObjectsOfTypeAll<Canvas>();
        for (int i = 0; i < all.Length; i++)
            if (all[i].gameObject.scene.IsValid() && all[i].isRootCanvas) return all[i];
        return null;
    }

    // ---------- আঁকা ----------
    void Build()
    {
        built = true;

        RectTransform rootRt = UIShapes.Stretch(canvas.transform, "SoundRoot");
        root = rootRt.gameObject;
        group = root.AddComponent<CanvasGroup>();

        Image back = Cyber.Backdrop(rootRt, "Back");
        Button bb = back.gameObject.AddComponent<Button>();
        bb.transition = Selectable.Transition.None;
        bb.targetGraphic = back;
        bb.onClick.AddListener(Close);

        Neo.Glow(rootRt, "GlowA", -340, 100, 900, Cyber.Magenta, 0.45f);
        Neo.Glow(rootRt, "GlowB", 500, 1100, 900, Cyber.Cyan, 0.38f);

        // মূল জানালা (মাঝখানে)
        GameObject wgo = new GameObject("Window", typeof(RectTransform));
        window = wgo.GetComponent<RectTransform>();
        window.SetParent(rootRt, false);
        window.anchorMin = new Vector2(0.5f, 0.5f);
        window.anchorMax = new Vector2(0.5f, 0.5f);
        window.pivot = new Vector2(0.5f, 0.5f);
        window.sizeDelta = new Vector2(900f, 1090f);
        window.anchoredPosition = Vector2.zero;

        // জানালার ভেতরে চাপলে যেন পেছনের পর্দা বন্ধ না হয়
        RectTransform block = UIKit.Box(window, "Block", 0, 0, 900, 1090);
        Image bi = block.gameObject.AddComponent<Image>();
        bi.color = new Color(0f, 0f, 0f, 0f);
        bi.raycastTarget = true;

        Cyber.Spec ws = Cyber.Make(60f, Cyber.C(26, 16, 62, 0.97f), Cyber.C(10, 6, 30, 0.97f),
                                   Cyber.Cyan, Cyber.Magenta, 3f, 0.7f, 14f, 0.04f, 0.6f);
        Cyber.Panel(window, "Frame", 0, 0, 900, 1090, ws);

        // নাম লেখা পাত
        Cyber.Spec ts = Cyber.Make(30f, Cyber.C(40, 14, 80, 0.97f), Cyber.C(16, 8, 44, 0.97f),
                                   Cyber.Magenta, Cyber.Cyan, 3f, 0.6f, 8f, 0.05f, 0.9f);
        Cyber.Panel(window, "Title", 100, 50, 640, 120, ts);
        Cyber.NeonLabel(window, "SOUND SETTINGS", 100, 50, 640, 120, 46, Cyber.Cyan, TextAlignmentOptions.Center, 5f);

        // বন্ধ করার বোতাম
        Cyber.Spec cs = Cyber.Make(20f, Cyber.C(60, 10, 60, 0.97f), Cyber.C(24, 6, 30, 0.97f),
                                   Cyber.Magenta, Cyber.Magenta, 3f, 0.7f, 6f, 0f, 1f);
        Cyber.Panel(window, "Close", 780, 38, 80, 80, cs);
        Neo.Icon(window, 798, 56, 44, "x", Cyber.C(255, 200, 240), Cyber.C(255, 40, 190));
        UIKit.Hit(window, 780, 38, 80, 80, Close);

        // Music আর Sound Effects
        music = BuildToggleRow(0, "MUSIC", "music", Cyber.Cyan, Cyber.C(200, 255, 255), Cyber.C(0, 170, 210));
        music.Changed = v => AudioManager.MusicOn = v;
        sfx = BuildToggleRow(1, "SOUND EFFECTS", "speaker", Cyber.Magenta, Cyber.C(255, 200, 240), Cyber.C(210, 20, 150));
        sfx.Changed = v => AudioManager.SfxOn = v;

        // ভলিউম স্লাইডার দুটো
        musicSlider = BuildVolumeRow(0, "MUSIC VOLUME", "music", Cyber.Green,
                                     Cyber.C(200, 255, 225), Cyber.C(0, 170, 90), out musicPct);
        musicSlider.onValueChanged.AddListener(v =>
        {
            AudioManager.MusicVolume = v;
            musicPct.text = Pct(v);
        });
        sfxSlider = BuildVolumeRow(1, "SOUND VOLUME", "speaker", Cyber.Yellow,
                                   Cyber.C(255, 245, 180), Cyber.C(240, 150, 20), out sfxPct);
        sfxSlider.onValueChanged.AddListener(v =>
        {
            AudioManager.SfxVolume = v;
            sfxPct.text = Pct(v);
        });

        // Default
        float sy = 515f + 2f * 190f + 10f;
        Cyber.Spec ds = Cyber.Make(32f, Cyber.C(60, 40, 150, 0.97f), Cyber.C(24, 14, 80, 0.97f),
                                   Cyber.Cyan, Cyber.Magenta, 3f, 0.75f, 10f, 0.04f, 0.9f);
        Cyber.Panel(window, "Default", 150, sy, 600, 112, ds);
        Neo.Icon(window, 220, sy + 30, 54, "refresh", Color.white, Cyber.C(200, 210, 255));
        Cyber.NeonLabel(window, "DEFAULT", 300, sy, 400, 112, 40, Cyber.Cyan, TextAlignmentOptions.Left, 3f);
        UIKit.Hit(window, 150, sy, 600, 112, RestoreDefault);

        root.SetActive(false);
    }

    Cyber.Toggle BuildToggleRow(int index, string name, string icon, Color neon, Color iconTop, Color iconBottom)
    {
        float cx = 45f;
        float cy = 215f + index * 150f;
        float cw = 810f;
        float ch = 124f;

        Cyber.Spec rs = Cyber.Make(26f, Cyber.C(30, 20, 70, 0.95f), Cyber.C(12, 8, 36, 0.95f),
                                   neon, neon, 2.5f, 0.35f, 8f, 0.05f, 0.75f);
        Cyber.Panel(window, "Row", cx, cy, cw, ch, rs);
        Cyber.Badge(window, cx + 24, cy + 20, 84, icon, neon, iconTop, iconBottom);
        Cyber.NeonLabel(window, name, cx + 134, cy, 420, ch, 36, neon, TextAlignmentOptions.Left);
        return new Cyber.Toggle(window, cx + cw - 200, cy + 24);
    }

    Slider BuildVolumeRow(int index, string name, string icon, Color neon, Color light, Color dark, out TMP_Text pct)
    {
        float cx = 45f;
        float cy = 515f + index * 190f;
        float cw = 810f;
        float ch = 170f;

        Cyber.Spec rs = Cyber.Make(26f, Cyber.C(30, 20, 70, 0.95f), Cyber.C(12, 8, 36, 0.95f),
                                   neon, neon, 2.5f, 0.35f, 8f, 0.05f, 0.75f);
        Cyber.Panel(window, "VolRow", cx, cy, cw, ch, rs);
        Cyber.Badge(window, cx + 24, cy + 20, 74, icon, neon, light, dark);
        Cyber.NeonLabel(window, name, cx + 120, cy + 16, 440, 80, 32, neon, TextAlignmentOptions.Left);

        Cyber.Spec ps = Cyber.Make(14f, Cyber.C(10, 8, 30, 0.97f), Cyber.C(20, 16, 50, 0.97f),
                                   neon, neon, 3f, 0.4f, 0f, 0f, 1f);
        Cyber.Panel(window, "PctBox", cx + cw - 190, cy + 28, 150, 56, ps);
        pct = Cyber.NeonLabel(window, "70%", cx + cw - 190, cy + 28, 150, 56, 32, neon, TextAlignmentOptions.Center);

        return Cyber.MakeSlider(window, cx + 44, cy + 100, cw - 100, neon, light, dark);
    }

    // ---------- কাজ ----------
    void Refresh()
    {
        music.Set(AudioManager.MusicOn);
        sfx.Set(AudioManager.SfxOn);
        musicSlider.SetValueWithoutNotify(AudioManager.MusicVolume);
        sfxSlider.SetValueWithoutNotify(AudioManager.SfxVolume);
        musicPct.text = Pct(AudioManager.MusicVolume);
        sfxPct.text = Pct(AudioManager.SfxVolume);
    }

    void RestoreDefault()
    {
        AudioManager.ResetSound();
        Refresh();
        AudioManager.PlayClick();
    }

    static string Pct(float v)
    {
        return Mathf.RoundToInt(v * 100f) + "%";
    }

    // ---------- নড়াচড়া ----------
    static float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        float u = t - 1f;
        return 1f + c3 * u * u * u + c1 * u * u;
    }

    void Update()
    {
        if (root == null || !root.activeSelf) return;
        float t = Time.unscaledTime - openTime;
        window.localScale = Vector3.one * Mathf.Lerp(0.92f, 1f, EaseOutBack(Mathf.Clamp01(t / 0.4f)));
        group.alpha = Mathf.Clamp01(t / 0.2f);
    }
}