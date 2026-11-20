using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// CHALLENGE পাতা: তিনটা মোডের কার্ড (Daily, Time Attack, Survival), সেরা স্কোর সহ।
// এখন শুধু ডিজাইন ও স্কোর দেখানো কাজ করে। PLAY চাপলে "Coming soon" আসে,
// মোডের আসল খেলা GameController এ যোগ করার সময় Play() এর ভেতরে বসবে।
// MainMenu তে নিজে নিজেই তৈরি হয়, Challenge বাটন নিজেই খুঁজে নেয়।
public class ChallengePanel : GlassPage
{
    static readonly string[] Titles = { "Daily Challenge", "Time Attack", "Survival" };
    static readonly string[] Descs =
    {
        "5 new words every day. Keep your streak alive.",
        "90 seconds. Solve as many words as you can.",
        "3 lives. Words get harder every round."
    };
    static readonly string[] Icons = { "star", "bolt", "flame" };
    static readonly Color[] Accents =
    {
        new Color32(70, 170, 255, 255),
        new Color32(255, 205, 80, 255),
        new Color32(255, 100, 90, 255)
    };

    protected override string[] ButtonTokens { get { return new[] { "challenge" }; } }
    protected override string PageTitle { get { return "CHALLENGE"; } }
    protected override string EmblemIcon { get { return "flame"; } }
    protected override Color Accent { get { return Neo.C(255, 100, 90); } }

    // ---------- সেভ করা তথ্য (PlayerPrefs) ----------
    static string Today { get { return DateTime.Now.ToString("yyyyMMdd"); } }
    static bool DailyDone { get { return PlayerPrefs.GetString("ch_daily_date", "") == Today; } }
    static int Best(int mode) { return PlayerPrefs.GetInt("ch_best_" + mode, 0); }
    static int DailyStreak { get { return PlayerPrefs.GetInt("ch_daily_streak", 0); } }

    protected override void BuildContent()
    {
        UIKit.Label(window, "Pick a mode and beat your best score", 60, 205, 780, 50, 32,
                    Neo.C(255, 255, 255, 0.85f), TextAlignmentOptions.Center);

        for (int i = 0; i < 3; i++)
        {
            BuildCard(i, 290f + i * 350f);
        }
    }

    void BuildCard(int i, float y)
    {
        const float x = 45f;
        const float w = 810f;
        const float h = 320f;
        Color ac = Accents[i];
        bool doneToday = i == 0 && DailyDone;

        GlassKit.Card(window, x, y, w, h, ac, true, pulses);

        // গোল ব্যাজ আর আইকন
        GlassKit.Badge(window, x + 30, y + 30, 110, ac, true);
        Neo.Icon(window, x + 47, y + 45, 76, Icons[i], Neo.Mix(ac, 1f, 110f), Neo.Mix(ac, 0.7f, 0f));

        // লেখা
        UIKit.Label(window, Titles[i], x + 170, y + 28, 480, 56, 42, Color.white, TextAlignmentOptions.Left);
        UIKit.Label(window, Descs[i], x + 170, y + 92, 600, 90, 26, Neo.C(195, 205, 240), TextAlignmentOptions.Left);

        // "SOON" ফিতা (মোডের খেলা বসলে এটা সরিয়ে দিন)
        GlassKit.Chip(window, "SOON", x + w - 170, y + 30, 140, 50,
                      Neo.C(170, 110, 255), Neo.C(110, 60, 220), Color.white, 24);

        // সেরা স্কোর আর মোডের তথ্য
        GlassKit.Chip(window, "BEST  " + Best(i), x + 30, y + 215, 230, 60,
                      Neo.C(255, 226, 110), Neo.C(240, 140, 20), Neo.C(40, 28, 8), 28);

        string info = i == 0 ? "STREAK  " + DailyStreak : (i == 1 ? "90 SEC" : "3 LIVES");
        GlassKit.Chip(window, info, x + 280, y + 215, 210, 60,
                      Neo.Mix(ac, 0.55f, 90f), Neo.Mix(ac, 0.55f, 0f), Color.white, 28);

        // PLAY বোতাম
        float bx = x + w - 290f;
        float by = y + h - 120f;
        if (doneToday)
        {
            Neo.Surface(window, "Play", bx, by, 250, 84,
                GlassKit.St(42, Neo.C(90, 98, 140), Neo.C(50, 56, 96), Neo.C(255, 255, 255, 0.3f),
                            Neo.C(255, 255, 255, 0.08f), 2.5f, 0.2f, 0.45f, 14f, 6f, 1f));
            UIKit.Label(window, "DONE TODAY", bx, by, 250, 84, 30, Neo.C(200, 208, 240), TextAlignmentOptions.Center);
        }
        else
        {
            Neo.Surface(window, "Play", bx, by, 250, 84,
                GlassKit.St(42, Neo.C(110, 240, 140), Neo.C(30, 175, 90), Neo.C(255, 255, 255, 0.7f),
                            Neo.C(80, 220, 120, 0.5f), 2.5f, 0.5f, 0.45f, 14f, 6f, 1f));
            UIKit.Label(window, "PLAY", bx, by, 250, 84, 38, Color.white, TextAlignmentOptions.Center);
        }
        UIKit.Hit(window, bx, by, 250, 84, () => Play(i));
    }

    void Play(int mode)
    {
        if (mode == 0 && DailyDone)
        {
            Toast("Come back tomorrow");
            return;
        }
        AudioManager.PlayClick();
        // TODO: এখানে GameSession এ মোড রেখে GameScene লোড হবে (Step 5)
        Toast(Titles[mode] + " - coming soon");
    }
}