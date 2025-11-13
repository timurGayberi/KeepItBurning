using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TMP_Text entryPrefab;

    void Start()
    {
        saveManager.LoadData();
        DisplayScores();
    }

    void DisplayScores()
    {
        var scores = saveManager.GetScores;
        if (scores == null || scores.Count == 0)
        {
            TMP_Text noScores = Instantiate(entryPrefab, contentParent);
            noScores.text = "No scores yet!";
            return;
        }

        int maxDisplay = Mathf.Min(6, scores.Count);
        for (int i = 0; i < maxDisplay; i++)
        {
            TMP_Text entry = Instantiate(entryPrefab, contentParent);
            entry.text = $"{i + 1}. {scores[i]} kills";
        }
    }
} 
    
