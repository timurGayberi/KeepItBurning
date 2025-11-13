using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public struct SaveData
{
    public List<int> Scores;
}

public class SaveManager : MonoBehaviour
{
    [SerializeField] private SaveData data;
    [SerializeField] string fileName = "save";

    public List<int> GetScores => data.Scores;

    private string GetPath()
    {
        return Application.persistentDataPath + "/" + fileName + ".json";
    }

    void Awake()
    {
        LoadData();
    }

    public void LoadData()
    {
        if (!File.Exists(GetPath()))
        {
            data.Scores = new List<int>();
            SaveGameFile();
            return;
        }

        string jsonfile = File.ReadAllText(GetPath());
        data = JsonUtility.FromJson<SaveData>(jsonfile);
        if (data.Scores == null)
            data.Scores = new List<int>();
    }

    public void SaveGameFile()
    {
        string jsonfile = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(), jsonfile);
    }

    public void AddScoreToLb(int score)
    {
        if (data.Scores == null)
            data.Scores = new List<int>();

        data.Scores.Add(score);
        data.Scores.Sort((a, b) => b.CompareTo(a));
        SaveGameFile();
    }

    public int GetHighscore()
    {
        if (data.Scores == null || data.Scores.Count == 0)
            return 0;

        return data.Scores[0];
    }
}
