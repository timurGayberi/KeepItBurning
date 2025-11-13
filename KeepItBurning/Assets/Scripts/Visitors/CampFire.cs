using Interfaces;
using Player;
using System;
using Managers.GamePlayManagers;
using UnityEngine;
using General;

namespace GamePlay.Interactables
{
    public class FireplaceInteraction : MonoBehaviour, IInteractable
    {
        public static event Action OnFireplaceOut;

        #region Variables
        [Header("Fire Particles")]
        [SerializeField] private ParticleSystem[] fireParticles;
        [Header("Interaction Settings")]
        [Tooltip("The text prompt shown to the player when near the fireplace.")]
        [SerializeField]
        private string interactionPrompt = "Add Wood to Fireplace";
        public string InteractionPrompt => interactionPrompt;

        [Header("Fire Status")]
        [Tooltip("The maximum amount of fuel the campfire can hold.")]
        [SerializeField]
        private float maxFuel = 100f;
        [Tooltip("The rate at which fuel decays per second.")]
        [SerializeField]
        private float decayRate = 1f;

        [Header("Current Status")]
        [Tooltip("The current fuel level (displayed at runtime).")]
        [SerializeField]
        private float _currentFuel;


        [Header("VFX References")]
        [Tooltip("The CampfireVFXController controlling the fire visuals.")]
        [SerializeField]
        private CampfireVFXController vfxController;
        

        [Space]
        [SerializeField] private float baseCampfireScore;
        [SerializeField] private float thresholdToScore = 50f;


        #endregion

        public event Action<float, float> OnFuelChanged;

        private void Awake()
        {
            _currentFuel = maxFuel;
            OnFuelChanged?.Invoke(_currentFuel, maxFuel);
        }

        private void UpdateVFXController()
        {
            if (vfxController != null)
            {
                float normalizedFuel = _currentFuel / maxFuel;
                vfxController.SetFuelNormalized(normalizedFuel);
            }
        }

        private void EnableFireParticles()
        {
            if (fireParticles == null || fireParticles.Length == 0) return;

            foreach (var ps in fireParticles)
            {
                if (ps == null) continue;

                if (!ps.gameObject.activeSelf)
                {
                    ps.gameObject.SetActive(true);
                }

                if (!ps.isPlaying)
                {
                    ps.Play();
                }
            }
        }

        private void Update()
        {
            if (_currentFuel > 0)
            {
                _currentFuel -= decayRate * Time.deltaTime;
                _currentFuel = Mathf.Max(0, _currentFuel);

                OnFuelChanged?.Invoke(_currentFuel, maxFuel);
                UpdateVFXController();
                UpdateScore();

                if (_currentFuel <= 0)
                {
                    Debug.Log("The campfire has gone out. Triggering Game Over.");

                    OnFireplaceOut?.Invoke();
                    
                    // FIX: Must check the static Instance and call the method on the instance.
                    if (PlayGameManager.Instance != null) 
                    {
                        PlayGameManager.Instance.TriggerGameOver(); 
                    }
                    else
                    {
                        Debug.LogError("PlayGameManager instance not found. Cannot trigger Game Over!");
                    }

                    enabled = false;
                }
            }
        }
        
        private void AddFuelFromInteractor(GameObject interactor)
        {
            PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();

            if (inventory == null)
            {
                Debug.LogError("PlayerInventory component not found on interactor!");
                return;
            }

            if (inventory.HasWood)
            {
                float fuelToAdd = GetFuelValueFromInventory(inventory);

                if (fuelToAdd > 0)
                {
                    _currentFuel += fuelToAdd;
                    _currentFuel = Mathf.Min(maxFuel, _currentFuel);

                    // ConsumeWood() handles decrementing the count
                    inventory.ConsumeWood();
                    EnableFireParticles();
                    OnFuelChanged?.Invoke(_currentFuel, maxFuel);

                    UpdateVFXController();
                    
                    if (!enabled)
                    {
                        enabled = true;
                    }

                    Debug.Log($"[CAMPFIRE] Added {fuelToAdd} fuel. Current Fuel: {_currentFuel:F1}/{maxFuel}.");
                }
                else
                {
                    Debug.LogWarning("[CAMPFIRE] Log carried has zero fuel value, or the inventory failed to provide it.");
                }
            }
            else
            {
                Debug.Log("[CAMPFIRE] Interaction attempted, but player is not carrying wood.");
            }
        }
        
        private float GetFuelValueFromInventory(PlayerInventory inventory)
        {
            // Requires PlayerInventory.GetWoodFuelValue() which is assumed to be correct now.
            return inventory.GetWoodFuelValue();
        }

        private void UpdateScore()
        {
            if (_currentFuel > thresholdToScore)
            {
                //ScoreManager.Instance.AddScore(baseCampfireScore);
            }
        }
        
        public void TryAddFuel(GameObject interactor)
        {
            AddFuelFromInteractor(interactor);
        }
        
        public void Interact()
        {
            Debug.LogWarning("[FIREPLACE] Standard Interact() called. Ensure the player's InteractionHandler is calling TryAddFuel(GameObject) instead.");
        }

        public InteractionData GetInteractionData()
        {
            return new InteractionData { promptText = interactionPrompt, actionDuration = 0f };
        }

        public void StopInteraction()
        {
            // Nothing to stop for an immediate fireplace interaction
        }
    }
}