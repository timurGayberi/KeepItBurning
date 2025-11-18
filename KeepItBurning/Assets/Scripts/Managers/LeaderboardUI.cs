using System.Collections.Generic;
using Managers;
using TMPro;
using UnityEngine;
using Managers.GeneralManagers;
using Interfaces;
using PlayFab.ClientModels;


public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TMP_Text entryPrefab;
    [SerializeField] private int maxEntriesToShow = 5;
    
    
    private IPlayFabService _playFabService;
    
    private List<PlayerLeaderboardEntry> _playFabScores;
    

    void Start()
    {
        try
        {
            _playFabService = General.ServiceLocator.GetService<IPlayFabService>();
        }
        catch (System.InvalidOperationException)
        {
            Debug.LogWarning("[LeaderboardUI] PlayFab Service not registered. Falling back to local SaveManager.");
            _playFabService = null; // Ensure it's null if registration failed
        }
        
        // 2. Determine which source to use and initiate the load/display
        if (_playFabService != null)
        {
            // If service exists, retrieve data. The display happens in the callback.
            _playFabService.RetrieveLeaderboard();
            // DisplayScores will be called once PlayFab returns the result.
        }
        else if (saveManager != null)
        {
            // If PlayFab fails/missing, use local data immediately.
            saveManager.LoadData();
            DisplayScoresLocal();
        }
    }
    
    public void ReceivePlayFabScores(List<PlayerLeaderboardEntry> scores)
    {
        _playFabScores = scores;
        DisplayScoresPlayFab();
    }

    void DisplayScoresLocal()
    {
        ClearContent();

        var scores = saveManager.GetScores;
        if (scores == null || scores.Count == 0)
        {
            Instantiate(entryPrefab, contentParent).text = "No local scores yet!";
            return;
        }

        int count = Mathf.Min(scores.Count, maxEntriesToShow);
        for (int i = 0; i < count; i++)
        {
            TMP_Text entry = Instantiate(entryPrefab, contentParent);
            entry.text = $"{i + 1}. {scores[i]:F0} Points (Local)";
        }
    }
    
    void DisplayScoresPlayFab()
    {
        ClearContent();

        if (_playFabScores == null || _playFabScores.Count == 0)
        {
            Instantiate(entryPrefab, contentParent).text = "No online scores found!";
            return;
        }

        int count = Mathf.Min(_playFabScores.Count, maxEntriesToShow);
        for (int i = 0; i < count; i++)
        {
            var item = _playFabScores[i];
            
            // Get nickname (Display Name)
            string nickname = item.Profile?.DisplayName ?? item.PlayFabId;
            
            TMP_Text entry = Instantiate(entryPrefab, contentParent);
            
            // Note: PlayFab Position is 0-indexed, so we add 1 for display
            entry.text = $"{item.Position + 1}. {nickname}: {item.StatValue} Pts";
        }
    }
    
    void ClearContent()
    {
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);
    }
}
