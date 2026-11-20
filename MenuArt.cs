using System.Collections.Generic;
using UnityEngine;

// মেইন মেনুর ৬টা রঙিন আইকন, পুরোটা কোড দিয়ে আঁকা (কোনো ছবি ফাইল লাগে না)।
// MenuArt.Get("quiz") থেকে "map" পর্যন্ত Sprite পাওয়া যায়: quiz, swords, target, trophy, book, map
// প্রতিটা আইকন কয়েকটা আদেশের তালিকা: D = বৃত্ত, O = ডিম্বাকার, P = বহুভুজ, L = রেখা, A = চাপ। রঙ #RRGGBB বা #RRGGBBAA।
public static class MenuArt
{
    const int S = 160;
    static readonly Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();

    public static Sprite Get(string name)
    {
        Sprite sp;
        if (cache.TryGetValue(name, out sp) && sp != null) return sp;
        sp = Build(name);
        cache[name] = sp;
        return sp;
    }

    static Sprite Build(string name)
    {
        string[] cmds;
        switch (name)
        {
            case "quiz": cmds = Quiz; break;
            case "swords": cmds = Swords; break;
            case "target": cmds = Target; break;
            case "trophy": cmds = Trophy; break;
            case "book": cmds = Book; break;
            default: cmds = Map; break;
        }

        Painter p = new Painter();
        for (int i = 0; i < cmds.Length; i++)
        {
            string[] t = cmds[i].Split(' ');
            switch (t[0])
            {
                case "D":
                    p.Disc(F(t[1]), F(t[2]), F(t[3]), Hex(t[4]));
                    break;
                case "O":
                    p.OvalR(F(t[1]), F(t[2]), F(t[3]), F(t[4]), F(t[5]), Hex(t[6]));
                    break;
                case "P":
                    {
                        int n = (t.Length - 2) / 2;
                        Vector2[] pts = new Vector2[n];
                        for (int k = 0; k < n; k++) pts[k] = new Vector2(F(t[2 + 2 * k]), F(t[3 + 2 * k]));
                        p.Poly(Hex(t[1]), pts);
                    }
                    break;
                case "L":
                    p.Line(F(t[1]), F(t[2]), F(t[3]), F(t[4]), F(t[5]), Hex(t[6]));
                    break;
                case "A":
                    p.Arc(F(t[1]), F(t[2]), F(t[3]), F(t[4]), F(t[5]), F(t[6]), Hex(t[7]));
                    break;
            }
        }
        return p.ToSprite();
    }

    static float F(string s)
    {
        return float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
    }

