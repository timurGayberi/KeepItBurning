using System;
using UnityEngine;
using Managers.GeneralManagers;
using Managers;
using Score;

namespace Managers.GamePlayManagers
{
    public class PlayGameManager : MonoBehaviour
    {
        public static PlayGameManager Instance { get; private set; }

        // --- Stats ---
        public int score = 0;
        public float timer = 0f;

        // --- Events for UI ---
        public event Action<string> OnTimeUpdated;
        public event Action<int> OnScoreUpdated;

        // OPTIMIZATION: Throttle time updates
        private int _lastSecond = -1;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // CRITICAL FIX: Make this manager persistent across scene loads
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // If another instance exists (from a previous scene load), destroy this one.
                Destroy(gameObject);
                return;
            }
        }


        private void Update()
        {
            if (Time.timeScale > 0)
            {
                timer += Time.deltaTime;

                // OPTIMIZATION: Only update UI when the second actually changes
                int currentSecond = Mathf.FloorToInt(timer);
                if (currentSecond != _lastSecond)
                {
                    _lastSecond = currentSecond;
                    OnTimeUpdated?.Invoke(GetFormatedTime());
                }
            }
        }


        public void AddScore(int points)
        {
            score += points;
            OnScoreUpdated?.Invoke(score);
        }

        public int GetCurrentScore()
        {
            return score;
        }

        public string GetCurrentFormattedTime()
        {
            return GetFormatedTime();
        }

        public void ResetAllStats()
        {
            timer = 0f;
            score = 0;

            OnScoreUpdated?.Invoke(score);
            OnTimeUpdated?.Invoke(GetFormatedTime());

            Debug.Log("[PlayGameManager] All stats reset.");
        }

        public void TriggerGameOver()
        {
            if (GameStateManager.instance != null)
            {
                GameStateManager.instance.TriggerGameOver();

                // --- this Save manager thingy need to be modified -----//

                SaveManager saveManager = FindObjectOfType<SaveManager>();


                if (saveManager != null)
                {
                    float finalScore = score;
                    if (ScoreManager.Instance != null)
                    {
                        finalScore = ScoreManager.Instance.Score;
                    }

                    saveManager.AddScoreToLb(finalScore);
                }

                // ------------------------------------------------------//

                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.SubmitFinalScore();
                }
                else
                {
                    Debug.LogError("[PlayGameManager] ScoreManager not found. Score cannot be submitted.");
                }

                GameStateManager.instance.TriggerGameOver();
            }
            else
            {
                Debug.LogError("[PlayGameManager] GameStateManager instance not found. Cannot trigger Game Over!");
            }
        }
        private string GetFormatedTime()
        {
            var minutes = Mathf.FloorToInt(timer / 60);
            var seconds = Mathf.FloorToInt(timer % 60);
            return $"{minutes:00}:{seconds:00}";
        }
    }
}