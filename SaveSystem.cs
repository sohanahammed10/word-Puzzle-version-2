using UnityEngine;

public static class SaveSystem
{
    const string LevelKey = "LastLevel";

    public static void SaveLevel(int level)
    {
        PlayerPrefs.SetInt(LevelKey, level);
        PlayerPrefs.Save();
    }

    public static int LoadLevel()
    {
        return PlayerPrefs.GetInt(LevelKey, 0);
    }

    public static bool HasSave()
    {
        return PlayerPrefs.HasKey(LevelKey);
    }
}