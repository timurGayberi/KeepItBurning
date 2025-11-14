using TMPro;
using UnityEngine;
using Managers.GamePlayManagers;

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
            // When the panel is shown, display the current score
            if (PlayGameManager.Instance != null && finalScoreText != null)
            {
                int finalScore = PlayGameManager.Instance.GetCurrentScore();
                finalScoreText.text = $"{finalScore:N0}";
            }
        }
    }
}
