using UnityEngine;

// ২০টা প্রাণীর ছবি, পুরোটা কোড দিয়ে আঁকা। কোনো ছবি ফাইল লাগে না।
// AvatarArt.Get(0) থেকে AvatarArt.Get(19) পর্যন্ত ছবি (Sprite) পাওয়া যায়।
public static class AvatarArt
{
    public const int Count = 30;

    static readonly Sprite[] cache = new Sprite[Count];

    public static Sprite Get(int index)
    {
        index = Mathf.Clamp(index, 0, Count - 1);
        if (cache[index] == null) cache[index] = Build(index);
        return cache[index];
    }

    static Sprite Build(int i)
    {
        Art p = new Art(192);
        switch (i)
        {
            case 0: Cat(p); break;
            case 1: Dog(p); break;
            case 2: Rabbit(p); break;
            case 3: Bear(p); break;
            case 4: Panda(p); break;
            case 5: Fox(p); break;
            case 6: Lion(p); break;
            case 7: Pig(p); break;
            case 8: Frog(p); break;
            case 9: Monkey(p); break;
            case 10: Owl(p); break;
            case 11: Penguin(p); break;
            case 12: Koala(p); break;
            case 13: Mouse(p); break;
            case 14: Chick(p); break;
            case 15: Tiger(p); break;
            case 16: Elephant(p); break;
            case 17: Unicorn(p); break;
            case 18: Hamster(p); break;
            case 19: Ladybug(p); break;
            case 20: Cow(p); break;
            case 21: Sheep(p); break;
            case 22: Wolf(p); break;
            case 23: Raccoon(p); break;
            case 24: Duck(p); break;
            case 25: Zebra(p); break;
            case 26: Dragon(p); break;
            case 27: Whale(p); break;
            case 28: Robot(p); break;
            default: Alien(p); break;
        }
        return p.ToSprite();
    }

