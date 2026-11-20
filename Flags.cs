using UnityEngine;

// ৪৩টা দেশের পতাকা, পুরোটা কোড দিয়ে আঁকা (৩ডি চকচকে ঢেউ-ঢেউ ভাব)। কোনো ছবি ফাইল লাগে না।
//   Flags.Count          = কয়টা পতাকা
//   Flags.Name(i)        = দেশের নাম
//   Flags.Get(i)         = পতাকার ছবি (Sprite), মাপ ২৩২ x ১৬৮ (ছায়া সহ)
// ProfileInfo এ ব্যবহারকারীর দেশ আর বয়স সেভ থাকে।
public static class Flags
{
    public const int Count = 43;

    static readonly string[] Names =
    {
        "Bangladesh", "India", "Pakistan", "Nepal", "Malaysia", "Singapore", "Indonesia", "Japan",
        "China", "South Korea", "Thailand", "Vietnam", "Philippines", "Saudi Arabia", "UAE", "Turkey",
        "Egypt", "Nigeria", "United Kingdom", "Ireland", "France", "Germany", "Italy", "Spain",
        "Portugal", "Netherlands", "Belgium", "Switzerland", "Sweden", "Norway", "Denmark",
        "Finland", "Poland", "Ukraine", "Russia", "Greece", "United States", "Canada", "Brazil",
        "Argentina", "Mexico", "Australia", "New Zealand"
    };

    static readonly Sprite[] cache = new Sprite[Count];

    public static string Name(int index)
    {
        if (index < 0 || index >= Count) return "";
        return Names[index];
    }

    public static Sprite Get(int index)
    {
        index = Mathf.Clamp(index, 0, Count - 1);
        if (cache[index] == null)
        {
            Fl f = new Fl();
            Draw(index, f);
            cache[index] = f.Finish();
        }
        return cache[index];
    }

