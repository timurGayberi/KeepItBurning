using System;
using System.Collections;
using UnityEngine;
using General;
using Interfaces;
using GamePlay.Interactables; 

namespace Player
{
    public class InteractionHandler : MonoBehaviour
    {
        private PlayersInteractionTargetDetector _detector;
        private PlayersActivities _playerActivities;
        private IInputService _inputService; 
        
        // State management for long interactions
        private IInteractable _activeInteractable = null;
        private Coroutine _interactionCoroutine = null;

        private void Awake()
        {
            _playerActivities = GetComponent<PlayersActivities>();
            _detector = GetComponent<PlayersInteractionTargetDetector>();

            if (_detector == null) Debug.LogError("PlayersInteractionTargetDetector missing on InteractionHandler.");
            if (_playerActivities == null) Debug.LogError("PlayersActivities missing on InteractionHandler.");
        }

        private void OnEnable()
        {
            try
            {
                _inputService = ServiceLocator.GetService<IInputService>();
                _inputService.OnInteractEvent += HandleInteractionInput;
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError("IInputService not found. Error: " + e.Message);
            }
        }

        private void OnDisable()
        {
            if (_inputService != null)
            {
                _inputService.OnInteractEvent -= HandleInteractionInput;
            }
            // Ensure any running coroutine is stopped if the handler is disabled
            if (_interactionCoroutine != null)
            {
                CancelInteraction(false);
            }
        }

        /// <summary>
        /// Continuous check to ensure player stays within range of the target during a long interaction.
        /// </summary>
        private void Update()
        {
            // Only perform the check if a long-running interaction is active (player is chopping)
            if (_interactionCoroutine != null)
            {
                // Check 1: Did the target get destroyed? (Unity null check)
                if (_activeInteractable == null)
                {
                    Debug.Log("[INTERACTION: OBJECT DESTROYED] Target was destroyed mid-interaction.");
                    // We use 'false' because we can't notify a destroyed object.
                    CancelInteraction(false); 
                    return;
                }
                
                // Check 2: Did the player move out of range?
                // The detector should still be reporting the _activeInteractable as the current target.
                // If it's not (it's null or a different target), the player moved.
                if (_detector.currentInteractable != _activeInteractable)
                {
                    HandleMovementInterruption(); 
                }
            }
        }
        
        private void HandleInteractionInput()
        {
            // --- 1. Cancellation Check (If the player is currently busy and presses the button again) ---
            if (_playerActivities.currentState == PlayerState.IsInteracting || _playerActivities.currentState == PlayerState.IsChopping)
            {
                CancelInteraction(true); 
                return; 
            }
            
            // --- 2. Start Interaction Check ---
            IInteractable interactable = _detector.currentInteractable;
            
            if (interactable == null)
            {
                Debug.Log("[INTERACTION: NO TARGET] No interactable target found.");
                return;
            }
            
            // Check if it's a long-running interaction like TreeToCut
            if (interactable is TreeToCut treeToCut)
            {
                InteractionData data = treeToCut.GetInteractionData();

                if (data.actionDuration > 0f)
                {
                    // Start a long interaction (chopping)
                    _activeInteractable = treeToCut;
                    _interactionCoroutine = StartCoroutine(PerformLongInteraction(data.actionDuration));
                    _playerActivities.SetPlayerState(PlayerState.IsChopping);
                    Debug.Log($"[INTERACTION: LONG START] Starting {data.promptText}. Will take {data.actionDuration} seconds.");
                    return;
                }
                
                // If duration is <= 0 (e.g., resource is regrowing), log and block
                Debug.Log($"[INTERACTION: BLOCKED] {data.promptText}. Cannot start action.");
                return;
            }
            
            // --- 3. Fallback for Instant/Simple Interactions ---
            
            _playerActivities.SetPlayerState(PlayerState.IsInteracting); 
            
            interactable.Interact();
            
            _playerActivities.SetPlayerState(PlayerState.IsIdle); 
        }
        
        private IEnumerator PerformLongInteraction(float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            // Interaction completed successfully!
            Debug.Log("[INTERACTION: SUCCESS] Long interaction timer finished.");
            
            if (_activeInteractable is TreeToCut treeToCut)
            {
                treeToCut.Interact(); 
            }
            
            // Clean up state
            _activeInteractable = null;
            _interactionCoroutine = null;
            _playerActivities.SetPlayerState(PlayerState.IsIdle);
        }
        
        /// <summary>
        /// Stops the currently active interaction timer and resets player state.
        /// </summary>
        /// <param name="notifyTarget">If true, calls StopInteraction() on the target.</param>
        private void CancelInteraction(bool notifyTarget)
        {
            if (_interactionCoroutine != null)
            {
                StopCoroutine(_interactionCoroutine);
                _interactionCoroutine = null;
                Debug.Log($"[INTERACTION: CANCELLATION] Interaction timer stopped. State reset from {_playerActivities.currentState}.");
            }
            
            if (notifyTarget && _activeInteractable != null)
            {
                _activeInteractable.StopInteraction(); 
            }

            _activeInteractable = null;
            _playerActivities.SetPlayerState(PlayerState.IsIdle);
        }
        
        /// <summary>
        /// Public method to interrupt interaction, typically called by player movement or proximity checks.
        /// </summary>
        public void HandleMovementInterruption()
        {
            if (_interactionCoroutine != null)
            {
                Debug.Log("[INTERACTION: MOVEMENT CANCEL] Interaction interrupted due to movement.");
                CancelInteraction(true);
            }
        }
    }
}