    // ---------- রং ----------
    static Color Col(int r, int g, int b, float a = 1f)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a);
    }

    static readonly Color Dark = new Color(60 / 255f, 40 / 255f, 40 / 255f, 1f);
    static readonly Color Black = new Color(30 / 255f, 28 / 255f, 34 / 255f, 1f);
    static readonly Color White = new Color(1f, 1f, 1f, 1f);
    static readonly Color Pink = new Color(1f, 150 / 255f, 170 / 255f, 1f);
    static readonly Color Blush = new Color(1f, 140 / 255f, 150 / 255f, 0.55f);

    // ---------- ছোট সাহায্যকারী ----------
    static void Bg(Art p, Color c)
    {
        p.Disc(96, 96, 94, Col(255, 255, 255, 0.9f), 0f);
        p.Disc(96, 96, 90, c, 0.35f);
        p.Oval(96, 26, 50, 9, Col(0, 0, 0, 0.20f), 0f);
    }

    static void Eyes(Art p, float cx, float cy, float dx, float r)
    {
        for (int k = 0; k < 2; k++)
        {
            float ex = cx + (k == 0 ? -dx : dx);
            p.Disc(ex, cy, r, Black);
            p.Disc(ex + r * 0.3f, cy + r * 0.35f, r * 0.38f, White);
        }
    }

    static void CatMouth(Art p, float x, float y, Color c)
    {
        p.Arc(x - 7, y, 7, 3, 230, 350, c);
        p.Arc(x + 7, y, 7, 3, 190, 310, c);
    }

    // ---------- প্রাণী ----------
    static void Cat(Art p)
    {
        Bg(p, Col(255, 226, 170));
        Color o = Col(255, 160, 60);
        Color stripe = Col(220, 120, 30);
        p.Tri(44, 124, 54, 172, 92, 146, o);
        p.Tri(148, 124, 138, 172, 100, 146, o);
        p.Tri(56, 134, 62, 160, 80, 146, Pink);
        p.Tri(136, 134, 130, 160, 112, 146, Pink);
        p.Disc(96, 92, 58, o);
        p.Line(96, 150, 96, 136, 5, stripe);
        p.Line(82, 146, 85, 136, 5, stripe);
        p.Line(110, 146, 107, 136, 5, stripe);
        p.Oval(96, 74, 26, 19, Col(255, 236, 208));
        Eyes(p, 96, 100, 25, 7);
        p.Tri(90, 84, 102, 84, 96, 76, Col(255, 110, 140));
        p.Line(96, 76, 96, 70, 3, Dark);
        CatMouth(p, 96, 70, Dark);
        p.Line(62, 80, 32, 84, 2.5f, Dark);
        p.Line(130, 80, 160, 84, 2.5f, Dark);
        p.Line(62, 71, 32, 66, 2.5f, Dark);
        p.Line(130, 71, 160, 66, 2.5f, Dark);
        p.Disc(62, 78, 9, Blush);
        p.Disc(130, 78, 9, Blush);
    }

    static void Dog(Art p)
    {
        Bg(p, Col(200, 230, 255));
        p.OvalR(42, 100, 19, 42, 14, Col(125, 78, 40));
        p.OvalR(150, 100, 19, 42, -14, Col(125, 78, 40));
        p.Disc(96, 92, 56, Col(200, 145, 85));
        p.Disc(126, 104, 18, Col(125, 78, 40));
        p.Oval(96, 70, 31, 25, Col(250, 232, 205));
        Eyes(p, 96, 104, 24, 7);
        p.Oval(96, 80, 13, 9, Black);
        p.Oval(96, 54, 9, 11, Col(255, 120, 135));
        p.Line(96, 72, 96, 63, 3, Dark);
        p.Arc(88, 63, 8, 3, 225, 335, Dark);
        p.Arc(104, 63, 8, 3, 205, 315, Dark);
    }

    static void Rabbit(Art p)
    {
        Bg(p, Col(190, 225, 255));
        p.OvalR(68, 148, 15, 40, 8, Col(250, 250, 252));
        p.OvalR(124, 148, 15, 40, -8, Col(250, 250, 252));
        p.OvalR(68, 148, 8, 29, 8, Col(255, 180, 200));
        p.OvalR(124, 148, 8, 29, -8, Col(255, 180, 200));
        p.Disc(96, 88, 57, Col(250, 250, 252));
        Eyes(p, 96, 96, 25, 7);
        p.Tri(90, 80, 102, 80, 96, 72, Col(255, 130, 155));
        p.Line(96, 72, 96, 66, 3, Dark);
        CatMouth(p, 96, 66, Dark);
        p.Disc(60, 76, 10, Col(255, 150, 175, 0.6f));
        p.Disc(132, 76, 10, Col(255, 150, 175, 0.6f));
    }

    static void Bear(Art p)
    {
        Bg(p, Col(255, 235, 190));
        Color b = Col(160, 105, 60);
        Color nose = Col(50, 30, 25);
        p.Disc(46, 142, 21, b);
        p.Disc(146, 142, 21, b);
        p.Disc(46, 142, 11, Col(215, 165, 125));
        p.Disc(146, 142, 11, Col(215, 165, 125));
        p.Disc(96, 90, 58, b);
        p.Oval(96, 72, 28, 22, Col(238, 205, 165));
        Eyes(p, 96, 100, 26, 7);
        p.Oval(96, 80, 11, 8, nose);
        p.Line(96, 74, 96, 66, 3, nose);
        p.Arc(89, 66, 7, 3, 225, 335, nose);
        p.Arc(103, 66, 7, 3, 205, 315, nose);
        p.Disc(62, 76, 9, Blush);
        p.Disc(130, 76, 9, Blush);
    }

    static void Panda(Art p)
    {
        Bg(p, Col(200, 240, 210));
        Color k = Col(40, 40, 46);
        p.Disc(46, 144, 21, k);
        p.Disc(146, 144, 21, k);
        p.Disc(96, 90, 58, Col(252, 252, 252));
        p.OvalR(66, 96, 15, 20, 20, k);
        p.OvalR(126, 96, 15, 20, -20, k);
        p.Disc(68, 98, 7, White);
        p.Disc(68, 98, 3.5f, Col(20, 20, 25));
        p.Disc(124, 98, 7, White);
        p.Disc(124, 98, 3.5f, Col(20, 20, 25));
        p.Oval(96, 76, 11, 8, k);
        p.Line(96, 70, 96, 62, 3, k);
        p.Arc(89, 62, 7, 3, 225, 335, k);
        p.Arc(103, 62, 7, 3, 205, 315, k);
        p.Disc(58, 72, 9, Blush);
        p.Disc(134, 72, 9, Blush);
    }

    static void Fox(Art p)
    {
        Bg(p, Col(255, 235, 200));
        Color o = Col(240, 125, 40);
        Color inner = Col(90, 40, 30);
        p.Tri(40, 118, 50, 176, 92, 146, o);
        p.Tri(152, 118, 142, 176, 100, 146, o);
        p.Tri(52, 130, 56, 160, 78, 146, inner);
        p.Tri(140, 130, 136, 160, 114, 146, inner);
        p.Disc(96, 92, 57, o);
        p.OvalR(66, 76, 32, 19, -25, White);
        p.OvalR(126, 76, 32, 19, 25, White);
        p.Oval(96, 64, 24, 16, White);
        Eyes(p, 96, 104, 26, 6);
        p.Oval(96, 62, 10, 7, Black);
        p.Line(96, 58, 96, 52, 3, Dark);
        p.Arc(89, 52, 7, 3, 225, 335, Dark);
        p.Arc(103, 52, 7, 3, 205, 315, Dark);
    }

    static void Lion(Art p)
    {
        Bg(p, Col(255, 236, 180));
        Color m = Col(196, 118, 38);
        for (int k = 0; k < 12; k++)
        {
            float a = k * 30f * Mathf.Deg2Rad;
            p.Disc(96 + 66 * Mathf.Cos(a), 92 + 66 * Mathf.Sin(a), 22, m);
        }
        p.Disc(96, 92, 70, m);
        p.Disc(52, 146, 16, Col(235, 175, 70));
        p.Disc(140, 146, 16, Col(235, 175, 70));
        p.Disc(96, 90, 50, Col(246, 192, 82));
        p.Oval(96, 72, 24, 19, Col(255, 236, 205));
        Eyes(p, 96, 98, 22, 6);
        p.Tri(88, 80, 104, 80, 96, 70, Col(110, 60, 50));
        p.Line(96, 70, 96, 64, 3, Dark);
        CatMouth(p, 96, 64, Dark);
        p.Disc(66, 74, 8, Blush);
        p.Disc(126, 74, 8, Blush);
    }

    static void Pig(Art p)
    {
        Bg(p, Col(255, 240, 190));
        p.Tri(46, 122, 58, 168, 92, 140, Col(240, 135, 160));
        p.Tri(146, 122, 134, 168, 100, 140, Col(240, 135, 160));
        p.Disc(96, 90, 58, Col(255, 170, 185));
        p.Oval(96, 76, 25, 19, Col(255, 140, 162));
        p.Oval(87, 76, 4, 7, Col(190, 80, 105));
        p.Oval(105, 76, 4, 7, Col(190, 80, 105));
        Eyes(p, 96, 106, 26, 6);
        p.Arc(96, 62, 12, 3, 215, 325, Col(200, 90, 110));
        p.Disc(58, 84, 9, Col(255, 120, 140, 0.6f));
        p.Disc(134, 84, 9, Col(255, 120, 140, 0.6f));
    }

    static void Frog(Art p)
    {
        Bg(p, Col(255, 245, 200));
        Color g = Col(110, 200, 90);
        p.Oval(96, 84, 62, 50, g);
        p.Disc(62, 134, 24, g);
        p.Disc(62, 136, 15, White);
        p.Disc(64, 136, 8, Black);
        p.Disc(68, 140, 3, White);
        p.Disc(130, 134, 24, g);
        p.Disc(130, 136, 15, White);
        p.Disc(128, 136, 8, Black);
        p.Disc(130, 140, 3, White);
        p.Arc(96, 92, 42, 4, 215, 325, Col(40, 100, 40));
        p.Disc(88, 100, 3, Col(50, 120, 50));
        p.Disc(104, 100, 3, Col(50, 120, 50));
        p.Disc(54, 76, 10, Col(255, 150, 150, 0.55f));
        p.Disc(138, 76, 10, Col(255, 150, 150, 0.55f));
    }

    static void Monkey(Art p)
    {
        Bg(p, Col(200, 235, 255));
        Color b = Col(140, 90, 48);
        Color t = Col(245, 210, 170);
        Color f = Col(120, 70, 40);
        p.Disc(36, 92, 21, b);
        p.Disc(156, 92, 21, b);
        p.Disc(36, 92, 12, t);
        p.Disc(156, 92, 12, t);
        p.Disc(96, 94, 56, b);
        p.Disc(78, 102, 23, t);
        p.Disc(114, 102, 23, t);
        p.Oval(96, 80, 38, 32, t);
        Eyes(p, 96, 102, 20, 6);
        p.Disc(90, 82, 3, f);
        p.Disc(102, 82, 3, f);
        p.Arc(96, 80, 22, 3, 215, 325, f);
    }

    static void Owl(Art p)
    {
        Bg(p, Col(210, 230, 255));
        p.Tri(44, 126, 48, 172, 86, 146, Col(120, 78, 40));
        p.Tri(148, 126, 144, 172, 106, 146, Col(120, 78, 40));
        p.Disc(96, 92, 60, Col(150, 100, 55));
        p.Oval(96, 96, 48, 42, Col(235, 205, 160));
        p.Disc(70, 100, 27, Col(120, 80, 40));
        p.Disc(70, 100, 24, White);
        p.Disc(72, 100, 11, Black);
        p.Disc(75, 104, 3.5f, White);
        p.Disc(122, 100, 27, Col(120, 80, 40));
        p.Disc(122, 100, 24, White);
        p.Disc(120, 100, 11, Black);
        p.Disc(123, 104, 3.5f, White);
        p.Tri(88, 86, 104, 86, 96, 64, Col(255, 170, 40));
    }

    static void Penguin(Art p)
    {
        Bg(p, Col(215, 240, 255));
        p.Disc(96, 92, 60, Col(35, 40, 60));
        p.Oval(96, 86, 42, 36, White);
        p.Disc(76, 100, 20, White);
        p.Disc(116, 100, 20, White);
        Eyes(p, 96, 100, 24, 6);
        p.Tri(86, 84, 106, 84, 96, 66, Col(255, 160, 30));
        p.Disc(66, 80, 9, Col(255, 150, 160, 0.6f));
        p.Disc(126, 80, 9, Col(255, 150, 160, 0.6f));
    }

    static void Koala(Art p)
    {
        Bg(p, Col(215, 240, 225));
        Color g = Col(175, 180, 190);
        p.Disc(38, 124, 29, g);
        p.Disc(154, 124, 29, g);
        p.Disc(38, 124, 17, Col(238, 238, 242));
        p.Disc(154, 124, 17, Col(238, 238, 242));
        p.Disc(96, 90, 57, g);
        Eyes(p, 96, 100, 25, 6);
        p.Oval(96, 78, 15, 19, Col(60, 60, 70));
        p.Disc(90, 84, 3, Col(140, 140, 150));
        p.Arc(96, 58, 10, 3, 215, 325, Dark);
        p.Disc(58, 78, 9, Blush);
        p.Disc(134, 78, 9, Blush);
    }

    static void Mouse(Art p)
    {
        Bg(p, Col(255, 235, 215));
        Color g = Col(200, 200, 212);
        p.Disc(46, 128, 29, g);
        p.Disc(146, 128, 29, g);
        p.Disc(46, 128, 18, Col(255, 190, 205));
        p.Disc(146, 128, 18, Col(255, 190, 205));
        p.Disc(96, 88, 53, g);
        p.Oval(96, 74, 27, 20, Col(244, 244, 248));
        Eyes(p, 96, 98, 24, 6);
        p.Disc(96, 80, 7, Col(255, 130, 150));
        p.Line(96, 74, 96, 66, 3, Dark);
        CatMouth(p, 96, 66, Dark);
        p.Line(70, 74, 40, 78, 2.5f, Dark);
        p.Line(70, 66, 42, 62, 2.5f, Dark);
        p.Line(122, 74, 152, 78, 2.5f, Dark);
        p.Line(122, 66, 150, 62, 2.5f, Dark);
    }

    static void Chick(Art p)
    {
        Bg(p, Col(200, 235, 255));
        Color y = Col(255, 225, 80);
        p.Oval(96, 152, 6, 15, y);
        p.OvalR(84, 150, 6, 13, 22, y);
        p.OvalR(108, 150, 6, 13, -22, y);
        p.Disc(96, 88, 60, y);
        p.OvalR(38, 84, 14, 24, 20, Col(250, 205, 60));
        p.OvalR(154, 84, 14, 24, -20, Col(250, 205, 60));
        Eyes(p, 96, 98, 26, 7);
        p.Tri(82, 82, 110, 82, 96, 64, Col(255, 150, 30));
        p.Disc(60, 76, 10, Col(255, 150, 130, 0.65f));
        p.Disc(132, 76, 10, Col(255, 150, 130, 0.65f));
    }

    static void Tiger(Art p)
    {
        Bg(p, Col(255, 240, 200));
        Color o = Col(250, 150, 40);
        Color st = Col(60, 40, 30);
        p.Disc(48, 140, 21, o);
        p.Disc(144, 140, 21, o);
        p.Disc(48, 140, 11, Col(250, 235, 210));
        p.Disc(144, 140, 11, Col(250, 235, 210));
        p.Disc(96, 90, 58, o);
        p.Line(96, 148, 96, 128, 5, st);
        p.Line(80, 144, 84, 128, 5, st);
        p.Line(112, 144, 108, 128, 5, st);
        p.Line(40, 98, 58, 92, 5, st);
        p.Line(40, 80, 58, 82, 5, st);
        p.Line(152, 98, 134, 92, 5, st);
        p.Line(152, 80, 134, 82, 5, st);
        p.Oval(96, 70, 27, 21, Col(255, 240, 215));
        Eyes(p, 96, 100, 24, 6);
        p.Tri(89, 80, 103, 80, 96, 72, Col(220, 100, 110));
        p.Line(96, 72, 96, 66, 3, Dark);
        CatMouth(p, 96, 66, Dark);
    }

    static void Elephant(Art p)
    {
        Bg(p, Col(255, 240, 215));
        Color g = Col(170, 190, 215);
        Color ear = Col(150, 170, 198);
        Color line = Col(135, 155, 182);
        p.Disc(44, 94, 31, ear);
        p.Disc(148, 94, 31, ear);
        p.Disc(44, 94, 20, Col(238, 178, 192));
        p.Disc(148, 94, 20, Col(238, 178, 192));
        p.Disc(96, 98, 54, g);
        p.Oval(96, 50, 17, 38, g);
        p.Oval(96, 22, 15, 10, ear);
        p.Line(88, 66, 88, 40, 2.5f, line);
        p.Line(104, 66, 104, 40, 2.5f, line);
        Eyes(p, 96, 108, 24, 6);
        p.Disc(58, 88, 9, Blush);
        p.Disc(134, 88, 9, Blush);
    }

    static void Unicorn(Art p)
    {
        Bg(p, Col(235, 225, 255));
        for (int k = 0; k < 2; k++)
        {
            float sgn = k == 0 ? -1f : 1f;
            float cx = 96 + sgn * 56;
            p.Disc(cx - sgn * 4, 112, 27, Col(255, 130, 190));
            p.Disc(cx - sgn * 8, 84, 22, Col(130, 200, 255));
            p.Disc(cx - sgn * 4, 58, 18, Col(190, 150, 255));
        }
        p.Tri(64, 134, 70, 160, 86, 142, Col(250, 250, 255));
        p.Tri(128, 134, 122, 160, 106, 142, Col(250, 250, 255));
        p.Tri(86, 136, 106, 136, 96, 184, Col(255, 215, 80));
        p.Line(89, 150, 103, 154, 3, Col(255, 160, 40));
        p.Line(91, 162, 101, 165, 3, Col(255, 160, 40));
        p.Disc(96, 88, 54, Col(250, 248, 255));
        p.Arc(72, 94, 9, 4, 30, 150, Black);
        p.Arc(120, 94, 9, 4, 30, 150, Black);
        p.Disc(60, 78, 10, Col(255, 150, 190, 0.6f));
        p.Disc(132, 78, 10, Col(255, 150, 190, 0.6f));
        p.Oval(96, 66, 22, 15, Col(255, 232, 244));
        p.Disc(89, 68, 2.5f, Col(220, 140, 170));
        p.Disc(103, 68, 2.5f, Col(220, 140, 170));
        p.Arc(96, 62, 8, 3, 215, 325, Dark);
    }

    static void Hamster(Art p)
    {
        Bg(p, Col(215, 240, 230));
        p.Disc(54, 138, 17, Col(230, 175, 115));
        p.Disc(138, 138, 17, Col(230, 175, 115));
        p.Disc(54, 138, 9, Pink);
        p.Disc(138, 138, 9, Pink);
        p.Oval(96, 88, 63, 54, Col(245, 200, 140));
        p.Oval(96, 70, 45, 30, Col(255, 246, 232));
        Eyes(p, 96, 100, 25, 7);
        p.Disc(96, 86, 5, Pink);
        p.Line(96, 82, 96, 74, 3, Dark);
        CatMouth(p, 96, 74, Dark);
        p.Disc(50, 76, 12, Col(255, 150, 160, 0.6f));
        p.Disc(142, 76, 12, Col(255, 150, 160, 0.6f));
    }

    static void Ladybug(Art p)
    {
        Bg(p, Col(215, 240, 215));
        p.Line(80, 148, 66, 178, 3.5f, Black);
        p.Line(112, 148, 126, 178, 3.5f, Black);
        p.Disc(66, 178, 6, Black);
        p.Disc(126, 178, 6, Black);
        p.Oval(96, 150, 32, 22, Col(40, 40, 46));
        p.Disc(86, 154, 7, White);
        p.Disc(106, 154, 7, White);
        p.Disc(87, 154, 3, Black);
        p.Disc(105, 154, 3, Black);
        p.Disc(96, 86, 64, Col(228, 50, 50));
        p.Line(96, 148, 96, 24, 3.5f, Black);
        p.Disc(66, 100, 11, Black);
        p.Disc(126, 100, 11, Black);
        p.Disc(60, 62, 10, Black);
        p.Disc(132, 62, 10, Black);
        p.Disc(80, 128, 6, Col(255, 255, 255, 0.35f));
    }

    // ---------- নতুন ১০টা ----------
    static void Cow(Art p)
    {
        Bg(p, Col(210, 235, 255));
        Color k = Col(50, 50, 56);
        p.OvalR(52, 140, 8, 16, 25, Col(245, 225, 170));
        p.OvalR(140, 140, 8, 16, -25, Col(245, 225, 170));
        p.OvalR(34, 112, 24, 13, -20, Col(250, 250, 250));
        p.OvalR(158, 112, 24, 13, 20, Col(250, 250, 250));
        p.OvalR(34, 112, 14, 7, -20, Pink);
        p.OvalR(158, 112, 14, 7, 20, Pink);
        p.Disc(96, 92, 58, Col(252, 252, 252));
        p.OvalR(128, 112, 22, 26, -20, k);
        p.OvalR(66, 124, 16, 12, 10, k);
        p.Oval(96, 64, 36, 26, Col(255, 170, 190));
        p.Oval(84, 64, 5, 8, Col(200, 100, 125));
        p.Oval(108, 64, 5, 8, Col(200, 100, 125));
        Eyes(p, 96, 100, 26, 7);
        p.Arc(96, 50, 14, 3, 215, 325, Col(200, 90, 110));
    }

    static void Sheep(Art p)
    {
        Bg(p, Col(215, 240, 225));
        Color wool = Col(250, 245, 235);
        for (int k = 0; k < 10; k++)
        {
            float a = k * 36f * Mathf.Deg2Rad;
            p.Disc(96 + 52 * Mathf.Cos(a), 98 + 52 * Mathf.Sin(a), 24, wool);
        }
        p.Disc(96, 98, 56, wool);
        Color face = Col(74, 64, 66);
        p.OvalR(38, 88, 18, 10, -15, face);
        p.OvalR(154, 88, 18, 10, 15, face);
        p.Oval(96, 80, 38, 44, face);
        p.Disc(96, 138, 26, wool);
        p.Disc(76, 90, 10, White);
        p.Disc(77, 90, 5, Black);
        p.Disc(116, 90, 10, White);
        p.Disc(117, 90, 5, Black);
        p.Oval(96, 68, 11, 8, Col(255, 170, 185));
        p.Arc(96, 58, 10, 3, 215, 325, Col(255, 170, 185));
    }

    static void Wolf(Art p)
    {
        Bg(p, Col(205, 220, 245));
        Color g = Col(130, 138, 160);
        Color lt = Col(240, 243, 250);
        p.Tri(40, 118, 48, 176, 90, 146, g);
        p.Tri(152, 118, 144, 176, 102, 146, g);
        p.Tri(52, 130, 56, 160, 78, 146, Col(60, 60, 80));
        p.Tri(140, 130, 136, 160, 114, 146, Col(60, 60, 80));
        p.Disc(96, 92, 57, g);
        p.OvalR(60, 76, 28, 16, -30, lt);
        p.OvalR(132, 76, 28, 16, 30, lt);
        p.Oval(96, 68, 30, 24, lt);
        Eyes(p, 96, 104, 26, 6);
        p.Oval(96, 76, 11, 8, Black);
        p.Line(96, 70, 96, 62, 3, Dark);
        p.Arc(89, 62, 7, 3, 225, 335, Dark);
        p.Arc(103, 62, 7, 3, 205, 315, Dark);
    }

    static void Raccoon(Art p)
    {
        Bg(p, Col(255, 240, 200));
        Color g = Col(150, 155, 170);
        Color k = Col(50, 50, 58);
        p.Disc(48, 142, 20, g);
        p.Disc(144, 142, 20, g);
        p.Disc(48, 142, 11, k);
        p.Disc(144, 142, 11, k);
        p.Disc(96, 90, 58, g);
        p.Oval(96, 70, 34, 28, Col(245, 245, 248));
        p.OvalR(64, 102, 26, 17, 20, k);
        p.OvalR(128, 102, 26, 17, -20, k);
        p.Disc(64, 102, 10, White);
        p.Disc(65, 102, 5.5f, Black);
        p.Disc(128, 102, 10, White);
        p.Disc(129, 102, 5.5f, Black);
        p.Oval(96, 78, 11, 8, Black);
        p.Line(96, 72, 96, 64, 3, Dark);
        p.Arc(89, 64, 7, 3, 225, 335, Dark);
        p.Arc(103, 64, 7, 3, 205, 315, Dark);
    }

    static void Duck(Art p)
    {
        Bg(p, Col(200, 235, 255));
        Color y = Col(255, 215, 70);
        p.Oval(96, 150, 7, 15, y);
        p.OvalR(86, 148, 6, 12, 20, y);
        p.OvalR(106, 148, 6, 12, -20, y);
        p.Disc(96, 90, 60, y);
        p.OvalR(36, 84, 14, 24, 20, Col(245, 195, 50));
        p.OvalR(156, 84, 14, 24, -20, Col(245, 195, 50));
        Eyes(p, 96, 104, 28, 7);
        p.Oval(96, 62, 30, 13, Col(240, 120, 20));
        p.Oval(96, 72, 38, 22, Col(255, 150, 40));
        p.Disc(86, 78, 3, Col(200, 90, 10));
        p.Disc(106, 78, 3, Col(200, 90, 10));
        p.Disc(60, 80, 10, Col(255, 150, 130, 0.6f));
        p.Disc(132, 80, 10, Col(255, 150, 130, 0.6f));
    }

    static void Zebra(Art p)
    {
        Bg(p, Col(210, 240, 215));
        Color k = Col(40, 40, 48);
        p.OvalR(58, 142, 12, 24, 15, Col(250, 250, 250));
        p.OvalR(134, 142, 12, 24, -15, Col(250, 250, 250));
        p.OvalR(58, 142, 6, 16, 15, k);
        p.OvalR(134, 142, 6, 16, -15, k);
        p.Oval(96, 152, 12, 22, k);
        p.Disc(96, 90, 56, Col(250, 250, 250));
        p.Line(40, 104, 62, 98, 7, k);
        p.Line(152, 104, 130, 98, 7, k);
        p.Line(40, 88, 62, 86, 7, k);
        p.Line(152, 88, 130, 86, 7, k);
        p.Line(40, 72, 62, 74, 7, k);
        p.Line(152, 72, 130, 74, 7, k);
        p.Line(96, 146, 96, 126, 7, k);
        p.Line(80, 142, 84, 126, 6, k);
        p.Line(112, 142, 108, 126, 6, k);
        p.Oval(96, 64, 32, 24, Col(72, 70, 82));
        p.Oval(86, 66, 4, 6, Col(30, 30, 38));
        p.Oval(106, 66, 4, 6, Col(30, 30, 38));
        Eyes(p, 96, 100, 24, 6);
    }

    static void Dragon(Art p)
    {
        Bg(p, Col(255, 235, 215));
        Color g = Col(90, 205, 110);
        Color sp = Col(255, 190, 60);
        p.Tri(70, 142, 78, 176, 90, 144, sp);
        p.Tri(90, 144, 96, 182, 102, 144, sp);
        p.Tri(102, 144, 114, 176, 122, 142, sp);
        Color h = Col(245, 225, 170);
        p.Tri(52, 126, 38, 168, 78, 140, h);
        p.Tri(140, 126, 154, 168, 114, 140, h);
        p.Disc(96, 90, 58, g);
        p.Oval(96, 68, 32, 24, Col(170, 235, 150));
        p.Disc(86, 72, 4, Col(40, 110, 50));
        p.Disc(106, 72, 4, Col(40, 110, 50));
        p.Disc(68, 104, 13, Col(255, 245, 160));
        p.Oval(68, 104, 4.5f, 10, Black);
        p.Disc(71, 108, 2.5f, White);
        p.Disc(124, 104, 13, Col(255, 245, 160));
        p.Oval(124, 104, 4.5f, 10, Black);
        p.Disc(127, 108, 2.5f, White);
        p.Arc(96, 56, 12, 3, 215, 325, Col(40, 110, 50));
        p.Disc(54, 78, 9, Col(255, 150, 130, 0.5f));
        p.Disc(138, 78, 9, Col(255, 150, 130, 0.5f));
    }

    static void Whale(Art p)
    {
        Bg(p, Col(255, 245, 210));
        Color b = Col(80, 150, 240);
        Color sp = Col(170, 220, 255);
        p.Oval(96, 160, 6, 16, sp);
        p.OvalR(84, 152, 5, 12, 25, sp);
        p.OvalR(108, 152, 5, 12, -25, sp);
        p.OvalR(36, 72, 16, 26, 25, Col(60, 125, 215));
        p.OvalR(156, 72, 16, 26, -25, Col(60, 125, 215));
        p.Disc(96, 86, 60, b);
        p.Oval(96, 60, 44, 34, Col(220, 238, 255));
        Eyes(p, 96, 100, 26, 7);
        p.Arc(96, 76, 26, 4, 215, 325, Col(40, 90, 170));
        p.Disc(56, 82, 10, Col(255, 150, 170, 0.5f));
        p.Disc(136, 82, 10, Col(255, 150, 170, 0.5f));
    }

    static void Robot(Art p)
    {
        Bg(p, Col(215, 235, 245));
        Color m = Col(150, 170, 200);
        Color bolt = Col(110, 125, 155);
        p.Line(96, 138, 96, 164, 5, Col(120, 130, 150));
        p.Disc(96, 168, 10, Col(255, 80, 80));
        p.RRect(22, 76, 46, 114, 8, bolt);
        p.RRect(146, 76, 170, 114, 8, bolt);
        p.RRect(38, 34, 154, 148, 30, m);
        p.RRect(52, 48, 140, 134, 22, Col(30, 40, 72), 0.35f);
        p.Disc(74, 100, 17, Col(30, 200, 235));
        p.Disc(74, 100, 9, Col(210, 255, 255));
        p.Disc(118, 100, 17, Col(30, 200, 235));
        p.Disc(118, 100, 9, Col(210, 255, 255));
        p.RRect(76, 62, 116, 78, 6, Col(70, 80, 110));
        p.Line(86, 62, 86, 78, 2, m);
        p.Line(96, 62, 96, 78, 2, m);
        p.Line(106, 62, 106, 78, 2, m);
        p.Disc(50, 48, 4, bolt);
        p.Disc(142, 48, 4, bolt);
        p.Disc(50, 134, 4, bolt);
        p.Disc(142, 134, 4, bolt);
    }

    static void Alien(Art p)
    {
        Bg(p, Col(225, 215, 255));
        Color g = Col(140, 230, 120);
        Color dk = Col(80, 160, 70);
        p.Line(70, 150, 56, 176, 4, Col(110, 200, 100));
        p.Line(122, 150, 136, 176, 4, Col(110, 200, 100));
        p.Disc(56, 178, 9, Col(255, 120, 200));
        p.Disc(136, 178, 9, Col(255, 120, 200));
        p.Oval(96, 90, 56, 64, g);
        p.OvalR(66, 96, 20, 30, 25, Black);
        p.OvalR(126, 96, 20, 30, -25, Black);
        p.Disc(60, 108, 6, White);
        p.Disc(120, 108, 6, White);
        p.Disc(92, 66, 2.5f, dk);
        p.Disc(100, 66, 2.5f, dk);
        p.Arc(96, 58, 10, 3, 215, 325, dk);
    }

    // ---------- আঁকার খাতা (রঙিন, ৩ডি আলো-ছায়া, y ওপরের দিকে বাড়ে) ----------
    class Art
    {
        // আলো আসে ওপর-বাম থেকে
        const float Lx = -0.4511f, Ly = 0.5514f, Lz = 0.7018f;
        const float Hx = -0.2445f, Hy = 0.2989f, Hz = 0.9224f;

        readonly int S;
        readonly float[] R;
        readonly float[] G;
        readonly float[] B;
        readonly float[] A;

        public Art(int size)
        {
            S = size;
            R = new float[size * size];
            G = new float[size * size];
            B = new float[size * size];
            A = new float[size * size];
        }

        void Paint(int x, int y, float cov, float cr, float cg, float cb, float ca)
        {
            if (x < 0 || y < 0 || x >= S || y >= S) return;
            float sa = Mathf.Clamp01(cov) * ca;
            if (sa <= 0f) return;
            int i = y * S + x;
            float da = A[i];
            float oa = sa + da * (1f - sa);
            R[i] = (cr * sa + R[i] * da * (1f - sa)) / oa;
            G[i] = (cg * sa + G[i] * da * (1f - sa)) / oa;
            B[i] = (cb * sa + B[i] * da * (1f - sa)) / oa;
            A[i] = oa;
        }

        void Blend(int x, int y, float cov, Color c)
        {
            Paint(x, y, cov, c.r, c.g, c.b, c.a);
        }

        static void Shade(float nx, float ny, float nz, float shade, out float k, out float spec)
        {
            float diff = Mathf.Clamp01(nx * Lx + ny * Ly + nz * Lz);
            k = 1f + shade * ((0.58f + 0.55f * diff) - 1f);
            float h = Mathf.Clamp01(nx * Hx + ny * Hy + nz * Hz);
            spec = shade * 0.5f * Mathf.Pow(h, 28f);
        }

        void PaintLit(int x, int y, float cov, Color c, float k, float spec)
        {
            Paint(x, y, cov, Mathf.Clamp01(c.r * k + spec), Mathf.Clamp01(c.g * k + spec),
                  Mathf.Clamp01(c.b * k + spec), c.a);
        }

        public void Disc(float cx, float cy, float r, Color c, float shade = 1f)
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
                    float nx = dx / r;
                    float ny = dy / r;
                    float nz = Mathf.Sqrt(Mathf.Max(0f, 1f - (nx * nx + ny * ny)));
                    float k, spec;
                    Shade(nx, ny, nz, shade, out k, out spec);
                    PaintLit(x, y, r - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f, c, k, spec);
                }
            }
        }

        public void Oval(float cx, float cy, float rx, float ry, Color c, float shade = 1f)
        {
            OvalR(cx, cy, rx, ry, 0f, c, shade);
        }

        // ang = কত ডিগ্রি ঘোরানো
        public void OvalR(float cx, float cy, float rx, float ry, float ang, Color c, float shade = 1f)
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
                    float nu = u / rx;
                    float nv = v / ry;
                    float d = Mathf.Sqrt(nu * nu + nv * nv);
                    float nz = Mathf.Sqrt(Mathf.Max(0f, 1f - d * d));
                    float wx = nu * ca - nv * sa;
                    float wy = nu * sa + nv * ca;
                    float k, spec;
                    Shade(wx, wy, nz, shade, out k, out spec);
                    PaintLit(x, y, (1f - d) * small + 0.5f, c, k, spec);
                }
            }
        }

        public void Tri(float ax, float ay, float bx, float by, float cx, float cy, Color c)
        {
            PolyS(c, 1f, new Vector2(ax, ay), new Vector2(bx, by), new Vector2(cx, cy));
        }

        // উত্তল বহুভুজ (ধারে একটু গাঢ়, ওপর-বামে একটু উজ্জ্বল)
        public void PolyS(Color c, float shade, params Vector2[] pts)
        {
            int n = pts.Length;
            float area = 0f;
            float minXf = float.MaxValue;
            float maxXf = float.MinValue;
            float minYf = float.MaxValue;
            float maxYf = float.MinValue;
            float sumX = 0f;
            float sumY = 0f;
            for (int i = 0; i < n; i++)
            {
                Vector2 p = pts[i];
                Vector2 q = pts[(i + 1) % n];
                area += p.x * q.y - q.x * p.y;
                minXf = Mathf.Min(minXf, p.x);
                maxXf = Mathf.Max(maxXf, p.x);
                minYf = Mathf.Min(minYf, p.y);
                maxYf = Mathf.Max(maxYf, p.y);
                sumX += p.x;
                sumY += p.y;
            }
            float sign = area >= 0f ? 1f : -1f;
            float cxp = sumX / n;
            float cyp = sumY / n;
            float size = Mathf.Max(maxXf - minXf, maxYf - minYf) * 0.5f + 0.000001f;
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
                    float kl = 1f + shade * 0.25f * ((-(px - cxp) * 0.45f + (py - cyp) * 0.55f) / size);
                    float ke = 1f - shade * 0.22f * (1f - Mathf.Clamp01(m / 7f));
                    PaintLit(x, y, m + 0.5f, c, kl * ke, 0f);
                }
            }
        }

        static float RRSd(float px, float py, float mx, float my, float hx, float hy, float r)
        {
            float qx = Mathf.Abs(px - mx) - hx;
            float qy = Mathf.Abs(py - my) - hy;
            float ox = Mathf.Max(qx, 0f);
            float oy = Mathf.Max(qy, 0f);
            return Mathf.Sqrt(ox * ox + oy * oy) + Mathf.Min(Mathf.Max(qx, qy), 0f) - r;
        }

        // গোল-কোণা চতুর্ভুজ, ফোলা বালিশের মতো আলো-ছায়া
        public void RRect(float x0, float y0, float x1, float y1, float r, Color c, float shade = 1f)
        {
            float mx = (x0 + x1) * 0.5f;
            float my = (y0 + y1) * 0.5f;
            float hx = (x1 - x0) * 0.5f - r;
            float hy = (y1 - y0) * 0.5f - r;
            for (int y = Mathf.FloorToInt(y0) - 2; y <= Mathf.CeilToInt(y1) + 2; y++)
            {
                for (int x = Mathf.FloorToInt(x0) - 2; x <= Mathf.CeilToInt(x1) + 2; x++)
                {
                    float px = x + 0.5f;
                    float py = y + 0.5f;
                    float sd = RRSd(px, py, mx, my, hx, hy, r);
                    float t = Mathf.Clamp01(-sd / 16f);
                    float gx = (RRSd(px + 1f, py, mx, my, hx, hy, r) - RRSd(px - 1f, py, mx, my, hx, hy, r)) * 0.5f;
                    float gy = (RRSd(px, py + 1f, mx, my, hx, hy, r) - RRSd(px, py - 1f, mx, my, hx, hy, r)) * 0.5f;
                    float ln = Mathf.Sqrt(gx * gx + gy * gy) + 0.000001f;
                    float nx = gx / ln * (1f - t);
                    float ny = gy / ln * (1f - t);
                    float nz = t;
                    float nl = Mathf.Sqrt(nx * nx + ny * ny + nz * nz) + 0.000001f;
                    float k, spec;
                    Shade(nx / nl, ny / nl, nz / nl, shade, out k, out spec);
                    PaintLit(x, y, 0.5f - sd, c, k, spec);
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
            float r0 = a0 * Mathf.Deg2Rad;
            float r1 = a1 * Mathf.Deg2Rad;
            Disc(cx + r * Mathf.Cos(r0), cy + r * Mathf.Sin(r0), hw, c);
            Disc(cx + r * Mathf.Cos(r1), cy + r * Mathf.Sin(r1), hw, c);
        }

        public Sprite ToSprite()
        {
            Texture2D tex = new Texture2D(S, S, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            Color32[] px = new Color32[S * S];
            for (int i = 0; i < px.Length; i++)
            {
                px[i] = new Color32(
                    (byte)(Mathf.Clamp01(R[i]) * 255f),
                    (byte)(Mathf.Clamp01(G[i]) * 255f),
                    (byte)(Mathf.Clamp01(B[i]) * 255f),
                    (byte)(Mathf.Clamp01(A[i]) * 255f));
            }
            tex.SetPixels32(px);
            tex.Apply(false, true);
            return Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}