using TMPro;
using UnityEngine;
using Managers.GamePlayManagers;
using Score;

namespace UI
{
    /// <summary>
    /// Displays the final score on the Game Over panel.
    /// Attach this to your Game Over panel GameObject.
    /// </summary>
    public class GameOverPanel : MonoBehaviour
    {
        [Header("UI References")]
        [Tooltip("The TextMeshPro element displaying the final score.")]
        public TextMeshProUGUI finalScoreText;

        private void OnEnable()
        {
            UpdateScore();
        }

        private void Update()
        {
            UpdateScore();
        }

        private void UpdateScore()
        {
            // Update the score text - use ScoreManager if available, otherwise PlayGameManager
            if (finalScoreText != null)
            {
                float finalScore = 0f;

                if (ScoreManager.Instance != null)
                {
                    finalScore = ScoreManager.Instance.Score;
                }
                else if (PlayGameManager.Instance != null)
                {
                    finalScore = PlayGameManager.Instance.GetCurrentScore();
                }

                finalScoreText.text = $"{finalScore:N0}";
            }
        }
    }
}
