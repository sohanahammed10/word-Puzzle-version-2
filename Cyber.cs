using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// সাইবারপাংক ৩ডি ধাঁচের মেনু আঁকার সাহায্যকারী (কাটা কোণা, নিয়ন আভা, গভীরতা)।
// এখানে কিছু বদলাতে হবে না। Neo, UIKit, UIShapes স্ক্রিপ্টও লাগে।
public static class Cyber
{
    // ---------- নিয়ন রং ----------
    public static readonly Color Cyan = new Color32(0, 235, 255, 255);
    public static readonly Color Magenta = new Color32(255, 40, 190, 255);
    public static readonly Color Green = new Color32(60, 255, 150, 255);
    public static readonly Color Yellow = new Color32(255, 225, 60, 255);
    public static readonly Color Violet = new Color32(150, 90, 255, 255);

    public struct Spec
    {
        public float chamfer;                 // কাটা কোণার মাপ
        public Color top, bottom;             // ভেতরের রং (ওপর থেকে নিচ)
        public Color edgeA, edgeB;            // নিয়ন ধারের রং (বাম-উপর থেকে ডান-নিচ)
        public float edgeW;                   // ধারের মোটা
        public float glowA, glowR;            // বাইরের আভা
        public float depth;                   // ৩ডি গভীরতা (নিচে-ডানে ঠেলে বের হওয়া অংশ)
        public float scan;                    // আড়াআড়ি হালকা দাগ
        public float scale;                   // ছবির খুঁটিনাটি
        public int pad;                       // আভার জন্য বাড়তি জায়গা
        public bool cutTL, cutBR;             // কোন কোণ কাটা
    }

    static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    static string Hex(Color c)
    {
        return ColorUtility.ToHtmlStringRGBA(c);
    }

