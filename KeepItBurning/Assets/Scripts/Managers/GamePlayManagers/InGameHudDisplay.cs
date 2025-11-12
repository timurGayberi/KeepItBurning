using TMPro;
using UnityEngine;

namespace Managers.GamePlayManagers
{
    public class InGameHudDisplayComponent : MonoBehaviour
    {
        [Header("UI References")]
        
        [Tooltip("The TextMeshPro element displaying the current player score.")]
        public TextMeshProUGUI scoreText;
        
        public TextMeshProUGUI timeText;
        
        public TextMeshProUGUI carriedLogsNumber;

        void OnEnable()
        {
            if (PlayGameManager.Instance != null)
            {
                PlayGameManager.Instance.OnTimeUpdated += UpdateTimeDisplay;
                
                PlayGameManager.Instance.OnScoreUpdated += UpdateScoreDisplay;

                // 2. Initial synchronization check
                if (timeText != null)
                {
                    UpdateTimeDisplay(PlayGameManager.Instance.GetCurrentFormattedTime());
                }
                
                if (scoreText != null)
                {
                    UpdateScoreDisplay(PlayGameManager.Instance.GetCurrentScore());
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
    }
}