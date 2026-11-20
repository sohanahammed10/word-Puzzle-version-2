using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// WORD MAP পাতা: ১০টা দ্বীপ (প্রতিটা একটা টিয়ার), প্রতি দ্বীপে ১০টা লেভেল-বিন্দু।
// সম্পন্ন বা বর্তমান বিন্দুতে চাপলে ওই লেভেল থেকে খেলা শুরু হয়। লক বিন্দুতে চাপলে বার্তা আসে।
// MainMenu তে নিজে নিজেই তৈরি হয়, MapButton নিজেই খুঁজে নেয়।
public class WordMapPanel : GlassPage
{
    public string gameSceneName = "GameScene";

    const float ViewH = 1180f;
    const float CardH = 350f;
    const float Step = 374f;

    ScrollRect scroll;
    RectTransform content;

    protected override string[] ButtonTokens { get { return new[] { "map" }; } }
    protected override string PageTitle { get { return "WORD MAP"; } }
    protected override string EmblemIcon { get { return "diamond"; } }
    protected override Color Accent { get { return Neo.C(60, 220, 200); } }

    protected override void BuildContent()
    {
        int reached = GlassKit.Reached;
        Summary("Cleared " + reached + " / 100", reached + "%", reached / 100f);

        float contentH = 10 * Step + 20f;
        scroll = GlassKit.Scroll(window, 45, 320, 810, ViewH, contentH, out content);

        for (int i = 0; i < 10; i++)
        {
            BuildIsland(i, 10f + i * Step, reached);
        }
    }

    void BuildIsland(int i, float y, int reached)
    {
        Color ac = GlassKit.TierColors[i];
        int done = Mathf.Clamp(reached - i * 10, 0, 10);
        bool unlocked = reached >= i * 10;
        bool current = unlocked && reached < (i + 1) * 10;

        GlassKit.Card(content, 15, y, 780, CardH, ac, unlocked, current ? pulses : null);

        Color nameColor = unlocked ? Color.white : Neo.C(255, 255, 255, 0.6f);
        UIKit.Label(content, "ISLAND " + (i + 1) + "  " + GlassKit.TierNames[i], 45, y + 18, 520, 56, 38,
                    nameColor, TextAlignmentOptions.Left);

        if (unlocked)
            GlassKit.Chip(content, done + "/10", 615, y + 20, 150, 52, Neo.Mix(ac, 0.55f, 90f), Neo.Mix(ac, 0.55f, 0f), Color.white, 28);
        else
            Neo.Icon(content, 700, y + 18, 56, "lock", Neo.C(225, 230, 255), Neo.C(150, 158, 205));

        // প্রগ্রেস বার
        Neo.Bar(content, "Track", 45, y + 88, 720, 16, Neo.C(8, 12, 40, 0.9f), Neo.C(30, 36, 86, 0.9f), 0f, true);
        Image fill = Neo.Bar(content, "Fill", 45, y + 88, 720, 16, Neo.Mix(ac, 0.4f, 150f), ac, 0.6f, false);
        AddBar(fill, 720f, 16f, done / 10f);

        // লেভেল-বিন্দু: দুই সারি, প্রতি সারিতে ৫টা
        for (int j = 0; j < 10; j++)
        {
            float dx = 45f + (j % 5) * 156f;
            float dy = y + 122f + (j / 5) * 110f;
            Dot(dx, dy, i * 10 + j, reached, ac);
        }
    }

    void Dot(float x, float y, int g, int reached, Color ac)
    {
        const float d = 96f;
        bool done = g < reached;
        bool cur = g == reached;

        if (done)
        {
            GlassKit.Badge(content, x, y, d, ac, true);
            UIKit.Label(content, (g + 1).ToString(), x, y, d, d, 38, Color.white, TextAlignmentOptions.Center);
        }
        else if (cur)
        {
            Color gold = Neo.C(255, 214, 90);
            Image glow = Neo.Glow(content, "DotGlow", x - 52, y - 52, 200, gold, 0.6f);
            pulses.Add(glow);
            Neo.Surface(content, "Dot", x, y, d, d,
                GlassKit.St(48, Neo.C(255, 236, 140), Neo.C(240, 150, 20), Neo.C(255, 255, 255, 0.85f),
                            Neo.C(255, 200, 80, 0.7f), 4f, 0.6f, 0.5f, 18f, 8f, 1f));
            UIKit.Label(content, (g + 1).ToString(), x, y, d, d, 40, Neo.C(60, 36, 6), TextAlignmentOptions.Center);
        }
        else
        {
            GlassKit.Badge(content, x, y, d, ac, false);
            Neo.Icon(content, x + 28, y + 26, 40, "lock", Neo.C(225, 230, 255), Neo.C(150, 158, 205));
        }

        UIKit.Hit(content, x, y, d, d, () => OnDot(g));
    }

    void OnDot(int g)
    {
        int reached = GlassKit.Reached;
        if (g > reached || g >= 100)
        {
            Toast("Finish level " + Mathf.Min(reached + 1, 100) + " first");
            return;
        }
        AudioManager.PlayClick();
        PlayerData.SavedLevel = g;
        SceneManager.LoadScene(gameSceneName);
    }

    protected override void OnOpened()
    {
        if (scroll == null) return;
        int cur = Mathf.Min(GlassKit.Reached / 10, 9);
        float max = Mathf.Max(0f, content.sizeDelta.y - ViewH);
        content.anchoredPosition = new Vector2(0f, Mathf.Clamp(cur * Step - 20f, 0f, max));
        scroll.velocity = Vector2.zero;
    }
}