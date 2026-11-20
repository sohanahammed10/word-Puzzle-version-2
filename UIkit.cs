using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// সুন্দর মেনু আঁকার সাহায্যকারী। SettingsPanel আর SoundPanel এটা ব্যবহার করে।
// এখানে কিছু বদলাতে হবে না।
public static class UIKit
{
    // ---------- রং ----------
    public static readonly Color Gold = new Color32(255, 200, 45, 255);
    public static readonly Color GoldDark = new Color32(190, 120, 12, 255);
    public static readonly Color Blue = new Color32(30, 100, 215, 255);
    public static readonly Color BlueDeep = new Color32(14, 52, 150, 255);
    public static readonly Color Row = new Color32(20, 72, 176, 255);
    public static readonly Color RowEdge = new Color32(92, 160, 245, 255);
    public static readonly Color Navy = new Color32(10, 30, 92, 255);
    public static readonly Color Green = new Color32(76, 210, 52, 255);
    public static readonly Color Red = new Color32(232, 54, 54, 255);
    public static readonly Color Yellow = new Color32(255, 226, 90, 255);
    public static readonly Color Cyan = new Color32(48, 196, 245, 255);

    static Sprite roundSprite;

    // ---------- মূল আকার ----------
    // x, y = বাম-উপর কোণ থেকে দূরত্ব (নিচের দিকে বাড়ে)
    public static RectTransform Box(Transform parent, string name, float x, float y, float w, float h)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(x, -y);
        rt.sizeDelta = new Vector2(w, h);
        return rt;
    }

    static Sprite RoundSprite()
    {
        if (roundSprite != null) return roundSprite;

        const int R = 32;
        const int S = 2 * R + 2;
        Texture2D tex = new Texture2D(S, S, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        Color32[] px = new Color32[S * S];
        for (int y = 0; y < S; y++)
        {
            for (int x = 0; x < S; x++)
            {
                float cx = Mathf.Clamp(x + 0.5f, R, S - R);
                float cy = Mathf.Clamp(y + 0.5f, R, S - R);
                float dx = x + 0.5f - cx;
                float dy = y + 0.5f - cy;
                float a = Mathf.Clamp01(R - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
                px[y * S + x] = new Color32(255, 255, 255, (byte)(a * 255f));
            }
        }
        tex.SetPixels32(px);
        tex.Apply(false, true);
        roundSprite = Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(R, R, R, R));
        return roundSprite;
    }

    // গোল-কোণা চতুর্ভুজ (radius বড় দিলে লম্বা গোল বা পুরো গোল হয়)
    public static Image Round(Transform parent, string name, float x, float y, float w, float h, float radius, Color color)
    {
        RectTransform rt = Box(parent, name, x, y, w, h);
        Image im = rt.gameObject.AddComponent<Image>();
        float r = Mathf.Min(radius, Mathf.Min(w, h) * 0.5f - 0.5f);
        im.sprite = RoundSprite();
        im.type = Image.Type.Sliced;
        im.pixelsPerUnitMultiplier = 32f / Mathf.Max(1f, r);
        im.color = color;
        im.raycastTarget = false;
        return im;
    }

    public static TMP_Text Label(Transform parent, string text, float x, float y, float w, float h, float size, Color color, TextAlignmentOptions align, bool outline = false)
    {
        RectTransform box = Box(parent, "Label", x, y, w, h);
        TMP_Text t = UIShapes.NewText(box, "Text", text, color);
        t.enableAutoSizing = false;
        t.fontSize = size;
        t.alignment = align;
        if (outline)
        {
            t.outlineWidth = 0.22f;
            t.outlineColor = new Color32(8, 28, 90, 255);
        }
        return t;
    }

    public static void Hit(Transform parent, float x, float y, float w, float h, UnityAction action)
    {
        UIShapes.MakeButton(Box(parent, "Hit", x, y, w, h), action);
    }

    public static Image Icon(Transform parent, float x, float y, float size, string name, Color tint)
    {
        RectTransform rt = Box(parent, "Icon_" + name, x, y, size, size);
        Image im = rt.gameObject.AddComponent<Image>();
        im.sprite = Icons.Get(name);
        im.color = tint;
        im.preserveAspect = true;
        im.raycastTarget = false;
        return im;
    }

    // ---------- সাজানো অংশ ----------
    // ওপরের অর্ধেকে হালকা চকচকে ভাব
    public static void Gloss(Transform parent, float x, float y, float w, float h, float radius)
    {
        float gh = h * 0.46f;
        Round(parent, "Gloss", x + 8, y + 5, w - 16, gh, radius, new Color(1f, 1f, 1f, 0.22f));
    }

    // বড় জানালা: ছায়া, সোনালি ফ্রেম, নীল ভেতর। রুট অবজেক্ট ফেরত দেয়, window বের করে দেয়।
    public static GameObject Popup(Transform parent, string name, float w, float h, out RectTransform window)
    {
        RectTransform rootRt = UIShapes.Stretch(parent, name);
        Image dim = rootRt.gameObject.AddComponent<Image>();
        dim.color = new Color(0f, 0f, 0f, 0.7f);
        dim.raycastTarget = true;

        GameObject wgo = new GameObject("Window", typeof(RectTransform));
        window = wgo.GetComponent<RectTransform>();
        window.SetParent(rootRt, false);
        window.anchorMin = new Vector2(0.5f, 0.5f);
        window.anchorMax = new Vector2(0.5f, 0.5f);
        window.pivot = new Vector2(0.5f, 0.5f);
        window.sizeDelta = new Vector2(w, h);
        window.anchoredPosition = Vector2.zero;

        Round(window, "Shadow", 0, 18, w, h, 66, new Color(0f, 0f, 0f, 0.4f));
        Round(window, "FrameEdge", 0, 8, w, h, 66, GoldDark);
        Round(window, "Frame", 0, 0, w, h, 66, Gold);
        Round(window, "FrameInner", 12, 12, w - 24, h - 24, 56, GoldDark);
        Round(window, "Body", 17, 17, w - 34, h - 34, 51, Blue);
        Round(window, "BodyShine", 17, 17, w - 34, (h - 34) * 0.32f, 51, new Color(1f, 1f, 1f, 0.08f));
        return rootRt.gameObject;
    }

    // নাম লেখা ফিতা
    public static void Title(Transform parent, string text, float x, float y, float w, float h, float size)
    {
        float r = h * 0.3f;
        Round(parent, "TitleShadow", x, y + 10, w, h, r, new Color(0f, 0f, 0f, 0.3f));
        Round(parent, "TitleEdge", x, y, w, h, r, Gold);
        Round(parent, "TitleBack", x + 8, y + 8, w - 16, h - 16, r - 6, BlueDeep);
        Gloss(parent, x + 8, y + 8, w - 16, h - 16, r - 6);
        TMP_Text t = Label(parent, text, x, y, w, h, size, Yellow, TextAlignmentOptions.Center);
        t.outlineWidth = 0.3f;
        t.outlineColor = new Color32(120, 60, 0, 255);
    }

    // সারির ঘর (radius না দিলে লম্বা গোল)
    public static void RowCard(Transform parent, float x, float y, float w, float h, float radius = -1f)
    {
        float r = radius < 0f ? h * 0.5f : radius;
        Round(parent, "RowShadow", x, y + 6, w, h, r, new Color(0f, 0f, 0f, 0.25f));
        Round(parent, "RowEdge", x, y, w, h, r, RowEdge);
        Round(parent, "RowBack", x + 4, y + 4, w - 8, h - 8, r - 4, Row);
        Gloss(parent, x + 4, y + 4, w - 8, h - 8, r - 4);
    }

    // গোল আইকন ব্যাজ
    public static void IconBadge(Transform parent, float x, float y, float d, string icon, Color tint)
    {
        Round(parent, "BadgeShadow", x, y + 5, d, d, d * 0.5f, new Color(0f, 0f, 0f, 0.3f));
        Round(parent, "BadgeRing", x, y, d, d, d * 0.5f, Gold);
        Round(parent, "BadgeBack", x + 6, y + 6, d - 12, d - 12, (d - 12) * 0.5f, Navy);
        Icon(parent, x + d * 0.2f, y + d * 0.2f, d * 0.6f, icon, tint);
    }

    // উঁচু সোনালি পাড়ের বোতাম
    public static void GoldButton(Transform parent, float x, float y, float w, float h, Color face)
    {
        float r = h * 0.5f;
        Round(parent, "BtnShadow", x, y + 14, w, h, r, new Color(0f, 0f, 0f, 0.35f));
        Round(parent, "BtnEdge", x, y + 8, w, h, r, GoldDark);
        Round(parent, "BtnRing", x, y, w, h, r, Gold);
        Round(parent, "BtnFace", x + 7, y + 7, w - 14, h - 14, r - 7, face);
        Gloss(parent, x + 7, y + 7, w - 14, h - 14, r - 7);
    }

    public static void IconButton(Transform parent, float x, float y, float w, float h, string icon, Color face, UnityAction action)
    {
        GoldButton(parent, x, y, w, h, face);
        float s = h * 0.55f;
        Icon(parent, x + (w - s) * 0.5f, y + (h - s) * 0.5f, s, icon, Color.white);
        Hit(parent, x, y, w, h, action);
    }

    public static void CloseButton(Transform parent, float x, float y, float d, UnityAction action)
    {
        Round(parent, "CloseShadow", x, y + 7, d, d, d * 0.5f, new Color(0f, 0f, 0f, 0.35f));
        Round(parent, "CloseRing", x, y, d, d, d * 0.5f, Gold);
        Round(parent, "CloseFace", x + 7, y + 7, d - 14, d - 14, (d - 14) * 0.5f, Red);
        Gloss(parent, x + 7, y + 7, d - 14, d - 14, (d - 14) * 0.5f);
        Icon(parent, x + d * 0.27f, y + d * 0.27f, d * 0.46f, "x", Color.white);
        Hit(parent, x, y, d, d, action);
    }

    // লাল ফিতা (Language এর মতো)
    public static void Ribbon(Transform parent, string text, float x, float y, float w, float h)
    {
        GoldButton(parent, x, y, w, h, Red);
        TMP_Text t = Label(parent, text, x, y, w, h, 50, Yellow, TextAlignmentOptions.Center);
        t.outlineWidth = 0.25f;
        t.outlineColor = new Color32(110, 20, 20, 255);
    }

    // লেখার ঘর (ভাষার ঘরের মতো)
    public static void Field(Transform parent, float x, float y, float w, float h)
    {
        float r = h * 0.5f;
        Round(parent, "FieldShadow", x, y + 6, w, h, r, new Color(0f, 0f, 0f, 0.3f));
        Round(parent, "FieldEdge", x, y, w, h, r, RowEdge);
        Round(parent, "FieldBack", x + 4, y + 4, w - 8, h - 8, r - 4, BlueDeep);
        Gloss(parent, x + 4, y + 4, w - 8, h - 8, r - 4);
    }

    // % দেখানোর ছোট ঘর
    public static TMP_Text PercentBox(Transform parent, float x, float y, float w, float h)
    {
        float r = h * 0.4f;
        Round(parent, "PctEdge", x, y, w, h, r, Gold);
        Round(parent, "PctBack", x + 4, y + 4, w - 8, h - 8, r - 3, Navy);
        return Label(parent, "70%", x, y, w, h, 40, Color.white, TextAlignmentOptions.Center);
    }

    // ---------- ON/OFF বাটন ----------
    public class Switch
    {
        public System.Action<bool> Changed;
        public bool On = true;

        readonly Image face;
        readonly RectTransform knob;
        readonly RectTransform labelBox;
        readonly TMP_Text label;

        public Switch(Transform parent, float x, float y)
        {
            RectTransform box = Box(parent, "Switch", x, y, 236, 84);
            Round(box, "Shadow", 0, 10, 236, 84, 42, new Color(0f, 0f, 0f, 0.3f));
            Round(box, "Edge", 0, 5, 236, 84, 42, GoldDark);
            Round(box, "Ring", 0, 0, 236, 84, 42, Gold);
            face = Round(box, "Face", 6, 6, 224, 72, 36, Green);
            Gloss(box, 6, 6, 224, 72, 36);

            knob = Box(box, "Knob", 162, 10, 64, 64);
            Round(knob, "KnobShadow", 0, 5, 64, 64, 32, new Color(0f, 0f, 0f, 0.35f));
            RectTransform disc = Box(knob, "Disc", 0, 0, 64, 64);
            Image di = disc.gameObject.AddComponent<Image>();
            di.sprite = UIShapes.Knob(64, new Color32(110, 165, 235, 255), new Color32(244, 250, 255, 255));
            di.raycastTarget = false;

            label = Label(box, "ON", 20, 6, 132, 72, 38, Color.white, TextAlignmentOptions.Center, true);
            labelBox = (RectTransform)label.rectTransform.parent;

            Hit(box, 0, 0, 236, 84, Click);
            Apply();
        }

        void Click()
        {
            On = !On;
            Apply();
            if (Changed != null) Changed(On);
            AudioManager.PlayClick();
        }

        public void Set(bool on)
        {
            On = on;
            Apply();
        }

        void Apply()
        {
            face.color = On ? Green : Row;   // OFF হলে ঘরের রং
            knob.anchoredPosition = new Vector2(On ? 162f : 10f, -10f);
            labelBox.anchoredPosition = new Vector2(On ? 20f : 84f, -6f);
            label.text = On ? "ON" : "OFF";
            label.color = On ? Color.white : new Color32(170, 205, 250, 255);
        }
    }

    // ---------- ভলিউম স্লাইডার ----------
    // টানলে ভরা অংশ কমে বাড়ে, নব সরে। মান ০ থেকে ১।
    public static Slider MakeSlider(Transform parent, float x, float y, float w, Color fillColor)
    {
        float knob = 64f;
        float track = 36f;
        float ty = (knob - track) * 0.5f;

        RectTransform root = Box(parent, "Slider", x, y, w, knob);
        Image hit = root.gameObject.AddComponent<Image>();
        hit.color = new Color(0f, 0f, 0f, 0f);
        hit.raycastTarget = true;
        Slider s = root.gameObject.AddComponent<Slider>();

        // নালা
        Round(root, "TrackEdge", 0, ty - 3, w, track + 6, (track + 6) * 0.5f, RowEdge);
        Round(root, "Track", 4, ty + 1, w - 8, track - 2, (track - 2) * 0.5f, Navy);

        // ভরা অংশ
        RectTransform fillArea = Box(root, "FillArea", 4, ty + 1, w - 8, track - 2);
        GameObject fg = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        RectTransform fillRt = fg.GetComponent<RectTransform>();
        fillRt.SetParent(fillArea, false);
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = new Vector2(0.7f, 1f);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        Image fi = fg.GetComponent<Image>();
        fi.sprite = RoundSprite();
        fi.type = Image.Type.Sliced;
        fi.pixelsPerUnitMultiplier = 32f / Mathf.Max(1f, (track - 2) * 0.5f - 0.5f);
        fi.color = fillColor;
        fi.raycastTarget = false;

        // নব
        RectTransform handleArea = Box(root, "HandleArea", knob * 0.5f, 0, w - knob, knob);
        GameObject hg = new GameObject("Handle", typeof(RectTransform));
        RectTransform hrt = hg.GetComponent<RectTransform>();
        hrt.SetParent(handleArea, false);
        hrt.sizeDelta = new Vector2(knob, 0f);
        Round(hrt, "HandleShadow", 0, 6, knob, knob, knob * 0.5f, new Color(0f, 0f, 0f, 0.35f));
        RectTransform hd = Box(hrt, "HandleDisc", 0, 0, knob, knob);
        Image hi = hd.gameObject.AddComponent<Image>();
        hi.sprite = UIShapes.Knob(64, GoldDark, Yellow);
        hi.raycastTarget = false;

        s.transition = Selectable.Transition.None;
        s.fillRect = fillRt;
        s.handleRect = hrt;
        s.direction = Slider.Direction.LeftToRight;
        s.minValue = 0f;
        s.maxValue = 1f;
        s.wholeNumbers = false;
        s.value = 0.7f;
        return s;
    }
}

