using Interfaces;
using UnityEngine;

namespace GamePlay.Interactables
{
    public class FireplaceInteraction : MonoBehaviour, IInteractable
    {
        //public static event Action OnFireplaceOut; // Keep commented for now

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
        [Tooltip("The standard fuel value added per log.")]
        [SerializeField]
        private float fuelPerLog = 25f;

        [Header("Current Status")]
        [Tooltip("The current fuel level (displayed at runtime).")]
        [SerializeField]
        private float _currentFuel;

        /* (VFX and Scoring References kept commented out for future use)
        [Header("VFX References")]
        [SerializeField] private CampfireVFXController vfxController;
        [Space]
        [SerializeField] private float baseCampfireScore;
        [SerializeField] private float thresholdToScore = 50f;
        */

        #endregion

        //public event Action<float, float> OnFuelChanged; // Keep commented for future UI

        private void Awake()
        {
            // Initializing fuel to max to start the fire
            _currentFuel = maxFuel; 
            // OnFuelChanged?.Invoke(_currentFuel, maxFuel);
        }

        private void UpdateVFXController()
        {
            // if (vfxController != null) { ... }
        }

        private void Update()
        {
            // --- UNCOMMENTED DECAY LOGIC ---
            if (_currentFuel > 0)
            {
                _currentFuel -= decayRate * Time.deltaTime;
                _currentFuel = Mathf.Max(0, _currentFuel);

                // OnFuelChanged?.Invoke(_currentFuel, maxFuel);
                UpdateVFXController();
                // UpdateScore();

                if (_currentFuel <= 0)
                {
                    Debug.Log("The campfire has gone out.");
                    // OnFireplaceOut?.Invoke();
                    enabled = false;
                }
            }
        }
        
        public bool AddFuel()
        {
            if (_currentFuel >= maxFuel)
            {
                Debug.Log("[FIREPLACE] Cannot add fuel, already full.");
                return false;
            }
            
            _currentFuel += fuelPerLog;
            _currentFuel = Mathf.Min(maxFuel, _currentFuel);

            // Re-enable the Update loop if fire was out
            if (!enabled)
            {
                enabled = true;
            }

            // OnFuelChanged?.Invoke(_currentFuel, maxFuel);
            UpdateVFXController();
            EnableFireParticles();
            Debug.Log($"[FIREPLACE] Added {fuelPerLog} fuel. Current Fuel: {_currentFuel:F1}/{maxFuel}.");
            return true;
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
        public InteractionData GetInteractionData()
        {
            return new InteractionData
            {
                promptText = InteractionPrompt,
                actionDuration = 0f 
            };
        }
        
        public void StopInteraction()
        {
            // 
        }

        public void Interact()
        {
            Debug.Log("[FIREPLACE] Interact called. The InteractionHandler should manage wood consumption and call AddFuel() directly.");
        }
        
        
        // This method is no longer needed since the Handler consumes the wood.
        /*
        private float GetFuelFromCarriedLog(PlayerInventory inventory)
        {
            // ... (old logic removed) ...
            return 0f;
        }

        public InteractionData GetInteractionData()
        {
            // Return data for an instant interaction (duration 0)
            return new InteractionData(InteractionPrompt, 0f);
        }
        
        public void StopInteraction() { } // Required by IInteractable
        */
        
    }
}