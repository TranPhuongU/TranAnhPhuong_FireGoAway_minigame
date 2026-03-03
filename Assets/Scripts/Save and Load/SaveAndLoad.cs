using System.IO;
using UnityEngine;

[System.Serializable]
public class LevelProgressData
{
    public int unlockedLevel = 0;
}

public static class SaveAndLoad
{
    static LevelProgressData data;
    static string SavePath =>
        Path.Combine(Application.persistentDataPath, "level_progress.json");

    // ⭐ đảm bảo luôn có data
    static void EnsureLoaded()
    {
        if (data != null) return;

        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            data = JsonUtility.FromJson<LevelProgressData>(json);
        }
        else
        {
            data = new LevelProgressData();
            Save();
        }
    }

    // ===== SAVE =====
    public static void Save()
    {
        EnsureLoaded();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    // ===== QUERY =====
    public static bool IsLevelUnlocked(int levelIndex)
    {
        EnsureLoaded();
        return levelIndex <= data.unlockedLevel;
    }

    public static void UnlockLevel(int levelIndex)
    {
        EnsureLoaded();

        if (levelIndex > data.unlockedLevel)
        {
            data.unlockedLevel = levelIndex;
            Save();
        }
    }

    public static int GetUnlockedLevel()
    {
        EnsureLoaded();
        return data.unlockedLevel;
    }
}