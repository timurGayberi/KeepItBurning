using TMPro;
using UnityEngine;
using Player;
using General; 
using System;

namespace Managers.GamePlayManagers
{
    public class InGameHudDisplayComponent : MonoBehaviour
    {
        [Header("UI References")]
        
        [Tooltip("The TextMeshPro element displaying the current player score.")]
        public TextMeshProUGUI scoreText;
        
        public TextMeshProUGUI timeText;
        
        public TextMeshProUGUI carriedLogsNumber;
        
        // Player inventory reference
        private PlayerInventory playerInventory;

        void OnEnable()
        {
            if (PlayGameManager.Instance != null)
            {
                PlayGameManager.Instance.OnTimeUpdated += UpdateTimeDisplay;
                
                PlayGameManager.Instance.OnScoreUpdated += UpdateScoreDisplay;
                
                if (timeText != null)
                {
                    UpdateTimeDisplay(PlayGameManager.Instance.GetCurrentFormattedTime());
                }
                
                if (scoreText != null)
                {
                    UpdateScoreDisplay(PlayGameManager.Instance.GetCurrentScore());
                }

                try
                {
                    playerInventory = ServiceLocator.GetService<PlayerInventory>();
                    
                    playerInventory.OnWoodCountChanged += UpdateLogDisplay;
                    
                    if (carriedLogsNumber != null)
                    {
                        UpdateLogDisplay(playerInventory.WoodCount); 
                    }
                }
                catch (InvalidOperationException e)
                {
                    Debug.LogWarning($"InGameHudDisplayComponent: PlayerInventory not found. Log UI will not update. Error: {e.Message}");
                }
            }
        }

        void OnDisable()
        {
            if (PlayGameManager.Instance != null)
            {
                PlayGameManager.Instance.OnTimeUpdated -= UpdateTimeDisplay;
                PlayGameManager.Instance.OnScoreUpdated -= UpdateScoreDisplay;
            }
            if (playerInventory != null)
            {
                playerInventory.OnWoodCountChanged -= UpdateLogDisplay;
            }
        }
        
        private void UpdateTimeDisplay(string formattedTime)
        {
            if (timeText != null)
            {
                timeText.text = formattedTime;
            }
        }
        
        private void UpdateScoreDisplay(int currentScore)
        {
            if (scoreText != null)
            {
                scoreText.text = currentScore.ToString("N0"); 
            }
        }

        private void UpdateLogDisplay(int currentCount)
        {
            if (carriedLogsNumber != null && playerInventory != null)
            {
                carriedLogsNumber.text = $"{currentCount}/{playerInventory.MaxWoodCount}";
            }
        }
    }
}