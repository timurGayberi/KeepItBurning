using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TMP_Text entryPrefab;
    [SerializeField] private int maxEntriesToShow = 5;

    void Start()
    {
        if (saveManager == null)
        {
            return;
        }

        saveManager.LoadData();
        DisplayScores();
    }

    void DisplayScores()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        var scores = saveManager.GetScores;
        if (scores == null || scores.Count == 0)
        {
            TMP_Text noScores = Instantiate(entryPrefab, contentParent);
            noScores.text = "No scores yet!";
            return;
        }

        int count = Mathf.Min(scores.Count, maxEntriesToShow);
        for (int i = 0; i < count; i++)
        {
            TMP_Text entry = Instantiate(entryPrefab, contentParent);
            entry.text = $"{i + 1}. {scores[i].ToString("F0")} Points";
        }
    }
}
