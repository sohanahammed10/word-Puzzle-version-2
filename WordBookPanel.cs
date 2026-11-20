using TMPro;
using UnityEngine;
using UnityEngine.UI;

// WORD BOOK পাতা: ওপরে ১০টা টিয়ারের ট্যাব, নিচে ১০টা শব্দের কার্ড।
// শেখা শব্দ রঙিন (চেক চিহ্ন সহ), না শেখা শব্দ "? ? ?" আর ধূসর।
// MainMenu তে নিজে নিজেই তৈরি হয়, BookButton নিজেই খুঁজে নেয়।
public class WordBookPanel : GlassPage
{
    int tier;
    RectTransform dyn;

    protected override string[] ButtonTokens { get { return new[] { "book" }; } }
    protected override string PageTitle { get { return "WORD BOOK"; } }
    protected override string EmblemIcon { get { return "medal"; } }
    protected override Color Accent { get { return Neo.C(170, 110, 255); } }

    protected override void BuildContent()
    {
        int reached = GlassKit.Reached;
        Summary("Words Learned", reached + " / 100", reached / 100f);
        tier = Mathf.Min(reached / 10, 9);
        Rebuild();
    }

    // ট্যাব বদলালে ট্যাব আর কার্ড নতুন করে আঁকা হয়
    void Rebuild()
    {
        if (dyn != null) Destroy(dyn.gameObject);
        dyn = UIKit.Box(window, "Dynamic", 0, 0, 900, 1580);

        int reached = GlassKit.Reached;

        // ট্যাব: দুই সারি, প্রতি সারিতে ৫টা
        for (int t = 0; t < 10; t++)
        {
            int ti = t;
            float x = 60f + (t % 5) * 158f;
            float y = 320f + (t / 5) * 76f;
            bool sel = t == tier;
            Color ac = GlassKit.TierColors[t];

            Neo.Style st;
            if (sel)
            {
                st = GlassKit.St(30, Neo.Mix(ac, 0.55f, 90f), Neo.Mix(ac, 0.55f, 0f), Neo.C(255, 255, 255, 0.7f), ac,
                                 3f, 0.5f, 0.45f, 14f, 6f, 1f);
            }
            else
            {
                st = GlassKit.St(30, Neo.C(46, 52, 110, 0.72f), Neo.C(28, 30, 76, 0.72f), Neo.C(255, 255, 255, 0.22f),
                                 Neo.C(255, 255, 255, 0.06f), 2.5f, 0.35f, 0.4f, 12f, 5f, 1f);
            }
            Neo.Surface(dyn, "Tab", x, y, 148, 60, st);
            UIKit.Label(dyn, GlassKit.TierNames[t], x, y, 148, 60, 21,
                        sel ? Color.white : Neo.C(195, 205, 240), TextAlignmentOptions.Center);
            UIKit.Hit(dyn, x, y, 148, 60, () =>
            {
                AudioManager.PlayClick();
                tier = ti;
                Rebuild();
            });
        }

        // শব্দের কার্ড: দুই কলাম, পাঁচ সারি
        Color tc = GlassKit.TierColors[tier];
        for (int i = 0; i < 10; i++)
        {
            int g = tier * 10 + i;
            string word = GlassKit.Words[tier][i];
            bool learned = g < reached;
            float x = 60f + (i % 2) * 405f;
            float y = 490f + (i / 2) * 190f;

            GlassKit.Card(dyn, x, y, 375, 168, tc, learned, learned ? pulses : null);

            if (learned)
            {
                float fs = word.Length > 8 ? 36f : 46f;
                UIKit.Label(dyn, word, x + 26, y + 22, 280, 70, fs, Color.white, TextAlignmentOptions.Left);
                UIKit.Label(dyn, word.Length + " letters  |  Level " + (g + 1), x + 26, y + 100, 300, 40, 24,
                            Neo.C(195, 205, 240), TextAlignmentOptions.Left);

                Neo.Surface(dyn, "Done", x + 375 - 84, y + 22, 60, 60,
                    GlassKit.St(30, Neo.C(110, 240, 140), Neo.C(30, 175, 90), Neo.C(255, 255, 255, 0.7f),
                                Neo.C(80, 220, 120, 0.5f), 2.5f, 0.5f, 0.45f, 12f, 5f, 1f));
                Neo.Icon(dyn, x + 375 - 84 + 12, y + 34, 36, "check", Color.white, Neo.C(205, 255, 220));
            }
            else
            {
                string q = "";
                for (int k = 0; k < word.Length; k++) q += (k == 0 ? "?" : " ?");
                UIKit.Label(dyn, q, x + 26, y + 26, 300, 60, 36, Neo.C(255, 255, 255, 0.55f), TextAlignmentOptions.Left);
                UIKit.Label(dyn, "Level " + (g + 1) + "  |  locked", x + 26, y + 100, 300, 40, 24,
                            Neo.C(195, 205, 240, 0.6f), TextAlignmentOptions.Left);
                Neo.Icon(dyn, x + 375 - 80, y + 26, 44, "lock", Neo.C(225, 230, 255), Neo.C(150, 158, 205));
            }

            UIKit.Hit(dyn, x, y, 375, 168, () => OnWord(g, word, learned));
        }
    }

    void OnWord(int g, string word, bool learned)
    {
        if (learned)
        {
            AudioManager.PlayClick();
            Toast(word + "  |  Level " + (g + 1));
        }
        else
        {
            Toast("Reach level " + (g + 1) + " to learn this word");
        }
    }
}