    public static Color C(int r, int g, int b, float a = 1f)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a);
    }

    public static Spec Make(float chamfer, Color top, Color bottom, Color edgeA, Color edgeB, float edgeW,
                            float glowA, float depth, float scan, float scale)
    {
        Spec s = new Spec();
        s.chamfer = chamfer;
        s.top = top;
        s.bottom = bottom;
        s.edgeA = edgeA;
        s.edgeB = edgeB;
        s.edgeW = edgeW;
        s.glowA = glowA;
        s.glowR = 10f;
        s.depth = depth;
        s.scan = scan;
        s.scale = scale;
        s.pad = 36;
        s.cutTL = true;
        s.cutBR = true;
        return s;
    }

    // ---------------------------------------------------------------- ঘর (Panel)
    public static Image Panel(Transform parent, string name, float x, float y, float w, float h, Spec sp)
    {
        int iw = Mathf.RoundToInt(w);
        int ih = Mathf.RoundToInt(h);
        string key = "P" + iw + "x" + ih + "c" + sp.chamfer + Hex(sp.top) + Hex(sp.bottom) + Hex(sp.edgeA)
                     + Hex(sp.edgeB) + sp.edgeW + "g" + sp.glowA + "/" + sp.glowR + "d" + sp.depth
                     + "s" + sp.scan + "x" + sp.scale + "p" + sp.pad + sp.cutTL + sp.cutBR;
        Sprite sprite;
        if (!cache.TryGetValue(key, out sprite) || sprite == null)
        {
            sprite = Bake(iw, ih, sp);
            cache[key] = sprite;
        }
        RectTransform rt = UIKit.Box(parent, name, x - sp.pad, y - sp.pad, iw + 2 * sp.pad, ih + 2 * sp.pad);
        Image im = rt.gameObject.AddComponent<Image>();
        im.sprite = sprite;
        im.raycastTarget = false;
        return im;
    }

    // দুই কোণ কাটা চতুর্ভুজের দূরত্ব (ভেতরে ঋণাত্মক)
    static float SD(float X, float Y, float w, float h, float c, bool cutTL, bool cutBR)
    {
        float u = X - w * 0.5f;
        float v = Y - h * 0.5f;
        float hw = w * 0.5f;
        float hh = h * 0.5f;
        float qx = Mathf.Abs(u) - hw;
        float qy = Mathf.Abs(v) - hh;
        float ox = Mathf.Max(qx, 0f);
        float oy = Mathf.Max(qy, 0f);
        float s = Mathf.Sqrt(ox * ox + oy * oy) + Mathf.Min(Mathf.Max(qx, qy), 0f);
        const float r2 = 0.70710678f;
        if (cutTL) s = Mathf.Max(s, (-u + v - (hw + hh - c)) * r2);
        if (cutBR) s = Mathf.Max(s, (u - v - (hw + hh - c)) * r2);
        return s;
    }

    static Sprite Bake(int w, int h, Spec sp)
    {
        float sc = sp.scale;
        int pad = sp.pad;
        int tw = Mathf.Max(4, Mathf.RoundToInt((w + 2 * pad) * sc));
        int th = Mathf.Max(4, Mathf.RoundToInt((h + 2 * pad) * sc));
        Color32[] px = new Color32[tw * th];

        for (int y = 0; y < th; y++)
        {
            float Y = (y + 0.5f) / sc - pad;
            float t = Mathf.Clamp01(Y / h);
            float scanRow = (Mathf.FloorToInt(Y / 3f) & 1);
            for (int x = 0; x < tw; x++)
            {
                float X = (x + 0.5f) / sc - pad;
                float s = SD(X, Y, w, h, sp.chamfer, sp.cutTL, sp.cutBR);
                float fc = Mathf.Clamp01(0.5f - s);

                Color col = Color.Lerp(sp.bottom, sp.top, t);
                float shine = Mathf.Clamp01((t - 0.55f) / 0.45f) * 0.10f;
                col.r = Mathf.Lerp(col.r, 1f, shine);
                col.g = Mathf.Lerp(col.g, 1f, shine);
                col.b = Mathf.Lerp(col.b, 1f, shine);
                if (sp.scan > 0f)
                {
                    float k = 1f - sp.scan * scanRow;
                    col.r *= k;
                    col.g *= k;
                    col.b *= k;
                }

                float u = X / w - 0.5f;
                float v = -(Y / h - 0.5f);
                float tt = Mathf.Clamp01(0.5f + 0.5f * (u + v));
                Color ec = Color.Lerp(sp.edgeA, sp.edgeB, tt);
                float ring = Mathf.Clamp01(s + sp.edgeW + 0.5f);
                col.r = Mathf.Lerp(col.r, ec.r, ring);
                col.g = Mathf.Lerp(col.g, ec.g, ring);
                col.b = Mathf.Lerp(col.b, ec.b, ring);
                col.a = Mathf.Max(col.a, ring);

                float inner = s < 0f ? Mathf.Clamp01(1f - (-s) / 14f) : 0f;
                col.r = Mathf.Lerp(col.r, ec.r, inner * 0.18f);
                col.g = Mathf.Lerp(col.g, ec.g, inner * 0.18f);
                col.b = Mathf.Lerp(col.b, ec.b, inner * 0.18f);

                float fa = fc * col.a;

                float gl = 0f;
                if (s > 0f && sp.glowA > 0f) gl = Mathf.Exp(-s / sp.glowR) * sp.glowA;

                float a2 = 0f;
                if (sp.depth > 0f)
                {
                    float s2 = SD(X - sp.depth * 0.5f, Y + sp.depth, w, h, sp.chamfer, sp.cutTL, sp.cutBR);
                    a2 = Mathf.Clamp01(0.5f - s2) * (1f - fc) * 0.95f;
                }

                float outA = a2 + gl * (1f - a2);
                float rr = 0f, gg = 0f, bb = 0f;
                if (outA > 0.000001f)
                {
                    rr = (ec.r * 0.35f * a2 + ec.r * gl * (1f - a2)) / outA;
                    gg = (ec.g * 0.35f * a2 + ec.g * gl * (1f - a2)) / outA;
                    bb = (ec.b * 0.35f * a2 + ec.b * gl * (1f - a2)) / outA;
                }

                float outA2 = fa + outA * (1f - fa);
                int i = y * tw + x;
                if (outA2 < 0.0001f)
                {
                    px[i] = new Color32(0, 0, 0, 0);
                    continue;
                }
                float r3 = (col.r * fa + rr * outA * (1f - fa)) / outA2;
                float g3 = (col.g * fa + gg * outA * (1f - fa)) / outA2;
                float b3 = (col.b * fa + bb * outA * (1f - fa)) / outA2;
                px[i] = new Color32((byte)(Mathf.Clamp01(r3) * 255f), (byte)(Mathf.Clamp01(g3) * 255f),
                                    (byte)(Mathf.Clamp01(b3) * 255f), (byte)(Mathf.Clamp01(outA2) * 255f));
            }
        }

        Texture2D tex = new Texture2D(tw, th, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.SetPixels32(px);
        tex.Apply(false, true);
        return Sprite.Create(tex, new Rect(0, 0, tw, th), new Vector2(0.5f, 0.5f), 100f * sc);
    }

    // ---------------------------------------------------------------- পটভূমি (গ্রিড)
    public static Image Backdrop(Transform parent, string name)
    {
        Sprite sprite;
        if (!cache.TryGetValue("BACK", out sprite) || sprite == null)
        {
            const int tw = 540;
            const int th = 960;
            Color32[] px = new Color32[tw * th];
            Color top = C(10, 6, 28, 0.95f);
            Color bot = C(22, 6, 44, 0.97f);
            for (int y = 0; y < th; y++)
            {
                Color bg = Color.Lerp(bot, top, y / (th - 1f));
                bool hLine = (y % 45) == 0;
                for (int x = 0; x < tw; x++)
                {
                    Color c = bg;
                    if ((x % 45) == 0) c = Over(c, new Color(0f, 235f / 255f, 1f, 0.10f));
                    if (hLine) c = Over(c, new Color(1f, 40f / 255f, 190f / 255f, 0.085f));
                    px[y * tw + x] = c;
                }
            }
            Texture2D tex = new Texture2D(tw, th, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.SetPixels32(px);
            tex.Apply(false, true);
            sprite = Sprite.Create(tex, new Rect(0, 0, tw, th), new Vector2(0.5f, 0.5f), 100f);
            cache["BACK"] = sprite;
        }
        RectTransform rt = UIShapes.Stretch(parent, name);
        Image im = rt.gameObject.AddComponent<Image>();
        im.sprite = sprite;
        im.raycastTarget = true;
        return im;
    }

    static Color Over(Color under, Color over)
    {
        float a = over.a + under.a * (1f - over.a);
        if (a < 0.0001f) return new Color(0, 0, 0, 0);
        return new Color(
            (over.r * over.a + under.r * under.a * (1f - over.a)) / a,
            (over.g * over.a + under.g * under.a * (1f - over.a)) / a,
            (over.b * over.a + under.b * under.a * (1f - over.a)) / a, a);
    }

    // ---------------------------------------------------------------- নিয়ন লেখা
    // সাদা লেখা, চারপাশে নিয়ন রঙের রেখা
    public static TMP_Text NeonLabel(Transform parent, string text, float x, float y, float w, float h,
                                     float size, Color neon, TextAlignmentOptions align, float spacing = 0f)
    {
        TMP_Text t = UIKit.Label(parent, text, x, y, w, h, size, Color.white, align);
        t.outlineWidth = 0.22f;
        t.outlineColor = (Color32)neon;
        t.characterSpacing = spacing;
        return t;
    }

    // ---------------------------------------------------------------- আইকন ব্যাজ
    public static void Badge(Transform parent, float x, float y, float d, string icon, Color neon, Color iconTop, Color iconBottom)
    {
        Spec sp = Make(d * 0.28f, C(18, 12, 46, 0.97f), C(8, 6, 24, 0.97f), neon, neon, 3f, 0.55f, 6f, 0f, 1f);
        Panel(parent, "Badge", x, y, d, d, sp);
        Neo.Icon(parent, x + d * 0.18f, y + d * 0.16f, d * 0.64f, icon, iconTop, iconBottom);
    }

    // ---------------------------------------------------------------- ON/OFF বাটন
    public class Toggle
    {
        public System.Action<bool> Changed;
        public bool On = true;

        readonly GameObject onGroup;
        readonly GameObject offGroup;
        readonly RectTransform knob;

        public Toggle(Transform parent, float x, float y)
        {
            RectTransform box = UIKit.Box(parent, "Toggle", x, y, 168, 76);

            RectTransform onBox = UIKit.Box(box, "On", 0, 0, 168, 76);
            onGroup = onBox.gameObject;
            Spec on = Make(18f, C(40, 220, 130, 0.95f), C(10, 120, 70, 0.95f), Green, Green, 3f, 0.7f, 6f, 0f, 1f);
            Panel(onBox, "Face", 0, 0, 168, 76, on);
            NeonLabel(onBox, "ON", 10, 0, 100, 76, 28, Green, TextAlignmentOptions.Center);

            RectTransform offBox = UIKit.Box(box, "Off", 0, 0, 168, 76);
            offGroup = offBox.gameObject;
            Color dim = C(120, 130, 190);
            Spec off = Make(18f, C(20, 18, 50, 0.95f), C(10, 8, 28, 0.95f), dim, dim, 2.5f, 0.15f, 6f, 0f, 1f);
            Panel(offBox, "Face", 0, 0, 168, 76, off);
            UIKit.Label(offBox, "OFF", 58, 0, 100, 76, 26, C(150, 160, 215), TextAlignmentOptions.Center);

            Spec k = Make(14f, Color.white, C(170, 180, 230), Color.white, C(190, 200, 255), 2f, 0.3f, 4f, 0f, 1f);
            knob = Panel(box, "Knob", 8, 8, 60, 60, k).rectTransform;

            UIKit.Hit(box, 0, 0, 168, 76, Click);
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
            onGroup.SetActive(On);
            offGroup.SetActive(!On);
            // কাটা কোণার ঘরে নবের জায়গা: ON এ ডানে, OFF এ বাঁয়ে
            float kx = (On ? 168f - 66f : 8f) - 36f;
            knob.anchoredPosition = new Vector2(kx, -(8f - 36f));
        }
    }

    // ---------------------------------------------------------------- ভলিউম স্লাইডার
    // মান ০ থেকে ১। টানলে ভরা অংশ বাড়ে-কমে, নব সরে।
    public static Slider MakeSlider(Transform parent, float x, float y, float w, Color neon, Color light, Color dark)
    {
        float th = 34f;
        float knobW = 30f;
        float knobH = 58f;
        float ty = (knobH - th) * 0.5f;

        RectTransform root = UIKit.Box(parent, "Slider", x, y, w, knobH);
        Image hit = root.gameObject.AddComponent<Image>();
        hit.color = new Color(0f, 0f, 0f, 0f);
        hit.raycastTarget = true;
        Slider s = root.gameObject.AddComponent<Slider>();

        // নালা
        Color rail = C(90, 100, 170);
        Spec trackSpec = Make(10f, C(10, 8, 30, 0.97f), C(20, 16, 50, 0.97f), rail, rail, 2f, 0f, 0f, 0f, 1f);
        Panel(root, "Track", 0, ty, w, th, trackSpec);

        // ভরা অংশ (পাশে আভা ছাড়া, যাতে ভরার হিসাব ঠিক থাকে)
        float fw = w - 12f;
        float fh = th - 12f;
        Spec fillSpec = Make(7f, light, dark, light, neon, 1.5f, 0f, 0f, 0f, 1f);
        fillSpec.pad = 0;
        RectTransform fillArea = UIKit.Box(root, "FillArea", 6, ty + 6, fw, fh);
        Image fill = Panel(fillArea, "Fill", 0, 0, fw, fh, fillSpec);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.fillAmount = 0.7f;

        // Slider নিজে ভরা অংশের মাপ জোর করে "পুরো ঘর জুড়ে" ধরে নেয়, তাই এখানেও পুরো ঘর জুড়ে রাখতে হয়
        // (নাহলে ভরা অংশ ঘরের ডবল বড় হয়ে বাইরে বেরিয়ে যায়)
        RectTransform fr = fill.rectTransform;
        fr.anchorMin = Vector2.zero;
        fr.anchorMax = Vector2.one;
        fr.offsetMin = Vector2.zero;
        fr.offsetMax = Vector2.zero;

        // নব
        RectTransform handleArea = UIKit.Box(root, "HandleArea", knobW * 0.5f, 0, w - knobW, knobH);
        GameObject hg = new GameObject("Handle", typeof(RectTransform));
        RectTransform hrt = hg.GetComponent<RectTransform>();
        hrt.SetParent(handleArea, false);
        hrt.sizeDelta = new Vector2(knobW, 0f);
        Spec knobSpec = Make(9f, Color.white, C(170, 180, 230), Color.white, neon, 2f, 0.55f, 5f, 0f, 1f);
        Panel(hrt, "Knob", 0, 0, knobW, knobH, knobSpec);

        s.transition = Selectable.Transition.None;
        s.fillRect = fill.rectTransform;
        s.handleRect = hrt;
        s.direction = Slider.Direction.LeftToRight;
        s.minValue = 0f;
        s.maxValue = 1f;
        s.wholeNumbers = false;
        s.value = 0.7f;
        return s;
    }
}