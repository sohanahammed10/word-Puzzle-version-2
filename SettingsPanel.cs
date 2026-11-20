using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Settings মেনু (সাইবারপাংক ৩ডি)। HomeScreen আগের মতোই settingsPanel.Open() ডাকে।
// Cyber, Neo, UIKit, UIShapes, AudioManager স্ক্রিপ্ট লাগে।
public class SettingsPanel : MonoBehaviour
{
    public string supportUrl = "";    // Help & Support চাপলে যা খুলবে, যেমন mailto:তোমার@gmail.com

    // প্রথমটা ডিফল্ট। (নাম ইংরেজিতে, কারণ TMP এর সাধারণ ফন্টে বাংলা অক্ষর আসে না)
    public string[] languages = { "English", "Bangla", "Hindi", "Arabic", "Spanish", "French" };

    Canvas canvas;
    GameObject root;
    CanvasGroup group;
    RectTransform window;
    Cyber.Toggle sound;
    Cyber.Toggle vibration;
    TMP_Text languageText;
    GameObject popup;
    TMP_Text[] items;
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
        if (popup != null) popup.SetActive(false);
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

        RectTransform rootRt = UIShapes.Stretch(canvas.transform, "SettingsRoot");
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
        window.sizeDelta = new Vector2(900f, 1150f);
        window.anchoredPosition = Vector2.zero;

        // জানালার ভেতরে চাপলে যেন পেছনের পর্দা বন্ধ না হয়
        RectTransform block = UIKit.Box(window, "Block", 0, 0, 900, 1150);
        Image bi = block.gameObject.AddComponent<Image>();
        bi.color = new Color(0f, 0f, 0f, 0f);
        bi.raycastTarget = true;

        Cyber.Spec ws = Cyber.Make(60f, Cyber.C(26, 16, 62, 0.97f), Cyber.C(10, 6, 30, 0.97f),
                                   Cyber.Cyan, Cyber.Magenta, 3f, 0.7f, 14f, 0.04f, 0.6f);
        Cyber.Panel(window, "Frame", 0, 0, 900, 1150, ws);

        // নাম লেখা পাত
        Cyber.Spec ts = Cyber.Make(30f, Cyber.C(40, 14, 80, 0.97f), Cyber.C(16, 8, 44, 0.97f),
                                   Cyber.Magenta, Cyber.Cyan, 3f, 0.6f, 8f, 0.05f, 0.9f);
        Cyber.Panel(window, "Title", 170, 50, 560, 120, ts);
        Cyber.NeonLabel(window, "SETTINGS", 170, 50, 560, 120, 60, Cyber.Cyan, TextAlignmentOptions.Center, 8f);

        // বন্ধ করার বোতাম
        Cyber.Spec cs = Cyber.Make(20f, Cyber.C(60, 10, 60, 0.97f), Cyber.C(24, 6, 30, 0.97f),
                                   Cyber.Magenta, Cyber.Magenta, 3f, 0.7f, 6f, 0f, 1f);
        Cyber.Panel(window, "Close", 780, 38, 80, 80, cs);
        Neo.Icon(window, 798, 56, 44, "x", Cyber.C(255, 200, 240), Cyber.C(255, 40, 190));
        UIKit.Hit(window, 780, 38, 80, 80, Close);

        BuildRow(0, "SOUND", "speaker", Cyber.Cyan, Cyber.C(200, 255, 255), Cyber.C(0, 170, 210));
        BuildRow(1, "VIBRATION", "phone", Cyber.Magenta, Cyber.C(255, 200, 240), Cyber.C(210, 20, 150));
        BuildRow(2, "LANGUAGE", "globe", Cyber.Violet, Cyber.C(220, 200, 255), Cyber.C(110, 60, 230));
        BuildRow(3, "HELP & SUPPORT", "headset", Cyber.Yellow, Cyber.C(255, 245, 180), Cyber.C(240, 160, 20));
        BuildRow(4, "RESET SETTINGS", "refresh", Cyber.Green, Cyber.C(210, 255, 225), Cyber.C(20, 190, 100));

        // Save
        float sy = 215f + 5f * 150f + 10f;
        Cyber.Spec ss = Cyber.Make(32f, Cyber.C(50, 240, 140, 0.97f), Cyber.C(8, 140, 80, 0.97f),
                                   Cyber.Green, Cyber.C(120, 255, 220), 3f, 0.75f, 10f, 0.04f, 0.9f);
        Cyber.Panel(window, "Save", 150, sy, 600, 112, ss);
        Neo.Icon(window, 300, sy + 30, 54, "check", Color.white, Cyber.C(200, 255, 225));
        Cyber.NeonLabel(window, "SAVE", 385, sy, 300, 112, 44, Cyber.Green, TextAlignmentOptions.Left, 4f);
        UIKit.Hit(window, 150, sy, 600, 112, SaveAndClose);

