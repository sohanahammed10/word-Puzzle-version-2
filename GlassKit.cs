using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// WORD MAP, WORD BOOK আর CHALLENGE পাতার কমন গ্লাসমরফিজম ৩D স্টাইল।
// Neo, UIKit, UIShapes স্ক্রিপ্ট লাগে (আগের মতোই)।
public static class GlassKit
{
    public static readonly string[] TierNames =
    {
        "EASY", "BASIC", "NORMAL", "MEDIUM", "ADVANCED",
        "HARD", "EXPERT", "MASTER", "GENIUS", "LEGEND"
    };

    // টিয়ারের রং: সবুজ থেকে সোনালি
    public static readonly Color[] TierColors =
    {
        Neo.C(90, 220, 120), Neo.C(70, 210, 190), Neo.C(70, 170, 255), Neo.C(130, 130, 255), Neo.C(190, 110, 255),
        Neo.C(255, 110, 200), Neo.C(255, 120, 90), Neo.C(255, 160, 60), Neo.C(255, 200, 70), Neo.C(255, 225, 110)
    };

    // GameController এর শব্দের তালিকার কপি (পরে WordData আসলে এখান থেকে সরে যাবে)
    public static readonly string[][] Words = new string[][]
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

    // এখন পর্যন্ত কয়টা লেভেল শেষ (০ থেকে ১০০)
    public static int Reached
    {
        get { return Mathf.Clamp(PlayerData.CompletedLevels, 0, 100); }
    }

    // ---------- স্টাইল ----------
    public static Neo.Style St(float radius, Color top, Color bottom, Color edgeTop, Color edgeBottom, float edgeW,
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

    // মূল কাচের জানালা
    public static Neo.Style WindowStyle()
    {
        return St(56, Neo.C(255, 255, 255, 0.17f), Neo.C(255, 255, 255, 0.06f),
                  Neo.C(255, 255, 255, 0.45f), Neo.C(255, 255, 255, 0.10f), 2.5f, 0f, 0.55f, 70f, 30f, 0.5f);
    }

    // গোল ব্যাজ: রঙিন (done) বা ধূসর
    public static void Badge(Transform p, float x, float y, float d, Color accent, bool lit)
    {
        if (lit)
        {
            Neo.Surface(p, "Badge", x, y, d, d,
                St(d * 0.5f, Neo.Mix(accent, 0.55f, 90f), Neo.Mix(accent, 0.55f, 0f),
                   Neo.C(255, 255, 255, 0.7f), accent, 3.5f, 0.5f, 0.5f, 16f, 8f, 1f));
        }
        else
        {
            Neo.Surface(p, "Badge", x, y, d, d,
                St(d * 0.5f, Neo.C(70, 76, 120), Neo.C(34, 38, 72), Neo.C(255, 255, 255, 0.25f),
                   Neo.C(255, 255, 255, 0.05f), 3f, 0.2f, 0.4f, 14f, 6f, 1f));
        }
    }

    // কার্ড: lit = true হলে রঙিন ধার আর আলো, নাহলে ঝাপসা ধূসর। pulses দিলে আলো দপদপ করে।
    public static void Card(Transform p, float x, float y, float w, float h, Color accent, bool lit, List<Image> pulses)
    {
        if (lit)
        {
            float d = Mathf.Min(w, 420f);
            Image glow = Neo.Glow(p, "CardGlow", x + w * 0.5f - d * 0.5f, y + h * 0.5f - d * 0.5f, d, accent, 0.40f);
            if (pulses != null) pulses.Add(glow);
            Color e1 = accent; e1.a = 0.95f;
            Color e2 = accent; e2.a = 0.35f;
            Neo.Surface(p, "Card", x, y, w, h,
                St(36, Neo.C(70, 80, 170, 0.92f), Neo.C(38, 40, 110, 0.92f), e1, e2, 2.5f, 0.35f, 0.45f, 36f, 14f, 0.75f));
        }
        else
        {
            Neo.Surface(p, "Card", x, y, w, h,
                St(36, Neo.C(46, 52, 110, 0.72f), Neo.C(28, 30, 76, 0.72f),
                   Neo.C(255, 255, 255, 0.22f), Neo.C(255, 255, 255, 0.06f), 2.5f, 0.35f, 0.45f, 36f, 14f, 0.75f));
        }
    }

    // ছোট গোল লেখার ঘর (চিপ)
    public static void Chip(Transform p, string text, float x, float y, float w, float h,
                            Color top, Color bottom, Color textColor, float size)
    {
        Neo.Surface(p, "Chip", x, y, w, h,
            St(h * 0.5f, top, bottom, Neo.C(255, 255, 255, 0.7f), Neo.C(255, 255, 255, 0.15f), 2.5f, 0.5f, 0.45f, 14f, 6f, 1f));
        UIKit.Label(p, text, x, y, w, h, size, textColor, TMPro.TextAlignmentOptions.Center);
    }

    // ---------- স্ক্রলভিউ ----------
    // x, y = বাম-উপর কোণ থেকে দূরত্ব। content এর ভেতরে x, y ও একই নিয়মে।
    public static ScrollRect Scroll(Transform parent, float x, float y, float w, float h, float contentH, out RectTransform content)
    {
        RectTransform view = UIKit.Box(parent, "ScrollView", x, y, w, h);
        Image hit = view.gameObject.AddComponent<Image>();
        hit.color = new Color(0f, 0f, 0f, 0f);
        hit.raycastTarget = true;
        view.gameObject.AddComponent<RectMask2D>();

        content = UIKit.Box(view, "Content", 0, 0, w, contentH);

        ScrollRect sr = view.gameObject.AddComponent<ScrollRect>();
        sr.content = content;
        sr.viewport = view;
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Elastic;
        sr.scrollSensitivity = 40f;
        sr.decelerationRate = 0.135f;
        return sr;
    }

    // ---------- খোঁজা ----------
    public static bool NameHas(string objName, string[] tokens)
    {
        string n = objName.ToLowerInvariant();
        for (int i = 0; i < tokens.Length; i++)
        {
            if (n.Contains(tokens[i])) return true;
        }
        return false;
    }

    public static T FindIn<T>(Func<T, bool> pick) where T : Component
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
}

// MainMenu সিন খুললে তিনটা পাতা নিজে নিজেই তৈরি হয়। কোনো অবজেক্টে কিছু লাগাতে হয় না।
public static class GlassMenus
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        SceneManager.sceneLoaded += OnLoaded;
        Spawn(SceneManager.GetActiveScene().name);
    }

    static void OnLoaded(Scene scene, LoadSceneMode mode)
    {
        Spawn(scene.name);
    }

    static void Spawn(string sceneName)
    {
        if (sceneName != "MainMenu") return;
        if (GlassKit.FindIn<WordMapPanel>(null) == null) new GameObject("WordMapPanel").AddComponent<WordMapPanel>();
        if (GlassKit.FindIn<WordBookPanel>(null) == null) new GameObject("WordBookPanel").AddComponent<WordBookPanel>();
        if (GlassKit.FindIn<ChallengePanel>(null) == null) new GameObject("ChallengePanel").AddComponent<ChallengePanel>();
    }
}