using System;
using System.IO;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int score;
    public int matchedPairs;
    public int countGuesses;
    public int totalCards;
    public int[] spriteIndices;    // which sprite each card has
    public bool[] matchedCards;    // which cards are already matched
}

public static class SaveSystem
{
    private static string SavePath => Application.persistentDataPath + "/savegame.json";

    public static void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("Game saved to: " + SavePath);
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found");
            return null;
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
    }
}