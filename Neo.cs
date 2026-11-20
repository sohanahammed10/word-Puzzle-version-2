using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// আধুনিক ৩ডি ধাঁচের মেনু আঁকার সাহায্যকারী (কাচের মতো স্বচ্ছ ঘর, ছায়া, আলো, চকচকে আইকন)।
// এখানে কিছু বদলাতে হবে না। UIKit আর UIShapes স্ক্রিপ্টও লাগে।
public static class Neo
{
    const int N = 128;   // আইকনের খাতার মাপ

    public struct Style
    {
        public float radius;
        public Color top, bottom;
        public Color edgeTop, edgeBottom;
        public float edgeW;
        public float gloss;
        public float shadowA, shadowBlur, shadowDy;
        public float scale;      // ছবির খুঁটিনাটি (বেশি হলে পরিষ্কার, কিন্তু ভারী)
        public bool inner;       // ভেতরে উপরে ছায়া (নালার মতো ঘরের জন্য)
    }

    static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    static string Hex(Color c)
    {
        return ColorUtility.ToHtmlStringRGBA(c);
    }

    static Sprite Cached(string key)
    {
        Sprite s;
        if (cache.TryGetValue(key, out s) && s != null) return s;
        return null;
    }

    public static Color C(int r, int g, int b, float a = 1f)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a);
    }

    public static Color Mix(Color accent, float k, float add)
    {
        return new Color(
            Mathf.Clamp01(accent.r * k + add / 255f),
            Mathf.Clamp01(accent.g * k + add / 255f),
            Mathf.Clamp01(accent.b * k + add / 255f), 1f);
    }

    // ---------------------------------------------------------------- ঘর (Surface)
    // x, y = বাম-উপর কোণ থেকে দূরত্ব। ছায়ার জন্য চারদিকে একটু বেশি জায়গা নেয়।
    public static Image Surface(Transform parent, string name, float x, float y, float w, float h, Style st)
    {
        int pad = st.shadowA > 0f ? Mathf.CeilToInt(st.shadowBlur * 1.1f + st.shadowDy) : 2;
        int iw = Mathf.RoundToInt(w);
        int ih = Mathf.RoundToInt(h);
        string key = "S" + iw + "x" + ih + "p" + pad + "r" + st.radius + Hex(st.top) + Hex(st.bottom)
                     + Hex(st.edgeTop) + Hex(st.edgeBottom) + st.edgeW + "g" + st.gloss
                     + "s" + st.shadowA + "/" + st.shadowBlur + "/" + st.shadowDy + "x" + st.scale + st.inner;
        Sprite sp = Cached(key);
        if (sp == null)
        {
            sp = BakeSurface(iw, ih, pad, st, Vector4.zero);
            cache[key] = sp;
        }
        RectTransform rt = UIKit.Box(parent, name, x - pad, y - pad, iw + 2 * pad, ih + 2 * pad);
        Image im = rt.gameObject.AddComponent<Image>();
        im.sprite = sp;
        im.raycastTarget = false;
        return im;
    }

    static float SD(float x, float y, float w, float h, float r)
    {
        float qx = Mathf.Abs(x - w * 0.5f) - (w * 0.5f - r);
        float qy = Mathf.Abs(y - h * 0.5f) - (h * 0.5f - r);
        float ox = Mathf.Max(qx, 0f);
        float oy = Mathf.Max(qy, 0f);
        return Mathf.Sqrt(ox * ox + oy * oy) + Mathf.Min(Mathf.Max(qx, qy), 0f) - r;
    }

    static Sprite BakeSurface(int w, int h, int pad, Style st, Vector4 border)
    {
        float sc = st.scale;
        int tw = Mathf.Max(4, Mathf.RoundToInt((w + 2 * pad) * sc));
        int th = Mathf.Max(4, Mathf.RoundToInt((h + 2 * pad) * sc));
        Color32[] px = new Color32[tw * th];
        float r = st.radius;

        for (int y = 0; y < th; y++)
        {
            float Y = (y + 0.5f) / sc - pad;
            float t = Mathf.Clamp01(Y / h);
            for (int x = 0; x < tw; x++)
            {
                float X = (x + 0.5f) / sc - pad;
                float s = SD(X, Y, w, h, r);
                float fc = Mathf.Clamp01(0.5f - s);

                Color col = Color.Lerp(st.bottom, st.top, t);
                float g = st.gloss * Mathf.Pow(Mathf.Clamp01((t - 0.5f) / 0.5f), 1.5f);
                col.r = Mathf.Lerp(col.r, 1f, g * 0.5f);
                col.g = Mathf.Lerp(col.g, 1f, g * 0.5f);
                col.b = Mathf.Lerp(col.b, 1f, g * 0.5f);
                col.a = Mathf.Clamp01(col.a + g * 0.25f);

                if (st.inner)
                {
                    float k = 1f - 0.405f * t;
                    col.r *= k;
                    col.g *= k;
                    col.b *= k;
                }

                if (st.edgeW > 0f)
                {
                    float ring = Mathf.Clamp01(s + st.edgeW + 0.5f);
                    Color ec = Color.Lerp(st.edgeBottom, st.edgeTop, t);
                    float k = ring * ec.a;
                    col.r = Mathf.Lerp(col.r, ec.r, k);
                    col.g = Mathf.Lerp(col.g, ec.g, k);
                    col.b = Mathf.Lerp(col.b, ec.b, k);
                    col.a = Mathf.Max(col.a, k * 0.9f);
                }

                float fa = fc * col.a;
                float sa = 0f;
                if (st.shadowA > 0f)
                {
                    float ss = SD(X, Y + st.shadowDy, w, h, r);
                    float sh = Mathf.Clamp01(1f - (ss + st.shadowBlur * 0.25f) / st.shadowBlur);
                    sa = st.shadowA * sh * sh * (1f - fc);
                }

                float oa = fa + sa * (1f - fa);
                int i = y * tw + x;
                if (oa < 0.0001f)
                {
                    px[i] = new Color32(0, 0, 0, 0);
                    continue;
                }
                float rr = col.r * fa / oa;
                float gg = col.g * fa / oa;
                float bb = col.b * fa / oa;
                px[i] = new Color32((byte)(Mathf.Clamp01(rr) * 255f), (byte)(Mathf.Clamp01(gg) * 255f),
                                    (byte)(Mathf.Clamp01(bb) * 255f), (byte)(Mathf.Clamp01(oa) * 255f));
            }
        }

        Texture2D tex = new Texture2D(tw, th, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.SetPixels32(px);
        tex.Apply(false, true);
        return Sprite.Create(tex, new Rect(0, 0, tw, th), new Vector2(0.5f, 0.5f), 100f * sc, 0, SpriteMeshType.FullRect, border);
    }

    // ---------------------------------------------------------------- প্রগ্রেস বার
    // রঙিন ভরা অংশ বা অন্ধকার নালা। চওড়া বদলালেও দুই মাথা গোলই থাকে।
    public static Image Bar(Transform parent, string name, float x, float y, float w, float h,
                            Color top, Color bottom, float gloss, bool inner)
    {
        int ih = Mathf.RoundToInt(h);
        Style st = new Style();
        st.radius = ih * 0.5f;
        st.top = top;
        st.bottom = bottom;
        st.edgeTop = top;
        st.edgeBottom = bottom;
        st.edgeW = 0f;
        st.gloss = gloss;
        st.scale = 3f;
        st.inner = inner;

        string key = "B" + ih + Hex(top) + Hex(bottom) + gloss + inner;
        Sprite sp = Cached(key);
        if (sp == null)
        {
            float b = ih * 0.5f * 3f;
            sp = BakeSurface(ih * 4, ih, 0, st, new Vector4(b, 0f, b, 0f));
            cache[key] = sp;
        }
        RectTransform rt = UIKit.Box(parent, name, x, y, w, h);
        Image im = rt.gameObject.AddComponent<Image>();
        im.sprite = sp;
        im.type = Image.Type.Sliced;
        im.raycastTarget = false;
        return im;
    }

    // ---------------------------------------------------------------- নরম আলোর দাগ
    public static Image Glow(Transform parent, string name, float x, float y, float d, Color c, float alpha)
    {
        string key = "G" + Hex(c) + alpha;
        Sprite sp = Cached(key);
        if (sp == null)
        {
            const int n = 128;
            Color32[] px = new Color32[n * n];
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    float dx = (i + 0.5f) / n * 2f - 1f;
                    float dy = (j + 0.5f) / n * 2f - 1f;
                    float r = Mathf.Sqrt(dx * dx + dy * dy);
                    float a = Mathf.Clamp01(1f - r);
                    a = a * a * alpha;
                    px[j * n + i] = new Color32((byte)(c.r * 255f), (byte)(c.g * 255f), (byte)(c.b * 255f), (byte)(a * 255f));
                }
            }
            Texture2D tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.SetPixels32(px);
            tex.Apply(false, true);
            sp = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), 100f);
            cache[key] = sp;
        }
        RectTransform rt = UIKit.Box(parent, name, x, y, d, d);
        Image im = rt.gameObject.AddComponent<Image>();
        im.sprite = sp;
        im.raycastTarget = false;
        return im;
    }

    // ওপর থেকে নিচে রং বদলানো পুরো পর্দার পটভূমি
    public static Sprite VerticalGradient(Color top, Color bottom)
    {
        Texture2D tex = new Texture2D(2, 256, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        Color32[] px = new Color32[2 * 256];
        for (int y = 0; y < 256; y++)
        {
            Color c = Color.Lerp(bottom, top, y / 255f);
            for (int x = 0; x < 2; x++) px[y * 2 + x] = c;
        }
        tex.SetPixels32(px);
        tex.Apply(false, true);
        return Sprite.Create(tex, new Rect(0, 0, 2, 256), new Vector2(0.5f, 0.5f), 100f);
    }

    // ---------------------------------------------------------------- ৩ডি আইকন
    public static Image Icon(Transform parent, float x, float y, float size, string name, Color top, Color bottom)
    {
        string key = "I" + name + Hex(top) + Hex(bottom);
        Sprite sp = Cached(key);
        if (sp == null)
        {
            sp = BakeIcon(name, top, bottom);
            cache[key] = sp;
        }
        RectTransform rt = UIKit.Box(parent, "Icon_" + name, x, y, size, size);
        Image im = rt.gameObject.AddComponent<Image>();
        im.sprite = sp;
        im.preserveAspect = true;
        im.raycastTarget = false;
        return im;
    }

    static Vector2 V(float x, float y)
    {
        return new Vector2(x, y);
    }

    // আইকনের আকার আঁকার খাতা (y ওপরের দিকে বাড়ে)
    class Mask
    {
        public readonly float[] a = new float[N * N];

        void Px(int x, int y, float cov, bool erase)
        {
            if (x < 0 || y < 0 || x >= N || y >= N) return;
            float c = Mathf.Clamp01(cov);
            int i = y * N + x;
            if (erase)
            {
                float keep = 1f - c;
                if (keep < a[i]) a[i] = keep;
            }
            else if (c > a[i])
            {
                a[i] = c;
            }
        }

        public void Disc(float cx, float cy, float r, bool erase = false)
        {
            for (int y = Mathf.FloorToInt(cy - r - 2f); y <= Mathf.CeilToInt(cy + r + 2f); y++)
            {
                for (int x = Mathf.FloorToInt(cx - r - 2f); x <= Mathf.CeilToInt(cx + r + 2f); x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    Px(x, y, r - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f, erase);
                }
            }
        }

        static float SegDist(float px, float py, float x0, float y0, float x1, float y1)
        {
            float dx = x1 - x0;
            float dy = y1 - y0;
            float l2 = dx * dx + dy * dy;
            float t = l2 > 0f ? Mathf.Clamp01(((px - x0) * dx + (py - y0) * dy) / l2) : 0f;
            float cx = x0 + t * dx;
            float cy = y0 + t * dy;
            return Mathf.Sqrt((px - cx) * (px - cx) + (py - cy) * (py - cy));
        }

        public void Line(float x0, float y0, float x1, float y1, float w, bool erase = false)
        {
            float hw = w * 0.5f;
            for (int y = Mathf.FloorToInt(Mathf.Min(y0, y1) - hw - 2f); y <= Mathf.CeilToInt(Mathf.Max(y0, y1) + hw + 2f); y++)
                for (int x = Mathf.FloorToInt(Mathf.Min(x0, x1) - hw - 2f); x <= Mathf.CeilToInt(Mathf.Max(x0, x1) + hw + 2f); x++)
                    Px(x, y, hw - SegDist(x + 0.5f, y + 0.5f, x0, y0, x1, y1) + 0.5f, erase);
        }

        public void Ring(float cx, float cy, float r, float w, float a0, float a1)
        {
            float hw = w * 0.5f;
            float span = a1 - a0;
            for (int y = Mathf.FloorToInt(cy - r - hw - 2f); y <= Mathf.CeilToInt(cy + r + hw + 2f); y++)
            {
                for (int x = Mathf.FloorToInt(cx - r - hw - 2f); x <= Mathf.CeilToInt(cx + r + hw + 2f); x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float ang = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
                    if (span >= 360f || Mathf.Repeat(ang - a0, 360f) <= span)
                        Px(x, y, hw - Mathf.Abs(d - r) + 0.5f, false);
                }
            }
            if (span < 360f)
            {
                Disc(cx + r * Mathf.Cos(a0 * Mathf.Deg2Rad), cy + r * Mathf.Sin(a0 * Mathf.Deg2Rad), hw);
                Disc(cx + r * Mathf.Cos(a1 * Mathf.Deg2Rad), cy + r * Mathf.Sin(a1 * Mathf.Deg2Rad), hw);
            }
        }

        // উত্তল বহুভুজ
        public void Poly(params Vector2[] pts)
        {
            int n = pts.Length;
            float area = 0f;
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            for (int i = 0; i < n; i++)
            {
                Vector2 p = pts[i];
                Vector2 q = pts[(i + 1) % n];
                area += p.x * q.y - q.x * p.y;
                minX = Mathf.Min(minX, p.x);
                maxX = Mathf.Max(maxX, p.x);
                minY = Mathf.Min(minY, p.y);
                maxY = Mathf.Max(maxY, p.y);
            }
            float sign = area >= 0f ? 1f : -1f;
            for (int y = Mathf.FloorToInt(minY) - 2; y <= Mathf.CeilToInt(maxY) + 2; y++)
            {
                for (int x = Mathf.FloorToInt(minX) - 2; x <= Mathf.CeilToInt(maxX) + 2; x++)
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
                        m = Mathf.Min(m, sign * (ex * (py - p.y) - ey * (px - p.x)) / len);
                    }
                    Px(x, y, m + 0.5f, false);
                }
            }
        }

        // ফাঁপা গোল-কোণা চতুর্ভুজ (শুধু পাড়)
        public void RRectLine(float x0, float y0, float x1, float y1, float r, float w)
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
                    Px(x, y, w * 0.5f - Mathf.Abs(sd) + 0.5f, false);
                }
            }
        }

        public void Ellipse(float cx, float cy, float rx, float ry, float w)
        {
            Vector2 prev = new Vector2(cx + rx, cy);
            for (int i = 1; i <= 36; i++)
            {
                float t = i * 2f * Mathf.PI / 36f;
                Vector2 cur = new Vector2(cx + rx * Mathf.Cos(t), cy + ry * Mathf.Sin(t));
                Line(prev.x, prev.y, cur.x, cur.y, w);
                prev = cur;
            }
        }

        public void RRect(float x0, float y0, float x1, float y1, float r)
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
                    Px(x, y, r - d + 0.5f, false);
                }
            }
        }
    }

    static float[] Shape(string name)
    {
        Mask p = new Mask();
        switch (name)
        {
            case "star":
                {
                    float sx = 64f, sy = 62f, ro = 58f, ri = 24f;
                    for (int k = 0; k < 5; k++)
                    {
                        float a0 = (90f + k * 72f - 36f) * Mathf.Deg2Rad;
                        float a1 = (90f + k * 72f) * Mathf.Deg2Rad;
                        float a2 = (90f + k * 72f + 36f) * Mathf.Deg2Rad;
                        p.Poly(V(sx, sy),
                               V(sx + ri * Mathf.Cos(a0), sy + ri * Mathf.Sin(a0)),
                               V(sx + ro * Mathf.Cos(a1), sy + ro * Mathf.Sin(a1)),
                               V(sx + ri * Mathf.Cos(a2), sy + ri * Mathf.Sin(a2)));
                    }
                    p.Disc(sx, sy, ri);
                }
                break;

            case "medal":
                p.Poly(V(30, 124), V(58, 124), V(82, 70), V(54, 70));
                p.Poly(V(98, 124), V(70, 124), V(46, 70), V(74, 70));
                p.Ring(64, 44, 30, 14, 0, 360);
                p.Disc(64, 44, 13);
                break;

            case "trophy":
                p.Poly(V(26, 114), V(102, 114), V(92, 70), V(36, 70));
                p.Disc(64, 70, 28);
                p.Ring(34, 94, 16, 9, 100, 260);
                p.Ring(94, 94, 16, 9, -80, 80);
                p.Line(64, 46, 64, 28, 14);
                p.RRect(34, 10, 94, 28, 6);
                break;

            case "crown":
                p.Poly(V(16, 40), V(112, 40), V(112, 74), V(16, 74));
                p.Poly(V(16, 70), V(16, 108), V(48, 70));
                p.Poly(V(44, 70), V(64, 116), V(84, 70));
                p.Poly(V(80, 70), V(112, 108), V(112, 70));
                p.Disc(16, 108, 8);
                p.Disc(64, 116, 9);
                p.Disc(112, 108, 8);
                p.RRect(14, 18, 114, 46, 8);
                break;

            case "flame":
                p.Disc(64, 44, 34);
                p.Poly(V(33, 56), V(95, 56), V(72, 122));
                p.Poly(V(30, 50), V(64, 50), V(50, 96));
                p.Disc(72, 90, 12);
                break;

            case "bolt":
                p.Poly(V(76, 122), V(24, 58), V(62, 58));
                p.Poly(V(24, 58), V(100, 58), V(84, 76), V(30, 76));
                p.Poly(V(52, 70), V(104, 70), V(50, 6));
                p.Poly(V(62, 58), V(76, 122), V(96, 70), V(62, 70));
                break;

            case "diamond":
                p.Poly(V(34, 104), V(94, 104), V(116, 74), V(12, 74));
                p.Poly(V(12, 74), V(116, 74), V(64, 14));
                p.Line(12, 74, 116, 74, 3, true);
                p.Line(40, 104, 52, 74, 3, true);
                p.Line(88, 104, 76, 74, 3, true);
                p.Line(52, 74, 64, 14, 3, true);
                p.Line(76, 74, 64, 14, 3, true);
                break;

            case "lock":
                p.RRect(24, 12, 104, 70, 12);
                p.Ring(64, 68, 26, 13, 0, 180);
                p.Disc(64, 46, 8, true);
                p.Line(64, 46, 64, 28, 8, true);
                break;

            case "speaker":
                p.RRect(14, 46, 40, 82, 5);
                p.Poly(V(36, 46), V(70, 22), V(70, 106), V(36, 82));
                p.Ring(66, 64, 26, 9, -42, 42);
                p.Ring(66, 64, 46, 9, -42, 42);
                break;

            case "music":
                p.Disc(38, 36, 15);
                p.Disc(88, 28, 15);
                p.Line(51, 38, 51, 100, 9);
                p.Line(101, 30, 101, 92, 9);
                p.Poly(V(51, 102), V(101, 94), V(101, 72), V(51, 80));
                break;

            case "phone":
                p.RRectLine(40, 12, 88, 116, 12, 9);
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
                p.RRect(12, 34, 36, 84, 8);
                p.RRect(92, 34, 116, 84, 8);
                p.Line(110, 36, 104, 16, 7);
                p.Line(104, 16, 76, 14, 7);
                p.Disc(70, 14, 8);
                break;

            case "refresh":
                {
                    float th = 325f * Mathf.Deg2Rad;
                    p.Ring(64, 64, 38, 12, 35, 325);
                    Vector2 e = V(64f + 38f * Mathf.Cos(th), 64f + 38f * Mathf.Sin(th));
                    Vector2 d = V(-Mathf.Sin(th), Mathf.Cos(th));
                    Vector2 pp = V(-d.y, d.x);
                    p.Poly(e + d * 22f, e + pp * 20f, e - pp * 20f);
                }
                break;

            case "chevron":
                p.Line(44, 24, 86, 64, 15);
                p.Line(44, 104, 86, 64, 15);
                break;

            case "arrow":
                p.Poly(V(14, 100), V(114, 100), V(64, 28));
                break;

            case "gear":
                p.Disc(64, 64, 32);
                for (int k = 0; k < 8; k++)
                {
                    float a = k * 45f * Mathf.Deg2Rad;
                    p.Line(64f + 30f * Mathf.Cos(a), 64f + 30f * Mathf.Sin(a),
                           64f + 50f * Mathf.Cos(a), 64f + 50f * Mathf.Sin(a), 22);
                }
                p.Disc(64, 64, 12, true);
                break;

            case "coin":
                p.Disc(64, 64, 54);
                p.Disc(64, 64, 42, true);
                p.Disc(64, 64, 35);
                break;

            case "book":
                p.Poly(V(8, 102), V(60, 92), V(60, 20), V(8, 30));
                p.Poly(V(120, 102), V(68, 92), V(68, 20), V(120, 30));
                for (int yy = 84; yy >= 48; yy -= 18)
                {
                    p.Line(20, yy, 52, yy - 5, 4, true);
                    p.Line(76, yy - 5, 108, yy, 4, true);
                }
                break;

            case "pencil":
                p.Poly(V(42.5f, 25.5f), V(100.5f, 83.5f), V(83.5f, 100.5f), V(25.5f, 42.5f));
                p.Poly(V(25.5f, 42.5f), V(42.5f, 25.5f), V(12, 12));
                p.Line(76, 104, 104, 76, 3, true);
                break;

            case "user":
                p.Disc(64, 88, 26);
                p.Disc(64, 14, 46);
                break;

            case "check":
                p.Line(24, 64, 52, 36, 16);
                p.Line(52, 36, 106, 98, 16);
                break;

            case "x":
                p.Line(32, 32, 96, 96, 16);
                p.Line(32, 96, 96, 32, 16);
                break;
        }
        return p.a;
    }

    static float At(float[] m, int x, int y)
    {
        if (x < 0 || y < 0 || x >= N || y >= N) return 0f;
        return m[y * N + x];
    }

    static float[] Shift(float[] m, int dx, int dy)
    {
        float[] o = new float[N * N];
        for (int y = 0; y < N; y++)
            for (int x = 0; x < N; x++)
                o[y * N + x] = At(m, x - dx, y - dy);
        return o;
    }

    static float[] BoxBlur(float[] src, int r)
    {
        int k = 2 * r + 1;
        float[] tmp = new float[N * N];
        float[] dst = new float[N * N];
        for (int y = 0; y < N; y++)
        {
            for (int x = 0; x < N; x++)
            {
                float sum = 0f;
                for (int d = -r; d <= r; d++) sum += src[y * N + Mathf.Clamp(x + d, 0, N - 1)];
                tmp[y * N + x] = sum / k;
            }
        }
        for (int y = 0; y < N; y++)
        {
            for (int x = 0; x < N; x++)
            {
                float sum = 0f;
                for (int d = -r; d <= r; d++) sum += tmp[Mathf.Clamp(y + d, 0, N - 1) * N + x];
                dst[y * N + x] = sum / k;
            }
        }
        return dst;
    }

    // আকারটাকে ৩ডি রূপ দেয়: গ্রেডিয়েন্ট রং, আলো-ছায়ার ধার, ওপরে চকচক, ধারে গাঢ় রেখা, নিচে ছায়া
    static Sprite BakeIcon(string name, Color top, Color bottom)
    {
        float[] m = Shape(name);
        float[] sh = BoxBlur(Shift(m, 3, -5), 4);
        float[] b2 = BoxBlur(m, 3);
        int[] ox = { 2, -2, 0, 0, 1, -1, 1, -1 };
        int[] oy = { 0, 0, 2, -2, 1, -1, -1, 1 };

        Color32[] px = new Color32[N * N];
        for (int y = 0; y < N; y++)
        {
            float t = y / (N - 1f);
            for (int x = 0; x < N; x++)
            {
                int i = y * N + x;
                float mi = m[i];

                float cr = Mathf.Lerp(bottom.r, top.r, t);
                float cg = Mathf.Lerp(bottom.g, top.g, t);
                float cb = Mathf.Lerp(bottom.b, top.b, t);

                float gx = At(b2, x + 1, y) - At(b2, x - 1, y);
                float gy = At(b2, x, y + 1) - At(b2, x, y - 1);
                float lit = (gx * 0.5f - gy * 0.7f) * 2.4f;
                float pos = Mathf.Clamp01(lit);
                float neg = Mathf.Clamp01(-lit);
                cr = Mathf.Lerp(cr, 1f, pos * 0.9f);
                cg = Mathf.Lerp(cg, 1f, pos * 0.9f);
                cb = Mathf.Lerp(cb, 1f, pos * 0.9f);
                cr *= 1f - neg * 0.55f;
                cg *= 1f - neg * 0.55f;
                cb *= 1f - neg * 0.55f;

                float shine = Mathf.Clamp01((t - 0.55f) / 0.45f) * 0.22f;
                cr = Mathf.Lerp(cr, 1f, shine);
                cg = Mathf.Lerp(cg, 1f, shine);
                cb = Mathf.Lerp(cb, 1f, shine);

                float mn = mi;
                for (int k = 0; k < 8; k++) mn = Mathf.Min(mn, At(m, x - ox[k], y - oy[k]));
                float edge = Mathf.Clamp01(mi - mn);
                float dark = 1f - 0.4f * edge;
                cr *= dark;
                cg *= dark;
                cb *= dark;

                float sa = sh[i] * 0.5f;
                float oa = mi + sa * (1f - mi);
                if (oa < 0.0001f)
                {
                    px[i] = new Color32(0, 0, 0, 0);
                    continue;
                }
                float k2 = mi / oa;
                px[i] = new Color32((byte)(Mathf.Clamp01(cr * k2) * 255f), (byte)(Mathf.Clamp01(cg * k2) * 255f),
                                    (byte)(Mathf.Clamp01(cb * k2) * 255f), (byte)(Mathf.Clamp01(oa) * 255f));
            }
        }

        Texture2D tex = new Texture2D(N, N, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.SetPixels32(px);
        tex.Apply(false, true);
        return Sprite.Create(tex, new Rect(0, 0, N, N), new Vector2(0.5f, 0.5f), 100f);
    }
}