// ---------- আঁকা আইকন (কোনো ছবি ফাইল লাগে না) ----------
public static class Icons
{
    static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public static Sprite Get(string name)
    {
        Sprite sp;
        if (cache.TryGetValue(name, out sp) && sp != null) return sp;
        sp = Make(name);
        cache[name] = sp;
        return sp;
    }

    static Sprite Make(string name)
    {
        Painter p = new Painter(128);
        switch (name)
        {
            case "speaker":
                p.RoundRect(14, 46, 40, 82, 5);
                p.Poly(new Vector2(36, 46), new Vector2(70, 22), new Vector2(70, 106), new Vector2(36, 82));
                p.Ring(66, 64, 26, 9, -42, 42);
                p.Ring(66, 64, 46, 9, -42, 42);
                break;

            case "music":
                p.Disc(38, 36, 15);
                p.Disc(88, 28, 15);
                p.Line(51, 38, 51, 100, 9);
                p.Line(101, 30, 101, 92, 9);
                p.Poly(new Vector2(51, 102), new Vector2(101, 94), new Vector2(101, 72), new Vector2(51, 80));
                break;

            case "phone":
                p.RoundRectLine(40, 12, 88, 116, 12, 9);
                p.Line(54, 26, 74, 26, 7);
                p.Ring(64, 64, 36, 8, -32, 32);
                p.Ring(64, 64, 54, 8, -30, 30);
                p.Ring(64, 64, 36, 8, 148, 212);
                p.Ring(64, 64, 54, 8, 150, 210);
                break;

            case "globe":
                p.Ring(64, 64, 46, 9, 0, 360);
                p.Ellipse(64, 64, 20, 46, 7);
                p.Line(18, 64, 110, 64, 7);
                p.Line(30, 92, 98, 92, 6);
                p.Line(30, 36, 98, 36, 6);
                break;

            case "headset":
                p.Ring(64, 64, 42, 10, 0, 180);
                p.RoundRect(12, 34, 36, 84, 8);
                p.RoundRect(92, 34, 116, 84, 8);
                p.Line(110, 36, 104, 16, 7);
                p.Line(104, 16, 76, 14, 7);
                p.Disc(70, 14, 8);
                break;

            case "refresh":
                {
                    float cx = 64f, cy = 64f, r = 38f;
                    p.Ring(cx, cy, r, 12, 35, 325);
                    float th = 325f * Mathf.Deg2Rad;
                    Vector2 e = new Vector2(cx + r * Mathf.Cos(th), cy + r * Mathf.Sin(th));
                    Vector2 dir = new Vector2(-Mathf.Sin(th), Mathf.Cos(th));
                    Vector2 perp = new Vector2(-dir.y, dir.x);
                    p.Poly(e + dir * 22f, e + perp * 20f, e - perp * 20f);
                }
                break;

            case "check":
                p.Line(24, 64, 52, 36, 15);
                p.Line(52, 36, 106, 98, 15);
                break;

            case "x":
                p.Line(30, 30, 98, 98, 15);
                p.Line(30, 98, 98, 30, 15);
                break;

            case "chevron":
                p.Line(44, 24, 86, 64, 15);
                p.Line(44, 104, 86, 64, 15);
                break;

            case "arrow":
                p.Poly(new Vector2(14, 100), new Vector2(114, 100), new Vector2(64, 28));
                break;

            case "play":
                p.Poly(new Vector2(34, 16), new Vector2(34, 112), new Vector2(112, 64));
                break;

            case "back":
                p.Poly(new Vector2(10, 70), new Vector2(58, 112), new Vector2(58, 28));
                p.Line(52, 70, 78, 70, 20);
                p.Ring(78, 40, 30, 20, -75, 90);
                break;

            case "star":
                {
                    float sx = 64f, sy = 62f, ro = 58f, ri = 24f;
                    for (int k = 0; k < 5; k++)
                    {
                        float a0 = (90f + k * 72f - 36f) * Mathf.Deg2Rad;
                        float a1 = (90f + k * 72f) * Mathf.Deg2Rad;
                        float a2 = (90f + k * 72f + 36f) * Mathf.Deg2Rad;
                        p.Poly(new Vector2(sx, sy),
                               new Vector2(sx + ri * Mathf.Cos(a0), sy + ri * Mathf.Sin(a0)),
                               new Vector2(sx + ro * Mathf.Cos(a1), sy + ro * Mathf.Sin(a1)),
                               new Vector2(sx + ri * Mathf.Cos(a2), sy + ri * Mathf.Sin(a2)));
                    }
                    p.Disc(sx, sy, ri);
                }
                break;

            case "trophy":
                p.Poly(new Vector2(26, 114), new Vector2(102, 114), new Vector2(92, 70), new Vector2(36, 70));
                p.Disc(64, 70, 28);
                p.Ring(34, 94, 16, 9, 100, 260);
                p.Ring(94, 94, 16, 9, -80, 80);
                p.Line(64, 46, 64, 28, 14);
                p.RoundRect(34, 10, 94, 28, 6);
                break;

            case "medal":
                p.Poly(new Vector2(30, 124), new Vector2(58, 124), new Vector2(82, 70), new Vector2(54, 70));
                p.Poly(new Vector2(98, 124), new Vector2(70, 124), new Vector2(46, 70), new Vector2(74, 70));
                p.Ring(64, 44, 30, 14, 0, 360);
                p.Disc(64, 44, 13);
                break;

            case "gear":
                p.Disc(64, 64, 32);
                for (int k = 0; k < 8; k++)
                {
                    float a = k * 45f * Mathf.Deg2Rad;
                    float c = Mathf.Cos(a);
                    float s = Mathf.Sin(a);
                    p.Line(64 + 30 * c, 64 + 30 * s, 64 + 50 * c, 64 + 50 * s, 22);
                }
                break;
        }
        return p.ToSprite();
    }

