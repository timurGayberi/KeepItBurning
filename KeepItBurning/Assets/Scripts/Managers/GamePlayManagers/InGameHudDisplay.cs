using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Player;
using General; 
using System;
using GamePlay.Interactables;

namespace Managers.GamePlayManagers
{
    public class InGameHudDisplayComponent : MonoBehaviour
    {
        [Header("UI References")]
        
        [Tooltip("The TextMeshPro element displaying the current player score.")]
        public TextMeshProUGUI scoreText;
        
        public TextMeshProUGUI timeText;
        
        public TextMeshProUGUI carriedLogsNumber;

        [Header("Fireplace UI")]
        [Tooltip("The slider representing the current fireplace fuel level.")]
        public Slider fireFuelSlider;
        
        private PlayerInventory playerInventory;
        private FireplaceInteraction fireplaceInteraction;

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

                // --- Player Inventory Setup ---
                
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
                
                SetupFireplaceListener();
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
            // Fireplace Cleanup (New)
            if (fireplaceInteraction != null)
            {
                fireplaceInteraction.OnFuelChanged -= UpdateFireFuelSlider;
            }
        }

        private void SetupFireplaceListener()
        {
            fireplaceInteraction = FindObjectOfType<FireplaceInteraction>();

            if (fireplaceInteraction != null)
            {
                fireplaceInteraction.OnFuelChanged += UpdateFireFuelSlider;
            }
            else
            {
                Debug.LogWarning("FireplaceInteraction not found in the scene. Fire fuel slider will not update.");
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

        // New: Update the slider based on the fireplace fuel
        private float _targetFuelValue;
        private float _currentDisplayFuel;

        private void Update()
        {
            if (fireFuelSlider != null)
            {
                // OPTIMIZATION: Smoothly interpolate the visual slider value
                // This makes the 10Hz data updates look like 60Hz smooth motion
                if (Mathf.Abs(fireFuelSlider.value - _targetFuelValue) > 0.01f)
                {
                    fireFuelSlider.value = Mathf.Lerp(fireFuelSlider.value, _targetFuelValue, Time.deltaTime * 5f);
                }
                else
                {
                    fireFuelSlider.value = _targetFuelValue;
                }
            }
        }

        private void UpdateFireFuelSlider(float currentFuel, float maxFuel)
        {
            if (fireFuelSlider != null)
            {
                // Set max value once at initialization
                if (fireFuelSlider.maxValue != maxFuel)
                {
                    fireFuelSlider.maxValue = maxFuel;
                    fireFuelSlider.value = currentFuel; // Snap immediately on first set
                    _targetFuelValue = currentFuel;
                }
                
                // Set target value for smooth interpolation
                _targetFuelValue = currentFuel;
            }
        }
    }
}
