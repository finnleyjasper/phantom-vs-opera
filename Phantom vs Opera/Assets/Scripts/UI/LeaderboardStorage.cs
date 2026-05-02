using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LeaderboardEntry
{
    public string playerName;
    public float score;
}

[Serializable]
class LeaderboardData
{
    public LeaderboardEntry[] entries = Array.Empty<LeaderboardEntry>();
}

/// <summary>Local top scores (PlayerPrefs). Call <see cref="RecordRun"/> from gameplay when a run ends.</summary>
public static class LeaderboardStorage
{
    const string PrefsKey = "PvO_Leaderboard_v1";
    public const int MaxEntries = 10;

    public static IReadOnlyList<LeaderboardEntry> GetTopEntries()
    {
        LeaderboardData data = Load();
        return data.entries ?? Array.Empty<LeaderboardEntry>();
    }

    public static void RecordRun(float audienceSupport, string playerName = "Player")
    {
        if (audienceSupport < 0f) return;

        var list = new List<LeaderboardEntry>(Load().entries ?? Array.Empty<LeaderboardEntry>());
        list.Add(new LeaderboardEntry { playerName = playerName, score = audienceSupport });
        list.Sort((a, b) => b.score.CompareTo(a.score));
        if (list.Count > MaxEntries)
            list.RemoveRange(MaxEntries, list.Count - MaxEntries);

        Save(new LeaderboardData { entries = list.ToArray() });
    }

    static LeaderboardData Load()
    {
        string json = PlayerPrefs.GetString(PrefsKey, "");
        if (string.IsNullOrEmpty(json))
            return new LeaderboardData();
        try
        {
            return JsonUtility.FromJson<LeaderboardData>(json);
        }
        catch
        {
            return new LeaderboardData();
        }
    }

    static void Save(LeaderboardData data)
    {
        PlayerPrefs.SetString(PrefsKey, JsonUtility.ToJson(data));
        PlayerPrefs.Save();
    }
}