    // 128x128 এর আঁকার খাতা, y ওপরের দিকে বাড়ে
    class Painter
    {
        readonly int S;
        readonly float[] a;

        public Painter(int size)
        {
            S = size;
            a = new float[size * size];
        }

        void Put(int x, int y, float v)
        {
            if (x < 0 || y < 0 || x >= S || y >= S) return;
            int i = y * S + x;
            if (v > a[i]) a[i] = v;
        }

        static float SegDist(float px, float py, float x0, float y0, float x1, float y1)
        {
            float dx = x1 - x0;
            float dy = y1 - y0;
            float len2 = dx * dx + dy * dy;
            float t = len2 > 0f ? Mathf.Clamp01(((px - x0) * dx + (py - y0) * dy) / len2) : 0f;
            float cx = x0 + t * dx;
            float cy = y0 + t * dy;
            return Mathf.Sqrt((px - cx) * (px - cx) + (py - cy) * (py - cy));
        }

        public void Line(float x0, float y0, float x1, float y1, float w)
        {
            float hw = w * 0.5f;
            int minX = Mathf.FloorToInt(Mathf.Min(x0, x1) - hw - 2f);
            int maxX = Mathf.CeilToInt(Mathf.Max(x0, x1) + hw + 2f);
            int minY = Mathf.FloorToInt(Mathf.Min(y0, y1) - hw - 2f);
            int maxY = Mathf.CeilToInt(Mathf.Max(y0, y1) + hw + 2f);
            for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                    Put(x, y, Mathf.Clamp01(hw - SegDist(x + 0.5f, y + 0.5f, x0, y0, x1, y1) + 0.5f));
        }

