using GamePlay.Collectibles;
using Interfaces;
using Player;
using System;
using UnityEngine;

namespace GamePlay.Interactables
{
    public class FireplaceInteraction : MonoBehaviour, IInteractable
    {
        public static event Action OnFireplaceOut;

        #region Variables

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

        /*
        [Header("VFX References")]
        [Tooltip("The CampfireVFXController controlling the fire visuals.")]
        [SerializeField]
        private CampfireVFXController vfxController;
        */

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

        /*
        private void UpdateVFXController()
        {
            if (vfxController != null)
            {
                float normalizedFuel = _currentFuel / maxFuel;
                vfxController.SetFuelNormalized(normalizedFuel);
            }
        }
        */

        private void Update()
        {
            if (_currentFuel > 0)
            {
                _currentFuel -= decayRate * Time.deltaTime;
                _currentFuel = Mathf.Max(0, _currentFuel);

                OnFuelChanged?.Invoke(_currentFuel, maxFuel);
                //UpdateVFXController();
                UpdateScore();

                if (_currentFuel <= 0)
                {
                    Debug.Log("The campfire has gone out. Triggering Game Over.");

                    /*
                    if (vfxController != null)
                    {
                        vfxController.ShutDownFire();
                    }
                    */

                    OnFireplaceOut?.Invoke();

                    enabled = false;
                }
            }
        }

        // The core logic to check inventory, consume wood, and add fuel
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

                    OnFuelChanged?.Invoke(_currentFuel, maxFuel);

                    //UpdateVFXController();

                    // Re-enable the Update loop if fuel was added when it was previously zero
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
                // This is still useful if the player interacts but isn't carrying wood.
                Debug.Log("[CAMPFIRE] Interaction attempted, but player is not carrying wood.");
            }
        }
        
        private float GetFuelValueFromInventory(PlayerInventory inventory)
        {
            return inventory.GetWoodFuelValue();
        }

        private void UpdateScore()
        {
            if (_currentFuel > thresholdToScore)
            {
                //ScoreManager.Instance.AddScore(baseCampfireScore);
            }
        }

        /*
        private float GetFuelFromCarriedLog(PlayerInventory inventory)
        {
            // ... (GetFuelFromCarriedLog logic remains the same)
            GameObject carriedLog = inventory.GetCarriedWoodInstance();
            if (carriedLog == null)
            {
                Debug.LogError("[FUEL GET] Carried wood instance is NULL.");
                return 0f;
            }

            // Assuming FireWoodLogs component exists on the carried log object
            FireWoodLogs logComponent = carriedLog.GetComponent<FireWoodLogs>();

            if (logComponent == null)
            {
                Debug.LogError("[FUEL GET] Carried log is missing FireWoodLogs component.");
                return 0f;
            }
            return logComponent.FuelValue;
        }
        */

        // --- New public method for InteractionHandler to call ---
        public void TryAddFuel(GameObject interactor)
        {
            AddFuelFromInteractor(interactor);
        }

        // --- IInteractable Implementation (Required by the Interface) ---

        // The Interact() required by the interface will now just call the robust logic
        // if this fireplace is interacted with, relying on InteractionHandler to call TryAddFuel.
        // If this method is called directly by a non-Player object, it logs an error.
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