    static Color Hex(string h)
    {
        h = h.TrimStart('#');
        int r = System.Convert.ToInt32(h.Substring(0, 2), 16);
        int g = System.Convert.ToInt32(h.Substring(2, 2), 16);
        int b = System.Convert.ToInt32(h.Substring(4, 2), 16);
        int a = h.Length >= 8 ? System.Convert.ToInt32(h.Substring(6, 2), 16) : 255;
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    // ---------- আইকনের আকার ----------
    static readonly string[] Quiz =
    {
        "O 79.6 15.0 43.2 6.5 0 #00000055",
        "P #A81E2B 27.8 20.7 131.4 20.7 131.4 32.2 27.8 32.2",
        "P #D6303F 30.6 25.0 128.6 25.0 128.6 35.1 30.6 35.1",
        "P #FFFFFF 32.1 32.2 78.2 27.9 78.2 89.8 32.1 94.2",
        "P #FBF6E6 81.0 27.9 127.1 32.2 127.1 94.2 81.0 89.8",
        "L 79.6 27.9 79.6 89.8 2.2 #C9C2AE",
        "L 39.3 79.8 71.0 76.9 2.9 #6C8EC9",
        "L 39.3 71.1 71.0 68.2 2.9 #D86A6A",
        "L 39.3 62.5 65.2 60.3 2.9 #7DBB7A",
        "L 39.3 53.8 69.5 51.7 2.9 #9AA3B8",
        "L 39.3 45.2 62.3 43.8 2.9 #E2A93B",
        "L 88.2 76.9 119.9 79.8 2.9 #6C8EC9",
        "L 88.2 68.2 119.9 71.1 2.9 #D86A6A",
        "L 88.2 59.6 114.2 61.8 2.9 #7DBB7A",
        "L 88.2 51.0 118.5 53.1 2.9 #9AA3B8",
        "L 88.2 42.3 111.3 44.5 2.9 #E2A93B",
        "P #2FBF6A 105.5 94.2 115.6 94.2 115.6 110.0 105.5 110.0",
        "A 97.0 108.0 24.0 13.0 -75 200 #1B4FC8",
        "L 103.2 84.8 95.0 78.0 13.0 #1B4FC8",
        "D 95.0 60.0 8.1 #1B4FC8",
        "A 100.0 112.0 24.0 13.0 -75 200 #3FA0FF",
        "L 106.2 88.8 98.0 82.0 13.0 #3FA0FF",
        "D 98.0 64.0 8.1 #3FA0FF",
    };

    static readonly string[] Swords =
    {
        "O 80 12 52 7 0 #00000044",
        "L 108 52 32 128 17 #AEB9D0",
        "L 108 52 32 128 8 #E9EEF8",
        "P #AEB9D0 41 137 23 119 9 151",
        "L 116 72 88 44 11 #F2B422",
        "L 116 72 88 44 5 #FFE07A",
        "L 108 52 132 28 12 #8A4A24",
        "L 108 52 132 28 5 #B8703A",
        "D 136 24 9 #F2B422",
        "L 52 52 128 128 17 #AEB9D0",
        "L 52 52 128 128 8 #E9EEF8",
        "P #AEB9D0 119 137 137 119 151 151",
        "L 44 72 72 44 11 #F2B422",
        "L 44 72 72 44 5 #FFE07A",
        "L 52 52 28 28 12 #8A4A24",
        "L 52 52 28 28 5 #B8703A",
        "D 24 24 9 #F2B422",
    };

    static readonly string[] Target =
    {
        "O 80 14 46 8 0 #00000055",
        "D 76 76 62 #A9192B",
        "D 76 76 57 #F7F7F7",
        "D 76 76 46 #E32D3B",
        "D 76 76 34 #F7F7F7",
        "D 76 76 23 #E32D3B",
        "D 76 76 11 #F7F7F7",
        "A 76 76 52 5 105 175 #FFFFFF99",
        "L 80 80 128 128 7 #6E768C",
        "L 80 80 128 128 3 #AEB6CC",
        "P #3C78EB 120 120 148 122 134 134",
        "P #3C78EB 120 120 122 148 134 134",
        "D 80 80 4 #444C60",
    };

    static readonly string[] Trophy =
    {
        "O 80 16 42 9 0 #00000055",
        "P #B8780F 46 20 114 20 114 34 46 34",
        "P #F5B91E 54 34 106 34 106 46 54 46",
        "P #E0A010 68 46 92 46 88 72 72 72",
        "A 38 116 22 9 95 265 #F0B21C",
        "A 122 116 22 9 -85 85 #F0B21C",
        "D 80 92 28 #E8A416",
        "P #FCC62A 40 150 120 150 108 92 52 92",
        "D 80 92 27 #FCC62A",
        "O 80 150 40 10 0 #FFE27A",
        "O 80 150 34 6.5 0 #C8860F",
        "P #FFF0B0AA 50 144 62 144 64 100 54 104",
    };

    static readonly string[] Book =
    {
        "O 80.0 18.0 60.0 9.0 0 #00000055",
        "P #A81E2B 8.0 26.0 152.0 26.0 152.0 42.0 8.0 42.0",
        "P #D6303F 12.0 32.0 148.0 32.0 148.0 46.0 12.0 46.0",
        "P #FFFFFF 14.0 42.0 78.0 36.0 78.0 122.0 14.0 128.0",
        "P #FBF6E6 82.0 36.0 146.0 42.0 146.0 128.0 82.0 122.0",
        "L 80.0 36.0 80.0 122.0 3.0 #C9C2AE",
        "L 24.0 108.0 68.0 104.0 4.0 #6C8EC9",
        "L 24.0 96.0 68.0 92.0 4.0 #D86A6A",
        "L 24.0 84.0 60.0 81.0 4.0 #7DBB7A",
        "L 24.0 72.0 66.0 69.0 4.0 #9AA3B8",
        "L 24.0 60.0 56.0 58.0 4.0 #E2A93B",
        "L 92.0 104.0 136.0 108.0 4.0 #6C8EC9",
        "L 92.0 92.0 136.0 96.0 4.0 #D86A6A",
        "L 92.0 80.0 128.0 83.0 4.0 #7DBB7A",
        "L 92.0 68.0 134.0 71.0 4.0 #9AA3B8",
        "L 92.0 56.0 124.0 59.0 4.0 #E2A93B",
        "P #2FBF6A 116.0 128.0 130.0 128.0 130.0 150.0 116.0 150.0",
    };

    static readonly string[] Map =
    {
        "O 80 14 56 8 0 #00000055",
        "P #FFFFFF 18 36 62 46 62 124 18 114",
        "P #5DB0EE 22 40 62 50 62 118 22 108",
        "P #7CCB6A 62 50 100 40 100 108 62 118",
        "P #4C9FE8 100 40 138 50 138 118 100 108",
        "L 62 50 62 118 3 #00000022",
        "L 100 40 100 108 3 #00000022",
        "O 42 78 12 7 20 #6FCB63",
        "O 120 82 11 8 -20 #6FCB63",
        "O 82 84 14 9 0 #58B24E",
        "P #E02D3C 100 104 128 104 114 68",
        "D 114 112 16 #E02D3C",
        "D 114 112 6 #FFFFFF",
        "D 108 118 4 #FF8E98",
    };

    // ---------- আঁকার খাতা (y ওপরের দিকে বাড়ে) ----------
    class Painter
    {
        readonly float[] R = new float[S * S];
        readonly float[] G = new float[S * S];
        readonly float[] B = new float[S * S];
        readonly float[] A = new float[S * S];

        void Blend(int x, int y, float cov, Color c)
        {
            if (x < 0 || y < 0 || x >= S || y >= S) return;
            float sa = Mathf.Clamp01(cov) * c.a;
            if (sa <= 0f) return;
            int i = y * S + x;
            float da = A[i];
            float oa = sa + da * (1f - sa);
            R[i] = (c.r * sa + R[i] * da * (1f - sa)) / oa;
            G[i] = (c.g * sa + G[i] * da * (1f - sa)) / oa;
            B[i] = (c.b * sa + B[i] * da * (1f - sa)) / oa;
            A[i] = oa;
        }

        public void Disc(float cx, float cy, float r, Color c)
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
                    Blend(x, y, r - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f, c);
                }
            }
        }

        public void OvalR(float cx, float cy, float rx, float ry, float ang, Color c)
        {
            float a = ang * Mathf.Deg2Rad;
            float ca = Mathf.Cos(a);
            float sa = Mathf.Sin(a);
            float big = Mathf.Max(rx, ry);
            float small = Mathf.Min(rx, ry);
            int minX = Mathf.FloorToInt(cx - big - 2f);
            int maxX = Mathf.CeilToInt(cx + big + 2f);
            int minY = Mathf.FloorToInt(cy - big - 2f);
            int maxY = Mathf.CeilToInt(cy + big + 2f);
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float u = dx * ca + dy * sa;
                    float v = -dx * sa + dy * ca;
                    float d = Mathf.Sqrt((u / rx) * (u / rx) + (v / ry) * (v / ry));
                    Blend(x, y, (1f - d) * small + 0.5f, c);
                }
            }
        }

        // উত্তল বহুভুজ
        public void Poly(Color c, Vector2[] pts)
        {
            int n = pts.Length;
            float area = 0f;
            float minXf = float.MaxValue, maxXf = float.MinValue, minYf = float.MaxValue, maxYf = float.MinValue;
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
                        m = Mathf.Min(m, sign * (ex * (py - p.y) - ey * (px - p.x)) / len);
                    }
                    Blend(x, y, m + 0.5f, c);
                }
            }
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

        public void Line(float x0, float y0, float x1, float y1, float w, Color c)
        {
            float hw = w * 0.5f;
            int minX = Mathf.FloorToInt(Mathf.Min(x0, x1) - hw - 2f);
            int maxX = Mathf.CeilToInt(Mathf.Max(x0, x1) + hw + 2f);
            int minY = Mathf.FloorToInt(Mathf.Min(y0, y1) - hw - 2f);
            int maxY = Mathf.CeilToInt(Mathf.Max(y0, y1) + hw + 2f);
            for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                    Blend(x, y, hw - SegDist(x + 0.5f, y + 0.5f, x0, y0, x1, y1) + 0.5f, c);
        }

        // চাপ। কোণ ডিগ্রিতে (ডান দিক = ০, ওপর = ৯০)
        public void Arc(float cx, float cy, float r, float w, float a0, float a1, Color c)
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
                    if (Mathf.Repeat(ang - a0, 360f) <= span)
                        Blend(x, y, hw - Mathf.Abs(d - r) + 0.5f, c);
                }
            }
            Disc(cx + r * Mathf.Cos(a0 * Mathf.Deg2Rad), cy + r * Mathf.Sin(a0 * Mathf.Deg2Rad), hw, c);
            Disc(cx + r * Mathf.Cos(a1 * Mathf.Deg2Rad), cy + r * Mathf.Sin(a1 * Mathf.Deg2Rad), hw, c);
        }

        public Sprite ToSprite()
        {
            Texture2D tex = new Texture2D(S, S, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            Color32[] px = new Color32[S * S];
            for (int i = 0; i < px.Length; i++)
            {
                px[i] = new Color32((byte)(Mathf.Clamp01(R[i]) * 255f), (byte)(Mathf.Clamp01(G[i]) * 255f),
                                    (byte)(Mathf.Clamp01(B[i]) * 255f), (byte)(Mathf.Clamp01(A[i]) * 255f));
            }
            tex.SetPixels32(px);
            tex.Apply(false, true);
            return Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}