    // ---------- রং ----------
    static Color Col(int r, int g, int b, float a = 1f)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a);
    }

    static readonly Color White = new Color(1f, 1f, 1f, 1f);
    static readonly Color Black = new Color(0f, 0f, 0f, 1f);
    static readonly Color Yellow = new Color(1f, 222f / 255f, 0f, 1f);

    // ---------- সাধারণ সাহায্যকারী ----------
    static void BgH(Fl f, Color[] colors, float[] weights = null)
    {
        int n = colors.Length;
        float tot = 0f;
        for (int i = 0; i < n; i++) tot += weights == null ? 1f : weights[i];
        float y = Fl.H;
        for (int i = 0; i < n; i++)
        {
            float h = Fl.H * (weights == null ? 1f : weights[i]) / tot;
            f.Rect(0, y - h, Fl.W, y, colors[i]);
            y -= h;
        }
    }

    static void BgV(Fl f, Color[] colors, float[] weights = null)
    {
        int n = colors.Length;
        float tot = 0f;
        for (int i = 0; i < n; i++) tot += weights == null ? 1f : weights[i];
        float x = 0f;
        for (int i = 0; i < n; i++)
        {
            float w = Fl.W * (weights == null ? 1f : weights[i]) / tot;
            f.Rect(x, 0, x + w, Fl.H, colors[i]);
            x += w;
        }
    }

    static void Crescent(Fl f, float cx, float cy, float r, Color c, Color bg, float off, float r2)
    {
        f.Disc(cx, cy, r, c);
        f.Disc(cx + r * off, cy, r * r2, bg);
    }

    static Color[] Repeat(Color a, Color b, int pairs)
    {
        Color[] arr = new Color[pairs * 2];
        for (int i = 0; i < pairs; i++)
        {
            arr[i * 2] = a;
            arr[i * 2 + 1] = b;
        }
        return arr;
    }

    // ---------- পতাকাগুলো ----------
    static void Draw(int index, Fl f)
    {
        switch (index)
        {
            case 0: // Bangladesh
                f.Rect(0, 0, Fl.W, Fl.H, Col(0, 106, 78));
                f.Disc(84, 64, 38, Col(244, 42, 65));
                break;

            case 1: // India
                BgH(f, new[] { Col(255, 153, 51), White, Col(19, 136, 8) });
                f.Ring(96, 64, 17, 3, Col(0, 0, 128));
                for (int k = 0; k < 24; k++)
                {
                    float a = k * 15f * Mathf.Deg2Rad;
                    f.Line(96, 64, 96 + 16 * Mathf.Cos(a), 64 + 16 * Mathf.Sin(a), 1.2f, Col(0, 0, 128));
                }
                f.Disc(96, 64, 3, Col(0, 0, 128));
                break;

            case 2: // Pakistan
                {
                    Color g = Col(1, 65, 28);
                    f.Rect(0, 0, Fl.W, Fl.H, g);
                    f.Rect(0, 0, 48, Fl.H, White);
                    Crescent(f, 118, 64, 34, White, g, 0.30f, 0.80f);
                    f.Star(142, 82, 11, White, 40f);
                }
                break;

            case 3: // Nepal
                {
                    f.Rect(0, 0, Fl.W, Fl.H, Col(214, 226, 245));
                    Color blue = Col(0, 56, 147);
                    Color red = Col(220, 20, 60);
                    f.Poly(blue, new Vector2(26, 124), new Vector2(26, 52), new Vector2(146, 52));
                    f.Poly(blue, new Vector2(26, 74), new Vector2(26, 4), new Vector2(146, 4));
                    f.Poly(red, new Vector2(34, 112), new Vector2(34, 60), new Vector2(122, 60));
                    f.Poly(red, new Vector2(34, 66), new Vector2(34, 14), new Vector2(122, 14));
                    f.Disc(52, 88, 6, White);
                    f.Disc(52, 34, 8, White);
                }
                break;

            case 4: // Malaysia
                {
                    BgH(f, Repeat(Col(204, 0, 1), White, 7));
                    Color blue = Col(0, 0, 102);
                    f.Rect(0, Fl.H - 8f * Fl.H / 14f, 100, Fl.H, blue);
                    Crescent(f, 40, 92, 24, Col(255, 204, 0), blue, 0.26f, 0.80f);
                    f.Star(68, 92, 15, Col(255, 204, 0), 90f);
                }
                break;

            case 5: // Singapore
                {
                    Color red = Col(237, 41, 57);
                    BgH(f, new[] { red, White });
                    Crescent(f, 36, 92, 24, White, red, 0.28f, 0.80f);
                    for (int k = 0; k < 5; k++)
                    {
                        float a = (90f + k * 72f) * Mathf.Deg2Rad;
                        f.Star(66 + 16 * Mathf.Cos(a), 92 + 16 * Mathf.Sin(a), 6, White, 90f);
                    }
                }
                break;

            case 6: // Indonesia
                BgH(f, new[] { Col(255, 0, 0), White });
                break;

            case 7: // Japan
                f.Rect(0, 0, Fl.W, Fl.H, White);
                f.Disc(96, 64, 38, Col(188, 0, 45));
                break;

            case 8: // China
                f.Rect(0, 0, Fl.W, Fl.H, Col(222, 41, 16));
                f.Star(38, 92, 28, Yellow, 90f);
                f.Star(78, 112, 8, Yellow, 90f);
                f.Star(90, 98, 8, Yellow, 90f);
                f.Star(90, 80, 8, Yellow, 90f);
                f.Star(78, 66, 8, Yellow, 90f);
                break;

            case 9: // South Korea
                {
                    f.Rect(0, 0, Fl.W, Fl.H, White);
                    Color red = Col(205, 46, 58);
                    Color blue = Col(0, 71, 160);
                    f.Half(96, 64, 30, red, true);
                    f.Half(96, 64, 30, blue, false);
                    f.Disc(81, 64, 15, red);
                    f.Disc(111, 64, 15, blue);
                    float[] px = { 34, 158, 34, 158 };
                    float[] py = { 100, 100, 28, 28 };
                    float[] rot = { 45, -45, -45, 45 };
                    for (int g = 0; g < 4; g++)
                    {
                        float a = rot[g] * Mathf.Deg2Rad;
                        for (int k = -8; k <= 8; k += 8)
                        {
                            float ox = -Mathf.Sin(a) * k;
                            float oy = Mathf.Cos(a) * k;
                            f.Line(px[g] + ox - Mathf.Cos(a) * 12, py[g] + oy - Mathf.Sin(a) * 12,
                                   px[g] + ox + Mathf.Cos(a) * 12, py[g] + oy + Mathf.Sin(a) * 12, 4, Black);
                        }
                    }
                }
                break;

            case 10: // Thailand
                BgH(f, new[] { Col(165, 25, 49), White, Col(45, 42, 74), White, Col(165, 25, 49) },
                    new float[] { 1, 1, 2, 1, 1 });
                break;

            case 11: // Vietnam
                f.Rect(0, 0, Fl.W, Fl.H, Col(218, 37, 29));
                f.Star(96, 66, 36, Col(255, 255, 0), 90f);
                break;

            case 12: // Philippines
                BgH(f, new[] { Col(0, 56, 168), Col(206, 17, 38) });
                f.Poly(White, new Vector2(0, 0), new Vector2(0, Fl.H), new Vector2(90, 64));
                f.Disc(28, 64, 9, Col(252, 209, 22));
                f.Star(10, 104, 6, Col(252, 209, 22), 90f);
                f.Star(10, 24, 6, Col(252, 209, 22), 90f);
                f.Star(68, 64, 6, Col(252, 209, 22), 90f);
                break;

            case 13: // Saudi Arabia
                f.Rect(0, 0, Fl.W, Fl.H, Col(0, 106, 52));
                f.Line(54, 84, 138, 84, 7, White);
                f.Line(46, 46, 146, 46, 5, White);
                f.Poly(White, new Vector2(108, 40), new Vector2(150, 40), new Vector2(150, 52), new Vector2(112, 52));
                break;

            case 14: // UAE
                BgH(f, new[] { Col(0, 115, 47), White, Black });
                f.Rect(0, 0, 48, Fl.H, Col(206, 17, 38));
                break;

            case 15: // Turkey
                {
                    Color red = Col(227, 10, 23);
                    f.Rect(0, 0, Fl.W, Fl.H, red);
                    Crescent(f, 78, 64, 34, White, red, 0.34f, 0.80f);
                    f.Star(124, 64, 14, White, 0f);
                }
                break;

            case 16: // Egypt
                BgH(f, new[] { Col(206, 17, 38), White, Black });
                f.Disc(96, 64, 11, Col(193, 154, 42));
                break;

            case 17: // Nigeria
                BgV(f, new[] { Col(0, 135, 81), White, Col(0, 135, 81) });
                break;

            case 18: // United Kingdom
                {
                    Color red = Col(200, 16, 46);
                    f.Rect(0, 0, Fl.W, Fl.H, Col(1, 33, 105));
                    f.Line(0, 0, Fl.W, Fl.H, 24, White);
                    f.Line(0, Fl.H, Fl.W, 0, 24, White);
                    f.Line(0, 0, Fl.W, Fl.H, 8, red);
                    f.Line(0, Fl.H, Fl.W, 0, 8, red);
                    f.Rect(74, 0, 118, Fl.H, White);
                    f.Rect(0, 42, Fl.W, 86, White);
                    f.Rect(83, 0, 109, Fl.H, red);
                    f.Rect(0, 51, Fl.W, 77, red);
                }
                break;

            case 19: // Ireland
                BgV(f, new[] { Col(22, 155, 98), White, Col(255, 136, 62) });
                break;

            case 20: // France
                BgV(f, new[] { Col(0, 85, 164), White, Col(239, 65, 53) });
                break;

            case 21: // Germany
                BgH(f, new[] { Black, Col(221, 0, 0), Col(255, 206, 0) });
                break;

            case 22: // Italy
                BgV(f, new[] { Col(0, 146, 70), White, Col(206, 43, 55) });
                break;

            case 23: // Spain
                BgH(f, new[] { Col(170, 21, 27), Col(241, 191, 0), Col(170, 21, 27) }, new float[] { 1, 2, 1 });
                f.Rect(54, 46, 76, 82, Col(173, 21, 18));
                break;

            case 24: // Portugal
                BgV(f, new[] { Col(0, 102, 0), Col(255, 0, 0) }, new float[] { 2, 3 });
                f.Disc(76, 64, 22, Col(255, 220, 0));
                f.Disc(76, 64, 12, Col(255, 0, 0));
                f.Disc(76, 64, 6, White);
                break;

            case 25: // Netherlands
                BgH(f, new[] { Col(174, 28, 40), White, Col(33, 70, 139) });
                break;

            case 26: // Belgium
                BgV(f, new[] { Black, Col(253, 218, 36), Col(239, 51, 64) });
                break;

            case 27: // Switzerland
                f.Rect(0, 0, Fl.W, Fl.H, Col(218, 41, 28));
                f.Rect(80, 22, 112, 106, White);
                f.Rect(44, 48, 148, 80, White);
                break;

            case 28: // Sweden
                f.Rect(0, 0, Fl.W, Fl.H, Col(0, 106, 167));
                f.Rect(52, 0, 82, Fl.H, Col(254, 204, 0));
                f.Rect(0, 49, Fl.W, 79, Col(254, 204, 0));
                break;

            case 29: // Norway
                f.Rect(0, 0, Fl.W, Fl.H, Col(186, 12, 47));
                f.Rect(46, 0, 90, Fl.H, White);
                f.Rect(0, 42, Fl.W, 86, White);
                f.Rect(56, 0, 80, Fl.H, Col(0, 32, 91));
                f.Rect(0, 52, Fl.W, 76, Col(0, 32, 91));
                break;

            case 30: // Denmark
                f.Rect(0, 0, Fl.W, Fl.H, Col(198, 12, 48));
                f.Rect(54, 0, 84, Fl.H, White);
                f.Rect(0, 49, Fl.W, 79, White);
                break;

            case 31: // Finland
                f.Rect(0, 0, Fl.W, Fl.H, White);
                f.Rect(52, 0, 84, Fl.H, Col(0, 53, 128));
                f.Rect(0, 48, Fl.W, 80, Col(0, 53, 128));
                break;

            case 32: // Poland
                BgH(f, new[] { White, Col(220, 20, 60) });
                break;

            case 33: // Ukraine
                BgH(f, new[] { Col(0, 87, 183), Col(255, 215, 0) });
                break;

            case 34: // Russia
                BgH(f, new[] { White, Col(0, 57, 166), Col(213, 43, 30) });
                break;

            case 35: // Greece
                {
                    Color blue = Col(13, 94, 175);
                    Color[] st = new Color[9];
                    for (int i = 0; i < 9; i++) st[i] = (i % 2 == 0) ? blue : White;
                    BgH(f, st);
                    float sh = Fl.H * 5f / 9f;
                    f.Rect(0, Fl.H - sh, 76, Fl.H, blue);
                    f.Rect(30, Fl.H - sh, 46, Fl.H, White);
                    f.Rect(0, Fl.H - sh / 2f - 8f, 76, Fl.H - sh / 2f + 8f, White);
                }
                break;

            case 36: // United States
                {
                    Color[] st = new Color[13];
                    for (int i = 0; i < 13; i++) st[i] = (i % 2 == 0) ? Col(178, 34, 52) : White;
                    BgH(f, st);
                    float ch = Fl.H * 7f / 13f;
                    f.Rect(0, Fl.H - ch, 84, Fl.H, Col(60, 59, 110));
                    for (int r = 0; r < 5; r++)
                        for (int c = 0; c < 6; c++)
                            f.Disc(8 + c * 13.5f, Fl.H - 9 - r * 12, 2.6f, White);
                }
                break;

            case 37: // Canada
                {
                    Color red = Col(216, 6, 33);
                    BgV(f, new[] { red, White, red }, new float[] { 1, 2, 1 });
                    f.Star(96, 64, 30, red, 90f);
                    f.Rect(93, 20, 99, 44, red);
                }
                break;

            case 38: // Brazil
                f.Rect(0, 0, Fl.W, Fl.H, Col(0, 155, 58));
                f.Poly(Col(255, 223, 0), new Vector2(96, 116), new Vector2(172, 64), new Vector2(96, 12), new Vector2(20, 64));
                f.Disc(96, 64, 27, Col(0, 39, 118));
                f.Line(74, 60, 118, 72, 5, White);
                break;

            case 39: // Argentina
                {
                    Color lb = Col(116, 172, 223);
                    BgH(f, new[] { lb, White, lb });
                    f.Disc(96, 64, 12, Col(246, 180, 14));
                }
                break;

            case 40: // Mexico
                BgV(f, new[] { Col(0, 104, 71), White, Col(206, 17, 38) });
                f.Disc(96, 64, 20, Col(150, 108, 50));
                f.Disc(96, 64, 10, Col(0, 104, 71));
                break;

            case 41: // Australia
                f.Rect(0, 0, Fl.W, Fl.H, Col(0, 0, 139));
                Canton(f);
                f.Star(48, 34, 18, White, 90f);
                f.Star(140, 104, 8, White, 90f);
                f.Star(160, 74, 8, White, 90f);
                f.Star(124, 64, 8, White, 90f);
                f.Star(146, 38, 8, White, 90f);
                break;

            default: // New Zealand
                f.Rect(0, 0, Fl.W, Fl.H, Col(0, 36, 125));
                Canton(f);
                f.Star(140, 100, 10, Col(204, 20, 43), 90f);
                f.Star(120, 66, 10, Col(204, 20, 43), 90f);
                f.Star(160, 66, 10, Col(204, 20, 43), 90f);
                f.Star(140, 30, 10, Col(204, 20, 43), 90f);
                break;
        }
    }

    // অস্ট্রেলিয়া আর নিউজিল্যান্ডের কোণার ছোট ইউনিয়ন জ্যাক (সহজ রূপ)
    static void Canton(Fl f)
    {
        Color red = Col(200, 16, 46);
        f.Rect(0, 64, 96, Fl.H, Col(1, 33, 105));
        f.Rect(42, 64, 54, Fl.H, White);
        f.Rect(0, 90, 96, 102, White);
        f.Rect(45, 64, 51, Fl.H, red);
        f.Rect(0, 93, 96, 99, red);
    }

    // ---------- আঁকার খাতা ----------
    class Fl
    {
        public const int W = 192;
        public const int H = 128;

        readonly float[] R = new float[W * H];
        readonly float[] G = new float[W * H];
        readonly float[] B = new float[W * H];
        readonly float[] A = new float[W * H];

        void Blend(int x, int y, float cov, Color c)
        {
            if (x < 0 || y < 0 || x >= W || y >= H) return;
            float sa = Mathf.Clamp01(cov) * c.a;
            if (sa <= 0f) return;
            int i = y * W + x;
            float da = A[i];
            float oa = sa + da * (1f - sa);
            R[i] = (c.r * sa + R[i] * da * (1f - sa)) / oa;
            G[i] = (c.g * sa + G[i] * da * (1f - sa)) / oa;
            B[i] = (c.b * sa + B[i] * da * (1f - sa)) / oa;
            A[i] = oa;
        }

        public void Rect(float x0, float y0, float x1, float y1, Color c)
        {
            for (int y = Mathf.FloorToInt(y0) - 1; y <= Mathf.CeilToInt(y1) + 1; y++)
            {
                for (int x = Mathf.FloorToInt(x0) - 1; x <= Mathf.CeilToInt(x1) + 1; x++)
                {
                    float px = x + 0.5f;
                    float py = y + 0.5f;
                    float cov = Mathf.Min(Mathf.Min(px - x0, x1 - px), Mathf.Min(py - y0, y1 - py)) + 0.5f;
                    Blend(x, y, cov, c);
                }
            }
        }

        public void Disc(float cx, float cy, float r, Color c)
        {
            for (int y = Mathf.FloorToInt(cy - r) - 2; y <= Mathf.CeilToInt(cy + r) + 2; y++)
            {
                for (int x = Mathf.FloorToInt(cx - r) - 2; x <= Mathf.CeilToInt(cx + r) + 2; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    Blend(x, y, r - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f, c);
                }
            }
        }

        public void Half(float cx, float cy, float r, Color c, bool up)
        {
            float sgn = up ? 1f : -1f;
            for (int y = Mathf.FloorToInt(cy - r) - 2; y <= Mathf.CeilToInt(cy + r) + 2; y++)
            {
                for (int x = Mathf.FloorToInt(cx - r) - 2; x <= Mathf.CeilToInt(cx + r) + 2; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float cov = Mathf.Clamp01(r - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
                    float side = Mathf.Clamp01((y + 0.5f - cy) * sgn + 0.5f);
                    Blend(x, y, cov * side, c);
                }
            }
        }

        public void Ring(float cx, float cy, float r, float w, Color c)
        {
            for (int y = Mathf.FloorToInt(cy - r - w) - 2; y <= Mathf.CeilToInt(cy + r + w) + 2; y++)
            {
                for (int x = Mathf.FloorToInt(cx - r - w) - 2; x <= Mathf.CeilToInt(cx + r + w) + 2; x++)
                {
                    float dx = x + 0.5f - cx;
                    float dy = y + 0.5f - cy;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    Blend(x, y, w * 0.5f - Mathf.Abs(d - r) + 0.5f, c);
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

        public void Line(float x0, float y0, float x1, float y1, float w, Color c)
        {
            float hw = w * 0.5f;
            for (int y = Mathf.FloorToInt(Mathf.Min(y0, y1) - hw) - 2; y <= Mathf.CeilToInt(Mathf.Max(y0, y1) + hw) + 2; y++)
                for (int x = Mathf.FloorToInt(Mathf.Min(x0, x1) - hw) - 2; x <= Mathf.CeilToInt(Mathf.Max(x0, x1) + hw) + 2; x++)
                    Blend(x, y, hw - SegDist(x + 0.5f, y + 0.5f, x0, y0, x1, y1) + 0.5f, c);
        }

        // উত্তল বহুভুজ
        public void Poly(Color c, params Vector2[] pts)
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
                    Blend(x, y, m + 0.5f, c);
                }
            }
        }

        // পাঁচ কোণা তারা (rot = প্রথম কোণ কত ডিগ্রিতে)
        public void Star(float cx, float cy, float ro, Color c, float rot)
        {
            float ri = ro * 0.382f;
            for (int k = 0; k < 5; k++)
            {
                float a0 = (rot + k * 72f - 36f) * Mathf.Deg2Rad;
                float a1 = (rot + k * 72f) * Mathf.Deg2Rad;
                float a2 = (rot + k * 72f + 36f) * Mathf.Deg2Rad;
                Poly(c, new Vector2(cx, cy),
                     new Vector2(cx + ri * Mathf.Cos(a0), cy + ri * Mathf.Sin(a0)),
                     new Vector2(cx + ro * Mathf.Cos(a1), cy + ro * Mathf.Sin(a1)),
                     new Vector2(cx + ri * Mathf.Cos(a2), cy + ri * Mathf.Sin(a2)));
            }
            Disc(cx, cy, ri, c);
        }

        static float SD(float x, float y, float r)
        {
            float qx = Mathf.Abs(x - W * 0.5f) - (W * 0.5f - r);
            float qy = Mathf.Abs(y - H * 0.5f) - (H * 0.5f - r);
            float ox = Mathf.Max(qx, 0f);
            float oy = Mathf.Max(qy, 0f);
            return Mathf.Sqrt(ox * ox + oy * oy) + Mathf.Min(Mathf.Max(qx, qy), 0f) - r;
        }

        // ৩ডি রূপ: গোল কোণা, ঢেউয়ের আলো-ছায়া, ওপরে চকচক, হালকা ধার, নিচে ছায়া
        public Sprite Finish()
        {
            const int pad = 20;
            const float r = 16f;
            int FW = W + 2 * pad;
            int FH = H + 2 * pad;
            Color32[] px = new Color32[FW * FH];

            for (int y = 0; y < FH; y++)
            {
                float Y = y + 0.5f - pad;
                float t = Mathf.Clamp01(Y / H);
                for (int x = 0; x < FW; x++)
                {
                    float X = x + 0.5f - pad;
                    float s = SD(X, Y, r);
                    float fc = Mathf.Clamp01(0.5f - s);

                    int fx = Mathf.Clamp((int)X, 0, W - 1);
                    int fy = Mathf.Clamp((int)Y, 0, H - 1);
                    int fi = fy * W + fx;
                    float ba = A[fi];
                    float cr = R[fi] * ba + 0.85f * (1f - ba);
                    float cg = G[fi] * ba + 0.88f * (1f - ba);
                    float cb = B[fi] * ba + 0.95f * (1f - ba);

                    float wave = 1f + 0.09f * Mathf.Sin(X * 0.055f + Y * 0.012f + 0.6f);
                    cr *= wave;
                    cg *= wave;
                    cb *= wave;

                    float gl = 0.20f * Mathf.Pow(Mathf.Clamp01((t - 0.45f) / 0.55f), 1.5f);
                    cr = cr * (1f - gl) + gl;
                    cg = cg * (1f - gl) + gl;
                    cb = cb * (1f - gl) + gl;

                    float low = 0.10f * Mathf.Clamp01((0.35f - t) / 0.35f);
                    cr *= 1f - low;
                    cg *= 1f - low;
                    cb *= 1f - low;

                    float ring = Mathf.Clamp01(s + 3f + 0.5f);
                    float edge = 0.25f + 0.65f * Mathf.Clamp01(0.5f + 0.5f * ((0.5f - X / W) + (t - 0.5f)));
                    float k = ring * 0.55f;
                    cr = cr * (1f - k) + k * edge;
                    cg = cg * (1f - k) + k * edge;
                    cb = cb * (1f - k) + k * edge;

                    float ss = SD(X, Y + 10f, r);
                    float sh = Mathf.Clamp01(1f - (ss + 5f) / 20f);
                    sh = sh * sh * 0.5f * (1f - fc);

                    float a = fc + sh * (1f - fc);
                    int i = y * FW + x;
                    if (a < 0.00001f)
                    {
                        px[i] = new Color32(0, 0, 0, 0);
                        continue;
                    }
                    float inv = fc / a;
                    px[i] = new Color32((byte)(Mathf.Clamp01(cr * inv) * 255f), (byte)(Mathf.Clamp01(cg * inv) * 255f),
                                        (byte)(Mathf.Clamp01(cb * inv) * 255f), (byte)(Mathf.Clamp01(a) * 255f));
                }
            }

            Texture2D tex = new Texture2D(FW, FH, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            tex.SetPixels32(px);
            tex.Apply(false, true);
            return Sprite.Create(tex, new Rect(0, 0, FW, FH), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}

// ব্যবহারকারীর দেশ আর বয়স (ফোনে সেভ থাকে)
public static class ProfileInfo
{
    const string KAge = "pi_age";
    const string KCountry = "pi_country";

    // বয়স: ০ মানে এখনো দেওয়া হয়নি
    public static int Age
    {
        get { return PlayerPrefs.GetInt(KAge, 0); }
        set
        {
            PlayerPrefs.SetInt(KAge, Mathf.Clamp(value, 0, 120));
            PlayerPrefs.Save();
        }
    }

    // দেশ: -১ মানে এখনো বাছা হয়নি, নাহলে Flags এর নম্বর
    public static int CountryIndex
    {
        get { return PlayerPrefs.GetInt(KCountry, -1); }
        set
        {
            PlayerPrefs.SetInt(KCountry, Mathf.Clamp(value, -1, Flags.Count - 1));
            PlayerPrefs.Save();
        }
    }

    public static string CountryName
    {
        get { return CountryIndex < 0 ? "" : Flags.Name(CountryIndex); }
    }
}