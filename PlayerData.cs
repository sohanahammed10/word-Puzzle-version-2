using UnityEngine;

public static class PlayerData
{
    private static int Get(string k) { return PlayerPrefs.GetInt(k, 0); }
    private static void Put(string k, int v) { PlayerPrefs.SetInt(k, v); PlayerPrefs.Save(); }

    public static string Name
    {
        get { return PlayerPrefs.GetString("pd_name", "Player"); }
        set { PlayerPrefs.SetString("pd_name", value); PlayerPrefs.Save(); }
    }

    public static int SavedLevel { get { return Get("pd_level"); } set { Put("pd_level", value); } }
    public static int Stars { get { return Get("pd_stars"); } set { Put("pd_stars", value); } }
    public static int Coins { get { return Get("pd_coins"); } set { Put("pd_coins", value); } }
    public static int CompletedLevels { get { return Get("pd_done"); } set { Put("pd_done", value); } }
    public static int BestStreak { get { return Get("pd_best"); } set { Put("pd_best", value); } }
    public static int CurrentStreak { get { return Get("pd_streak"); } set { Put("pd_streak", value); } }
    public static int Xp { get { return Get("pd_xp"); } set { Put("pd_xp", value); } }

    public static int AvatarIndex
    {
        get { return PlayerPrefs.GetInt("pd_avatar", -1); }
        set { PlayerPrefs.SetInt("pd_avatar", value); PlayerPrefs.Save(); }
    }

    public static int PlayerLevel { get { return 1 + Xp / 100; } }
    public static int XpInLevel { get { return Xp % 100; } }

    public static int Achievements
    {
        get
        {
            int n = 0;
            int[] levels = { 10, 25, 50, 100 };
            int[] streaks = { 5, 10, 20 };
            foreach (int x in levels) if (CompletedLevels >= x) n++;
            foreach (int x in streaks) if (BestStreak >= x) n++;
            return n;
        }
    }

    public static void LevelCompleted(int mistakes)
    {
        int stars = mistakes == 0 ? 3 : (mistakes <= 2 ? 2 : 1);
        Stars += stars;
        Coins += stars * 10;
        Xp += 20;
        CompletedLevels += 1;

        if (mistakes == 0)
        {
            CurrentStreak += 1;
            if (CurrentStreak > BestStreak) BestStreak = CurrentStreak;
        }
        else
        {
            CurrentStreak = 0;
        }
    }
}