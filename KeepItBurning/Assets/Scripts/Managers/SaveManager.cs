using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Managers
{
    [Serializable]
    public struct SaveData
    {
        public List<float> Scores;
    }

    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private SaveData data;
        [SerializeField] string fileName = "save";

        public List<float> GetScores => data.Scores;

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
                data.Scores = new List<float>();
                SaveGameFile();
                return;
            }

            string jsonfile = File.ReadAllText(GetPath());
            data = JsonUtility.FromJson<SaveData>(jsonfile);
            if (data.Scores == null)
                data.Scores = new List<float>();
        }

        public void SaveGameFile()
        {
            string jsonfile = JsonUtility.ToJson(data, true);
            File.WriteAllText(GetPath(), jsonfile);
        }

        public void AddScoreToLb(float Score)
        {
            if (data.Scores == null)
                data.Scores = new List<float>();

            data.Scores.Add(Score);
            data.Scores.Sort((a, b) => b.CompareTo(a));
            SaveGameFile();
        }

        public float GetHighscore()
        {
            if (data.Scores == null || data.Scores.Count == 0)
                return 0;

            return data.Scores[0];
        }
    }
}