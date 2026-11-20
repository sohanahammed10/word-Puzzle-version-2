using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Profile Setting মেনু (সাইবারপাংক ৩ডি)। HomeScreen নিজেই এটা যোগ করে নেয়, Profile বাটনে চাপলে Open() চলে।
// নতুন: দেশ + পতাকা, বয়স, পদবি (Title), বর্তমান স্ট্রিক, Ranking অবস্থান, ৭টা ব্যাজের সারি।
// লাগে: Cyber, Neo, UIKit, UIShapes, AvatarArt, Flags (ProfileInfo), GlassKit, RankingData, PlayerData, AudioManager।
public class ProfileMenu : MonoBehaviour
{
    Canvas canvas;
    GameObject root;
    CanvasGroup group;
    RectTransform window;

    TMP_InputField nameInput;
    Image avatarImage;
    TMP_Text levelText;
    TMP_Text xpText;
    RectTransform xpFill;
    readonly TMP_Text[] values = new TMP_Text[8];

    RectTransform titleHolder;
    TMP_Text countryText;
    TMP_Text ageText;
    Image flagImage;
    TMP_Text badgeCount;
    GameObject[] badgeLit;
    GameObject[] badgeDim;

    GameObject chooserRoot;
    GameObject[] selected;
    GameObject countryRoot;

    const float XpBarW = 560f;
    const float XpBarH = 26f;
    const float WinH = 1620f;