        public void Disc(float cx, float cy, float r)
        {
            int minX = Mathf.FloorToInt(cx - r - 2f);
            int maxX = Mathf.CeilToInt(cx + r + 2f);
            int minY = Mathf.FloorToInt(cy - r - 2f);
            int maxY = Mathf.CeilToInt(cy + r + 2f);
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    Put(x, y, Mathf.Clamp01(r - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f));
                }
            }
        }

        // আংটি বা চাপ। কোণ ডিগ্রিতে (ডান দিক = ০, ওপর = ৯০)
        public void Ring(float cx, float cy, float r, float w, float a0, float a1)
        {
            float hw = w * 0.5f;
            float span = a1 - a0;
            int minX = Mathf.FloorToInt(cx - r - hw - 2f);
            int maxX = Mathf.CeilToInt(cx + r + hw + 2f);
            int minY = Mathf.FloorToInt(cy - r - hw - 2f);
            int maxY = Mathf.CeilToInt(cy + r + hw + 2f);
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float ang = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
                    bool inside = span >= 360f || Mathf.Repeat(ang - a0, 360f) <= span;
                    if (inside) Put(x, y, Mathf.Clamp01(hw - Mathf.Abs(d - r) + 0.5f));
                }
            }
            if (span < 360f)
            {
                float r0 = a0 * Mathf.Deg2Rad;
                float r1 = a1 * Mathf.Deg2Rad;
                Disc(cx + r * Mathf.Cos(r0), cy + r * Mathf.Sin(r0), hw);
                Disc(cx + r * Mathf.Cos(r1), cy + r * Mathf.Sin(r1), hw);
            }
        }

        // উত্তল বহুভুজ (কোণগুলো ঘুরিয়ে ঘুরিয়ে দিতে হবে)
        public void Poly(params Vector2[] pts)
        {
            int n = pts.Length;
            float area = 0f;
            float minXf = float.MaxValue;
            float maxXf = float.MinValue;
            float minYf = float.MaxValue;
            float maxYf = float.MinValue;
            for (int i = 0; i < n; i++)
            {
                Vector2 p = pts[i];
                Vector2 q = pts[(i + 1) % n];
                area += p.x * q.y - q.x * p.y;
                minXf = Mathf.Min(minXf, p.x);
                maxXf = Mathf.Max(maxXf, p.x);
                minYf = Mathf.Min(minYf, p.y);
                maxYf = Mathf.Max(maxYf, p.y);
            }
            float sign = area >= 0f ? 1f : -1f;
            for (int y = Mathf.FloorToInt(minYf) - 2; y <= Mathf.CeilToInt(maxYf) + 2; y++)
            {
                for (int x = Mathf.FloorToInt(minXf) - 2; x <= Mathf.CeilToInt(maxXf) + 2; x++)
                {
                    float px = x + 0.5f;
                    float py = y + 0.5f;
                    float m = float.MaxValue;
                    for (int i = 0; i < n; i++)
                    {
                        Vector2 p = pts[i];
                        Vector2 q = pts[(i + 1) % n];
                        float ex = q.x - p.x;
                        float ey = q.y - p.y;
                        float len = Mathf.Sqrt(ex * ex + ey * ey);
                        if (len < 0.0001f) continue;
                        float sd = sign * (ex * (py - p.y) - ey * (px - p.x)) / len;
                        m = Mathf.Min(m, sd);
                    }
                    Put(x, y, Mathf.Clamp01(m + 0.5f));
                }
            }
        }

        public void RoundRect(float x0, float y0, float x1, float y1, float r)
        {
            for (int y = Mathf.FloorToInt(y0) - 2; y <= Mathf.CeilToInt(y1) + 2; y++)
            {
                for (int x = Mathf.FloorToInt(x0) - 2; x <= Mathf.CeilToInt(x1) + 2; x++)
                {
                    float px = x + 0.5f;
                    float py = y + 0.5f;
                    float cx = Mathf.Clamp(px, x0 + r, x1 - r);
                    float cy = Mathf.Clamp(py, y0 + r, y1 - r);
                    float d = Mathf.Sqrt((px - cx) * (px - cx) + (py - cy) * (py - cy));
                    Put(x, y, Mathf.Clamp01(r - d + 0.5f));
                }
            }
        }

        // ফাঁপা গোল-কোণা চতুর্ভুজ (শুধু পাড়)
        public void RoundRectLine(float x0, float y0, float x1, float y1, float r, float w)
        {
            float mx = (x0 + x1) * 0.5f;
            float my = (y0 + y1) * 0.5f;
            float hx = (x1 - x0) * 0.5f - r;
            float hy = (y1 - y0) * 0.5f - r;
            for (int y = Mathf.FloorToInt(y0 - w) - 2; y <= Mathf.CeilToInt(y1 + w) + 2; y++)
            {
                for (int x = Mathf.FloorToInt(x0 - w) - 2; x <= Mathf.CeilToInt(x1 + w) + 2; x++)
                {
                    float qx = Mathf.Abs(x + 0.5f - mx) - hx;
                    float qy = Mathf.Abs(y + 0.5f - my) - hy;
                    float ox = Mathf.Max(qx, 0f);
                    float oy = Mathf.Max(qy, 0f);
                    float sd = Mathf.Sqrt(ox * ox + oy * oy) + Mathf.Min(Mathf.Max(qx, qy), 0f) - r;
                    Put(x, y, Mathf.Clamp01(w * 0.5f - Mathf.Abs(sd) + 0.5f));
                }
            }
        }

        public void Ellipse(float cx, float cy, float rx, float ry, float w)
        {
            const int N = 36;
            Vector2 prev = new Vector2(cx + rx, cy);
            for (int i = 1; i <= N; i++)
            {
                float t = i * 2f * Mathf.PI / N;
                Vector2 cur = new Vector2(cx + rx * Mathf.Cos(t), cy + ry * Mathf.Sin(t));
                Line(prev.x, prev.y, cur.x, cur.y, w);
                prev = cur;
            }
        }

        public Sprite ToSprite()
        {
            Texture2D tex = new Texture2D(S, S, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            Color32[] px = new Color32[S * S];
            for (int i = 0; i < px.Length; i++)
                px[i] = new Color32(255, 255, 255, (byte)(Mathf.Clamp01(a[i]) * 255f));
            tex.SetPixels32(px);
            tex.Apply(false, true);
            return Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}