        BuildList();
        root.SetActive(false);
    }

    void BuildRow(int index, string name, string icon, Color neon, Color iconTop, Color iconBottom)
    {
        float cx = 45f;
        float cy = 215f + index * 150f;
        float cw = 810f;
        float ch = 124f;

        Cyber.Spec rs = Cyber.Make(26f, Cyber.C(30, 20, 70, 0.95f), Cyber.C(12, 8, 36, 0.95f),
                                   neon, neon, 2.5f, 0.35f, 8f, 0.05f, 0.75f);
        Cyber.Panel(window, "Row", cx, cy, cw, ch, rs);
        Cyber.Badge(window, cx + 24, cy + 20, 84, icon, neon, iconTop, iconBottom);
        Cyber.NeonLabel(window, name, cx + 134, cy, 420, ch, 38, neon, TextAlignmentOptions.Left);

        if (index == 0)
        {
            sound = new Cyber.Toggle(window, cx + cw - 200, cy + 24);
            sound.Changed = v => AudioManager.SoundOn = v;
        }
        else if (index == 1)
        {
            vibration = new Cyber.Toggle(window, cx + cw - 200, cy + 24);
            vibration.Changed = v =>
            {
                AudioManager.VibrationOn = v;
                if (v) AudioManager.Vibrate();
            };
        }
        else if (index == 2)
        {
            Cyber.Spec fs = Cyber.Make(18f, Cyber.C(22, 14, 56, 0.97f), Cyber.C(10, 8, 30, 0.97f),
                                       Cyber.Violet, Cyber.Cyan, 3f, 0.35f, 5f, 0f, 1f);
            Cyber.Panel(window, "Field", cx + cw - 300, cy + 24, 270, 76, fs);
            languageText = UIKit.Label(window, "English", cx + cw - 272, cy + 24, 200, 76, 32, Color.white, TextAlignmentOptions.Left);
            Neo.Icon(window, cx + cw - 78, cy + 46, 30, "arrow", Cyber.C(255, 235, 150), Cyber.C(240, 150, 30));
            UIKit.Hit(window, cx + cw - 300, cy + 24, 270, 76, ToggleList);
        }
        else
        {
            Cyber.Spec bs = Cyber.Make(18f, Cyber.C(22, 14, 56, 0.97f), Cyber.C(10, 8, 30, 0.97f),
                                       neon, neon, 3f, 0.5f, 5f, 0f, 1f);
            Cyber.Panel(window, "Go", cx + cw - 110, cy + 24, 76, 76, bs);
            Neo.Icon(window, cx + cw - 92, cy + 42, 40, "chevron", iconTop, iconBottom);
            if (index == 3) UIKit.Hit(window, cx, cy, cw, ch, OpenHelp);
            else UIKit.Hit(window, cx, cy, cw, ch, ResetAll);
        }
    }

    void BuildList()
    {
        RectTransform pop = UIShapes.Stretch(window, "LanguagePopup");
        popup = pop.gameObject;
        UIShapes.MakeButton(pop, HideList);   // বাইরে চাপলে তালিকা বন্ধ

        int n = languages.Length;
        float rowH = 68f;
        float x = 525f;
        float y = 623f;
        float w = 300f;
        float h = rowH * n + 20f;

        Cyber.Spec ls = Cyber.Make(22f, Cyber.C(14, 10, 44, 0.99f), Cyber.C(8, 6, 28, 0.99f),
                                   Cyber.Violet, Cyber.Cyan, 3f, 0.6f, 8f, 0.03f, 0.75f);
        Cyber.Panel(pop, "List", x, y, w, h, ls);

        items = new TMP_Text[n];
        for (int i = 0; i < n; i++)
        {
            int index = i;
            float iy = y + 10f + i * rowH;
            items[i] = UIKit.Label(pop, languages[i], x + 30, iy, w - 60, rowH, 34, Color.white, TextAlignmentOptions.Left);
            UIKit.Hit(pop, x, iy, w, rowH, () => Pick(index));
        }
        popup.SetActive(false);
    }

    // ---------- কাজ ----------
    void Refresh()
    {
        sound.Set(AudioManager.SoundOn);
        vibration.Set(AudioManager.VibrationOn);
        int lang = Mathf.Clamp(AudioManager.Language, 0, languages.Length - 1);
        languageText.text = languages[lang];
        for (int i = 0; i < items.Length; i++)
            items[i].color = (i == lang) ? (Color)Cyber.Green : Color.white;
        popup.SetActive(false);
    }

    void ToggleList()
    {
        popup.SetActive(!popup.activeSelf);
    }

    void HideList()
    {
        popup.SetActive(false);
    }

    void Pick(int index)
    {
        AudioManager.Language = index;
        Refresh();
        AudioManager.PlayClick();
    }

    void OpenHelp()
    {
        AudioManager.PlayClick();
        if (string.IsNullOrEmpty(supportUrl))
        {
            Debug.Log("SettingsPanel: Support Url ঘর খালি (যেমন mailto:তোমার@gmail.com)");
            return;
        }
        Application.OpenURL(supportUrl);
    }

    void ResetAll()
    {
        AudioManager.ResetAll();
        Refresh();
        AudioManager.PlayClick();
    }

    void SaveAndClose()
    {
        AudioManager.PlayClick();
        Close();
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