    // ব্যাজ: আইকন, লক্ষ্য, স্ট্রিক কিনা (AchievementPanel এর সাথে মিল)
    static readonly string[] BadgeIcons = { "star", "medal", "trophy", "crown", "flame", "bolt", "diamond" };
    static readonly int[] BadgeTarget = { 10, 25, 50, 100, 5, 10, 20 };

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
        AudioManager.Play("open");
        openTime = Time.unscaledTime;
        group.alpha = isActiveAndEnabled ? 0f : 1f;
        window.localScale = Vector3.one;
    }

    public void Close()
    {
        AudioManager.Play("close");
        if (nameInput != null) SaveName(nameInput.text);
        if (chooserRoot != null) chooserRoot.SetActive(false);
        if (countryRoot != null) countryRoot.SetActive(false);
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

        RectTransform rootRt = UIShapes.Stretch(canvas.transform, "ProfileRoot");
        root = rootRt.gameObject;
        group = root.AddComponent<CanvasGroup>();

        Image back = Cyber.Backdrop(rootRt, "Back");
        Button bb = back.gameObject.AddComponent<Button>();
        bb.transition = Selectable.Transition.None;
        bb.targetGraphic = back;
        bb.onClick.AddListener(Close);

        Neo.Glow(rootRt, "GlowA", -340, 100, 900, Cyber.Magenta, 0.45f);
        Neo.Glow(rootRt, "GlowB", 500, 1100, 900, Cyber.Cyan, 0.38f);

        window = MakeWindow(rootRt, 900f, WinH);

        Cyber.Spec ts = Cyber.Make(30f, Cyber.C(40, 14, 80, 0.97f), Cyber.C(16, 8, 44, 0.97f),
                                   Cyber.Magenta, Cyber.Cyan, 3f, 0.6f, 8f, 0.05f, 0.9f);
        Cyber.Panel(window, "Title", 100, 50, 640, 120, ts);
        Cyber.NeonLabel(window, "PROFILE SETTING", 100, 50, 640, 120, 46, Cyber.Cyan, TextAlignmentOptions.Center, 5f);
        CloseButton(window, 780, 38, Close);

        // ছবির ফ্রেম
        Cyber.Spec fs = Cyber.Make(36f, Cyber.C(18, 12, 46, 0.97f), Cyber.C(8, 6, 24, 0.97f),
                                   Cyber.Cyan, Cyber.Magenta, 3f, 0.7f, 8f, 0f, 0.9f);
        Cyber.Panel(window, "AvatarFrame", 60, 215, 190, 190, fs);
        RectTransform pic = UIKit.Box(window, "Avatar", 75, 230, 160, 160);
        avatarImage = pic.gameObject.AddComponent<Image>();
        avatarImage.preserveAspect = true;
        avatarImage.raycastTarget = false;
        UIKit.Hit(window, 60, 215, 190, 190, OpenChooser);

        // নামের ঘর
        Cyber.Spec ns = Cyber.Make(18f, Cyber.C(22, 14, 56, 0.97f), Cyber.C(10, 8, 30, 0.97f),
                                   Cyber.Violet, Cyber.Cyan, 3f, 0.35f, 5f, 0f, 1f);
        Cyber.Panel(window, "NameBox", 280, 225, 560, 84, ns);
        BuildNameInput(300, 225, 520, 84);

        // লেভেল আর XP
        levelText = Cyber.NeonLabel(window, "LEVEL 1", 280, 320, 300, 50, 30, Cyber.Cyan, TextAlignmentOptions.Left, 2f);
        xpText = UIKit.Label(window, "0/100 XP", 540, 320, 300, 50, 26, Cyber.C(190, 200, 240), TextAlignmentOptions.Right);
        Neo.Bar(window, "XpTrack", 280, 372, XpBarW, XpBarH, Cyber.C(8, 12, 40, 0.95f), Cyber.C(24, 20, 64, 0.95f), 0f, true);
        Image fill = Neo.Bar(window, "XpFill", 280, 372, XpBarW, XpBarH, Cyber.C(190, 255, 255), Cyber.C(0, 170, 210), 0.6f, false);
        xpFill = fill.rectTransform;

        // পদবি (লেভেল অনুযায়ী বদলায়)
        titleHolder = UIKit.Box(window, "TitleHolder", 280, 414, 300, 50);

        // দেশ আর বয়স
        BuildInfoRow(ns);

        // আটটা ঘর
        Tile(0, 0, "TOTAL STARS", "star", Cyber.Yellow, Cyber.C(255, 245, 180), Cyber.C(240, 160, 20));
        Tile(1, 1, "TOTAL COINS", "coin", Cyber.C(255, 200, 60), Cyber.C(255, 240, 160), Cyber.C(220, 130, 10));
        Tile(2, 2, "COMPLETED", "check", Cyber.Green, Cyber.C(210, 255, 225), Cyber.C(20, 190, 100));
        Tile(3, 3, "WORDS LEARNED", "book", Cyber.Cyan, Cyber.C(200, 255, 255), Cyber.C(0, 170, 210));
        Tile(4, 4, "BEST STREAK", "flame", Cyber.Magenta, Cyber.C(255, 235, 110), Cyber.C(240, 60, 30));
        Tile(5, 5, "ACHIEVEMENTS", "trophy", Cyber.Violet, Cyber.C(214, 170, 255), Cyber.C(110, 60, 220));
        Tile(6, 6, "CURRENT STREAK", "bolt", Cyber.C(255, 140, 60), Cyber.C(255, 245, 160), Cyber.C(240, 90, 20));
        Tile(7, 7, "RANKING", "crown", Cyber.C(255, 205, 80), Cyber.C(255, 240, 160), Cyber.C(240, 150, 20));

        BuildBadges();

        // Change Avatar আর Edit Name
        float by = 1330f;
        Cyber.Spec cas = Cyber.Make(26f, Cyber.C(60, 40, 150, 0.97f), Cyber.C(24, 14, 80, 0.97f),
                                    Cyber.Violet, Cyber.Cyan, 3f, 0.6f, 8f, 0.04f, 0.9f);
        Cyber.Panel(window, "ChangeAvatar", 45, by, 395, 104, cas);
        Neo.Icon(window, 70, by + 26, 52, "user", Color.white, Cyber.C(200, 210, 255));
        Cyber.NeonLabel(window, "CHANGE AVATAR", 135, by, 300, 104, 26, Cyber.Violet, TextAlignmentOptions.Left, 1f);
        UIKit.Hit(window, 45, by, 395, 104, OpenChooser);

        Cyber.Spec ens = Cyber.Make(26f, Cyber.C(60, 120, 60, 0.97f), Cyber.C(14, 60, 40, 0.97f),
                                    Cyber.Yellow, Cyber.Green, 3f, 0.6f, 8f, 0.04f, 0.9f);
        Cyber.Panel(window, "EditName", 460, by, 395, 104, ens);
        Neo.Icon(window, 485, by + 26, 52, "pencil", Color.white, Cyber.C(255, 240, 180));
        Cyber.NeonLabel(window, "EDIT NAME", 550, by, 300, 104, 28, Cyber.Green, TextAlignmentOptions.Left, 1f);
        UIKit.Hit(window, 460, by, 395, 104, FocusName);

        // Save (নাম সেভ করে বন্ধ হয়)
        float bk = by + 130f;
        Cyber.Spec bs = Cyber.Make(32f, Cyber.C(50, 240, 140, 0.97f), Cyber.C(8, 140, 80, 0.97f),
                                   Cyber.Green, Cyber.C(120, 255, 220), 3f, 0.75f, 10f, 0.04f, 0.9f);
        Cyber.Panel(window, "Save", 150, bk, 600, 104, bs);
        Neo.Icon(window, 300, bk + 25, 54, "check", Color.white, Cyber.C(200, 255, 225));
        Cyber.NeonLabel(window, "SAVE", 385, bk, 300, 104, 42, Cyber.Green, TextAlignmentOptions.Left, 4f);
        UIKit.Hit(window, 150, bk, 600, 104, SaveAndClose);

        root.SetActive(false);
    }

    // ---------- দেশ আর বয়সের সারি ----------
    void BuildInfoRow(Cyber.Spec boxSpec)
    {
        const float y = 484f;

        // দেশ
        Cyber.Panel(window, "CountryBox", 45, y, 395, 104, boxSpec);
        RectTransform fl = UIKit.Box(window, "Flag", 62, y + 16, 100, 72);
        flagImage = fl.gameObject.AddComponent<Image>();
        flagImage.preserveAspect = true;
        flagImage.raycastTarget = false;
        countryText = UIKit.Label(window, "Select country", 176, y, 210, 104, 28, Color.white, TextAlignmentOptions.MidlineLeft);
        countryText.enableAutoSizing = true;
        countryText.fontSizeMin = 14f;
        countryText.fontSizeMax = 28f;
        Neo.Icon(window, 45 + 395 - 52, y + 34, 36, "chevron", Cyber.C(255, 235, 150), Cyber.C(240, 150, 30));
        UIKit.Hit(window, 45, y, 395, 104, OpenCountryChooser);

        // বয়স
        Cyber.Panel(window, "AgeBox", 460, y, 395, 104, boxSpec);
        Cyber.Spec bs = Cyber.Make(18f, Cyber.C(60, 40, 150, 0.97f), Cyber.C(24, 14, 80, 0.97f),
                                   Cyber.Violet, Cyber.Cyan, 3f, 0.6f, 6f, 0.04f, 0.9f);
        Cyber.Panel(window, "AgeMinus", 478, y + 18, 68, 68, bs);
        UIKit.Label(window, "-", 478, y + 18, 68, 68, 52, Color.white, TextAlignmentOptions.Center);
        UIKit.Hit(window, 478, y + 18, 68, 68, AgeMinus);
        Cyber.Panel(window, "AgePlus", 460 + 395 - 18 - 68, y + 18, 68, 68, bs);
        UIKit.Label(window, "+", 460 + 395 - 18 - 68, y + 18, 68, 68, 52, Color.white, TextAlignmentOptions.Center);
        UIKit.Hit(window, 460 + 395 - 18 - 68, y + 18, 68, 68, AgePlus);
        ageText = UIKit.Label(window, "Age: not set", 552, y, 190, 104, 28, Color.white, TextAlignmentOptions.Center);
        ageText.enableAutoSizing = true;
        ageText.fontSizeMin = 14f;
        ageText.fontSizeMax = 28f;
    }

    // ---------- ব্যাজের সারি ----------
    void BuildBadges()
    {
        badgeCount = UIKit.Label(window, "BADGES", 45, 1158, 810, 40, 26, Cyber.C(190, 200, 240), TextAlignmentOptions.Left);

        Color[] neon =
        {
            Cyber.C(255, 160, 70), Cyber.C(60, 220, 200), Cyber.C(170, 110, 255), Cyber.C(255, 205, 80),
            Cyber.C(255, 120, 60), Cyber.C(70, 170, 255), Cyber.C(255, 100, 190)
        };
        Color dimN = Cyber.C(120, 130, 190);

        badgeLit = new GameObject[7];
        badgeDim = new GameObject[7];
        const float d = 100f;
        for (int i = 0; i < 7; i++)
        {
            float x = 45f + i * 118f;
            float y = 1205f;

            RectTransform lit = UIKit.Box(window, "BadgeLit" + i, x, y, d, d);
            Cyber.Badge(lit, 0, 0, d, BadgeIcons[i], neon[i], Neo.Mix(neon[i], 1f, 70f), neon[i]);
            badgeLit[i] = lit.gameObject;

            RectTransform dim = UIKit.Box(window, "BadgeDim" + i, x, y, d, d);
            Cyber.Badge(dim, 0, 0, d, BadgeIcons[i], dimN, Cyber.C(150, 156, 190), Cyber.C(88, 94, 130));
            badgeDim[i] = dim.gameObject;
        }
    }

    // মাঝখানের জানালা (ছায়া ফ্রেম সহ)
    RectTransform MakeWindow(RectTransform rootRt, float w, float h)
    {
        GameObject wgo = new GameObject("Window", typeof(RectTransform));
        RectTransform win = wgo.GetComponent<RectTransform>();
        win.SetParent(rootRt, false);
        win.anchorMin = new Vector2(0.5f, 0.5f);
        win.anchorMax = new Vector2(0.5f, 0.5f);
        win.pivot = new Vector2(0.5f, 0.5f);
        win.sizeDelta = new Vector2(w, h);
        win.anchoredPosition = Vector2.zero;

        // জানালার ভেতরে চাপলে যেন পেছনের পর্দা বন্ধ না হয়
        RectTransform block = UIKit.Box(win, "Block", 0, 0, w, h);
        Image bi = block.gameObject.AddComponent<Image>();
        bi.color = new Color(0f, 0f, 0f, 0f);
        bi.raycastTarget = true;

        Cyber.Spec ws = Cyber.Make(60f, Cyber.C(26, 16, 62, 0.97f), Cyber.C(10, 6, 30, 0.97f),
                                   Cyber.Cyan, Cyber.Magenta, 3f, 0.7f, 14f, 0.04f, 0.6f);
        Cyber.Panel(win, "Frame", 0, 0, w, h, ws);
        return win;
    }

    void CloseButton(Transform parent, float x, float y, UnityEngine.Events.UnityAction action)
    {
        Cyber.Spec cs = Cyber.Make(20f, Cyber.C(60, 10, 60, 0.97f), Cyber.C(24, 6, 30, 0.97f),
                                   Cyber.Magenta, Cyber.Magenta, 3f, 0.7f, 6f, 0f, 1f);
        Cyber.Panel(parent, "Close", x, y, 80, 80, cs);
        Neo.Icon(parent, x + 18, y + 18, 44, "x", Cyber.C(255, 200, 240), Cyber.C(255, 40, 190));
        UIKit.Hit(parent, x, y, 80, 80, action);
    }

    void Tile(int slot, int index, string label, string icon, Color neon, Color iconTop, Color iconBottom)
    {
        float x = 45f + (index % 2) * 415f;
        float y = 612f + (index / 2) * 136f;
        Cyber.Spec ts = Cyber.Make(22f, Cyber.C(30, 20, 70, 0.95f), Cyber.C(12, 8, 36, 0.95f),
                                   neon, neon, 2.5f, 0.35f, 8f, 0.05f, 0.75f);
        Cyber.Panel(window, "Tile", x, y, 395, 120, ts);
        Neo.Icon(window, x + 22, y + 26, 68, icon, iconTop, iconBottom);
        UIKit.Label(window, label, x + 104, y + 17, 280, 34, 22, Cyber.C(190, 200, 240), TextAlignmentOptions.Left);
        values[slot] = Cyber.NeonLabel(window, "0", x + 104, y + 48, 280, 60, 44, neon, TextAlignmentOptions.Left);
    }

    // নাম লেখার ঘর (আসল টাইপ করা যায়)
    void BuildNameInput(float x, float y, float w, float h)
    {
        RectTransform inRt = UIKit.Box(window, "NameInput", x, y, w, h);
        GameObject inGo = inRt.gameObject;
        Image inImg = inGo.AddComponent<Image>();
        inImg.color = new Color(0f, 0f, 0f, 0f);
        inImg.raycastTarget = true;

        RectTransform area = UIShapes.Stretch(inRt, "TextArea", 6f);
        area.gameObject.AddComponent<RectMask2D>();

        TMP_Text text = UIShapes.NewText(area, "Text", "", Color.white);
        text.enableAutoSizing = false;
        text.fontSize = 36;
        text.fontStyle = FontStyles.Normal;
        text.alignment = TextAlignmentOptions.MidlineLeft;

        TMP_Text ph = UIShapes.NewText(area, "Placeholder", "Tap to enter name", Cyber.C(150, 160, 215));
        ph.enableAutoSizing = false;
        ph.fontSize = 36;
        ph.fontStyle = FontStyles.Italic;
        ph.alignment = TextAlignmentOptions.MidlineLeft;

        inGo.SetActive(false);
        nameInput = inGo.AddComponent<TMP_InputField>();
        nameInput.textViewport = area;
        nameInput.textComponent = text;
        nameInput.placeholder = ph;
        nameInput.targetGraphic = inImg;
        nameInput.characterLimit = 14;
        nameInput.caretWidth = 3;
        nameInput.customCaretColor = true;
        nameInput.caretColor = Cyber.Cyan;
        nameInput.selectionColor = new Color(0f, 0.9f, 1f, 0.35f);
        nameInput.onEndEdit.AddListener(SaveName);
        inGo.SetActive(true);
    }

    // ---------- কাজ ----------
    static string TitleFor(int lvl, out Color top, out Color bottom)
    {
        if (lvl >= 15) { top = Neo.C(255, 232, 130); bottom = Neo.C(236, 138, 22); return "WORD LEGEND"; }
        if (lvl >= 10) { top = Neo.C(214, 170, 255); bottom = Neo.C(110, 60, 220); return "WORD MASTER"; }
        if (lvl >= 6) { top = Neo.C(120, 255, 230); bottom = Neo.C(20, 160, 170); return "WORD EXPLORER"; }
        if (lvl >= 3) { top = Neo.C(150, 210, 255); bottom = Neo.C(60, 130, 235); return "WORD HUNTER"; }
        top = Neo.C(170, 240, 170); bottom = Neo.C(40, 170, 90);
        return "BEGINNER";
    }

    void Refresh()
    {
        nameInput.text = PlayerData.Name;

        int lvl = PlayerData.PlayerLevel;
        levelText.text = "LEVEL " + lvl;
        xpText.text = PlayerData.XpInLevel + "/100 XP";
        float frac = Mathf.Clamp01(PlayerData.XpInLevel / 100f);
        float w = frac <= 0f ? 0f : Mathf.Max(XpBarH, XpBarW * frac);
        xpFill.sizeDelta = new Vector2(w, XpBarH);
        xpFill.gameObject.SetActive(w > 0.5f);

        // পদবি
        for (int i = titleHolder.childCount - 1; i >= 0; i--) Destroy(titleHolder.GetChild(i).gameObject);
        Color tt, tb;
        string tn = TitleFor(lvl, out tt, out tb);
        GlassKit.Chip(titleHolder, tn, 0, 0, 300, 50, tt, tb, Color.white, 24);

        values[0].text = PlayerData.Stars.ToString();
        values[1].text = PlayerData.Coins.ToString();
        values[2].text = PlayerData.CompletedLevels.ToString();
        values[3].text = PlayerData.CompletedLevels.ToString();
        values[4].text = PlayerData.BestStreak.ToString();
        values[5].text = PlayerData.Achievements + "/7";
        values[6].text = PlayerData.CurrentStreak.ToString();
        RankEntry me;
        RankingData.Build(out me);
        values[7].text = "#" + me.rank;

        // ব্যাজ
        int got = 0;
        for (int i = 0; i < 7; i++)
        {
            int v = i >= 4 ? PlayerData.BestStreak : PlayerData.CompletedLevels;
            bool on = v >= BadgeTarget[i];
            badgeLit[i].SetActive(on);
            badgeDim[i].SetActive(!on);
            if (on) got++;
        }
        badgeCount.text = "BADGES  " + got + " / 7";

        RefreshInfo();
        ShowAvatar();
    }

    void RefreshInfo()
    {
        int ci = ProfileInfo.CountryIndex;
        if (ci >= 0)
        {
            flagImage.enabled = true;
            flagImage.sprite = Flags.Get(ci);
            countryText.text = Flags.Name(ci);
            countryText.color = Color.white;
        }
        else
        {
            flagImage.enabled = false;
            countryText.text = "Select country";
            countryText.color = Cyber.C(150, 160, 215);
        }

        int age = ProfileInfo.Age;
        ageText.text = age > 0 ? "Age: " + age : "Age: not set";
        ageText.color = age > 0 ? Color.white : Cyber.C(150, 160, 215);
    }

    void AgeMinus()
    {
        AudioManager.PlayClick();
        int a = ProfileInfo.Age;
        ProfileInfo.Age = a <= 1 ? 0 : a - 1;
        RefreshInfo();
    }

    void AgePlus()
    {
        AudioManager.PlayClick();
        int a = ProfileInfo.Age;
        ProfileInfo.Age = a == 0 ? 10 : a + 1;
        RefreshInfo();
    }

    void SaveAndClose()
    {
        AudioManager.PlayClick();
        Close();   // Close নামটা সেভ করে নেয়
    }

    void SaveName(string value)
    {
        value = value == null ? "" : value.Trim();
        if (value.Length == 0) value = "Player";
        PlayerData.Name = value;
        if (nameInput != null) nameInput.text = value;
    }

    void FocusName()
    {
        AudioManager.PlayClick();
        nameInput.Select();
        nameInput.ActivateInputField();
    }

    void ShowAvatar()
    {
        int i = PlayerData.AvatarIndex;
        if (i < 0 || i >= AvatarArt.Count) i = 0;   // এখনো না বাছলে প্রথমটা (বিড়াল)
        avatarImage.sprite = AvatarArt.Get(i);
        avatarImage.color = Color.white;
    }

    // ---------- দেশ বাছার তালিকা ----------
    void OpenCountryChooser()
    {
        AudioManager.PlayClick();
        if (countryRoot != null) Destroy(countryRoot);

        RectTransform rootRt = UIShapes.Stretch(canvas.transform, "CountryChooser");
        countryRoot = rootRt.gameObject;

        Image back = Cyber.Backdrop(rootRt, "Back");
        Button bb = back.gameObject.AddComponent<Button>();
        bb.transition = Selectable.Transition.None;
        bb.targetGraphic = back;
        bb.onClick.AddListener(HideCountry);

        RectTransform win = MakeWindow(rootRt, 860f, 1300f);

        Cyber.Spec ts = Cyber.Make(30f, Cyber.C(40, 14, 80, 0.97f), Cyber.C(16, 8, 44, 0.97f),
                                   Cyber.Magenta, Cyber.Cyan, 3f, 0.6f, 8f, 0.05f, 0.9f);
        Cyber.Panel(win, "Title", 90, 50, 600, 120, ts);
        Cyber.NeonLabel(win, "CHOOSE COUNTRY", 90, 50, 600, 120, 42, Cyber.Cyan, TextAlignmentOptions.Center, 4f);
        CloseButton(win, 740, 38, HideCountry);

        Cyber.Spec tile = Cyber.Make(22f, Cyber.C(18, 12, 46, 0.97f), Cyber.C(8, 6, 24, 0.97f),
                                     Cyber.Cyan, Cyber.Magenta, 3f, 0.4f, 4f, 0f, 1f);
        Cyber.Spec pick = Cyber.Make(22f, Cyber.C(18, 40, 30, 0.97f), Cyber.C(8, 24, 20, 0.97f),
                                     Cyber.Green, Cyber.Green, 4f, 0.9f, 4f, 0f, 1f);

        int n = Flags.Count;
        int rows = (n + 3) / 4;
        RectTransform content;
        GlassKit.Scroll(win, 30, 200, 800, 1050, rows * 170f + 20f, out content);

        int cur = ProfileInfo.CountryIndex;
        for (int i = 0; i < n; i++)
        {
            int index = i;
            float x = (i % 4) * 200f;
            float y = 10f + (i / 4) * 170f;

            Cyber.Panel(content, "Tile", x, y, 190, 160, i == cur ? pick : tile);

            RectTransform fl = UIKit.Box(content, "Flag", x + 30, y + 10, 130, 94);
            Image fi = fl.gameObject.AddComponent<Image>();
            fi.sprite = Flags.Get(i);
            fi.preserveAspect = true;
            fi.raycastTarget = false;

            TMP_Text nm = UIKit.Label(content, Flags.Name(i), x + 6, y + 108, 178, 40, 20, Color.white, TextAlignmentOptions.Center);
            nm.enableAutoSizing = true;
            nm.fontSizeMin = 12f;
            nm.fontSizeMax = 20f;

            UIKit.Hit(content, x, y, 190, 160, () => ChooseCountry(index));
        }
        countryRoot.transform.SetAsLastSibling();
    }

    void HideCountry()
    {
        if (countryRoot != null) countryRoot.SetActive(false);
    }

    void ChooseCountry(int index)
    {
        ProfileInfo.CountryIndex = index;
        RefreshInfo();
        HideCountry();
        AudioManager.PlayClick();
    }

    // ---------- Avatar বাছার তালিকা ----------
    void OpenChooser()
    {
        AudioManager.PlayClick();
        if (chooserRoot == null) BuildChooser();
        int sel = PlayerData.AvatarIndex;
        for (int i = 0; i < selected.Length; i++) selected[i].SetActive(i == sel);
        chooserRoot.SetActive(true);
        chooserRoot.transform.SetAsLastSibling();
    }

    void HideChooser()
    {
        if (chooserRoot != null) chooserRoot.SetActive(false);
    }

    void BuildChooser()
    {
        RectTransform rootRt = UIShapes.Stretch(canvas.transform, "AvatarChooser");
        chooserRoot = rootRt.gameObject;

        Image back = Cyber.Backdrop(rootRt, "Back");
        Button bb = back.gameObject.AddComponent<Button>();
        bb.transition = Selectable.Transition.None;
        bb.targetGraphic = back;
        bb.onClick.AddListener(HideChooser);

        RectTransform win = MakeWindow(rootRt, 860f, 1300f);

        Cyber.Spec ts = Cyber.Make(30f, Cyber.C(40, 14, 80, 0.97f), Cyber.C(16, 8, 44, 0.97f),
                                   Cyber.Magenta, Cyber.Cyan, 3f, 0.6f, 8f, 0.05f, 0.9f);
        Cyber.Panel(win, "Title", 90, 50, 600, 120, ts);
        Cyber.NeonLabel(win, "CHOOSE AVATAR", 90, 50, 600, 120, 46, Cyber.Cyan, TextAlignmentOptions.Center, 5f);
        CloseButton(win, 740, 38, HideChooser);

        Cyber.Spec tile = Cyber.Make(28f, Cyber.C(18, 12, 46, 0.97f), Cyber.C(8, 6, 24, 0.97f),
                                     Cyber.Cyan, Cyber.Magenta, 3f, 0.5f, 6f, 0f, 1f);
        Cyber.Spec pick = Cyber.Make(28f, Cyber.C(18, 40, 30, 0.97f), Cyber.C(8, 24, 20, 0.97f),
                                     Cyber.Green, Cyber.Green, 4f, 0.9f, 6f, 0f, 1f);

        selected = new GameObject[AvatarArt.Count];
        for (int i = 0; i < AvatarArt.Count; i++)
        {
            int index = i;
            float x = 45f + (i % 5) * 156f;      // ৫ কলাম × ৬ সারি = ৩০টা ছবি
            float y = 250f + (i / 5) * 152f;

            Cyber.Panel(win, "Tile", x, y, 136, 136, tile);
            Image mark = Cyber.Panel(win, "Selected", x, y, 136, 136, pick);
            selected[i] = mark.gameObject;

            RectTransform pic = UIKit.Box(win, "Avatar", x + 12, y + 12, 112, 112);
            Image im = pic.gameObject.AddComponent<Image>();
            im.sprite = AvatarArt.Get(i);
            im.preserveAspect = true;
            im.raycastTarget = false;

            UIKit.Hit(win, x, y, 136, 136, () => Choose(index));
        }
        chooserRoot.SetActive(false);
    }

    void Choose(int index)
    {
        PlayerData.AvatarIndex = index;
        ShowAvatar();
        HideChooser();
        AudioManager.PlayClick();
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