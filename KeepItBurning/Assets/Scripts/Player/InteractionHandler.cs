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
        private PlayerInventory _inventory;
        private PlayersActivities _playerActivities;
        private IInputService _inputService; 
        private IInteractable _activeInteractable = null;
        private Coroutine _interactionCoroutine = null;

        private void Awake()
        {
            _playerActivities = GetComponent<PlayersActivities>();
            _detector = GetComponent<PlayersInteractionTargetDetector>();
            _inventory = GetComponent<PlayerInventory>();

            if (_detector == null) Debug.LogError("PlayersInteractionTargetDetector missing on InteractionHandler.");
            if (_playerActivities == null) Debug.LogError("PlayersActivities missing on InteractionHandler.");
            if (_inventory == null) Debug.LogError("PlayerInventory missing on InteractionHandler.");
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
            if (_interactionCoroutine != null)
            {
                CancelInteraction(false);
            }
        }

        private void Update()
        {
            if (_interactionCoroutine != null)
            {
                if (_activeInteractable == null)
                {
                    Debug.Log("[INTERACTION: OBJECT DESTROYED] Target was destroyed mid-interaction.");
                    CancelInteraction(false); 
                    return;
                }
                if (_detector.currentInteractable != _activeInteractable || _detector.currentCollectible != null)
                {
                    HandleMovementInterruption(); 
                }
            }
        }
        
        private void HandleInteractionInput()
        {
            if (_playerActivities.currentState == PlayerState.IsInteracting || _playerActivities.currentState == PlayerState.IsChopping)
            {
                CancelInteraction(true); 
                return; 
            }
            
            ICollectible collectible = _detector.currentCollectible;
            if (collectible != null)
            {
                bool success = collectible.Collect(this.gameObject);
                
                if (success)
                {
                    Debug.Log($"[INTERACTION: COLLECTED] Picked up ");
                }
                else
                {
                    Debug.Log($"[INTERACTION: FAILED COLLECT] Could not pick up (Inventory full?)");
                }
                return;
            }
            
            var interactable = _detector.currentInteractable;
            if (interactable != null)
            {
                
                if (interactable is TreeToCut treeToCut)
                {
                    var data = treeToCut.GetInteractionData();

                    if (data.actionDuration > 0f)
                    {
                        _activeInteractable = treeToCut;
                        _interactionCoroutine = StartCoroutine(PerformLongInteraction(data.actionDuration));
                        _playerActivities.SetPlayerState(PlayerState.IsChopping);
                        Debug.Log($"[INTERACTION: LONG START] Starting {data.promptText}. Will take {data.actionDuration} seconds.");
                        return;
                    }
                    
                    Debug.Log($"[INTERACTION: BLOCKED] {data.promptText}. Cannot start action.");
                    return;
                }
            
                // --- Fallback for Instant/Simple Interactions ---
                _playerActivities.SetPlayerState(PlayerState.IsInteracting); 
                interactable.Interact();
                _playerActivities.SetPlayerState(PlayerState.IsIdle); 
                return;
            }
            
            if (_inventory.HasWood)
            {
                _inventory.DropWood();
            }
            else
            {
                Debug.Log("[INTERACTION: NO TARGET] No interactable or collectible target found.");
            }
        }
        
        private IEnumerator PerformLongInteraction(float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            
            Debug.Log("[INTERACTION: SUCCESS] Long interaction timer finished.");
            
            if (_activeInteractable is TreeToCut treeToCut)
            {
                treeToCut.Interact(); 
            }
            
            _activeInteractable = null;
            _interactionCoroutine = null;
            _playerActivities.SetPlayerState(PlayerState.IsIdle);
        }
        
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