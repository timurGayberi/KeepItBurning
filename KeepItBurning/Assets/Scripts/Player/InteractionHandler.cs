/*
using System;
using General;
using Interfaces;
using UnityEngine;

namespace Player
{
    public class InteractionHandler : MonoBehaviour
    {
        // References (assigned in Awake)
        private InteractionTargetDetector _detector;
        private PlayerMovement _playerMovement;
        // private PlayerInventory _playerInventory; // Use this when available
        private IInputService _inputService; 

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            // _playerInventory = GetComponent<PlayerInventory>();
            _detector = GetComponent<InteractionTargetDetector>();

            if (_detector == null) Debug.LogError("InteractionHandler requires an InteractionTargetDetector component.");
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
                Debug.LogError("IInputService not found. Is InputReader registered in Awake? Error: " + e.Message);
            }
        }

        private void OnDisable()
        {
            if (_inputService != null)
            {
                _inputService.OnInteractEvent -= HandleInteractionInput;
            }
        }
        
        private void HandleInteractionInput()
        {
            // --- PRIORITY 0: CANCEL INTERACTION ---
            if (_playerMovement.CurrentState == PlayerState.IsInteracting || _playerMovement.CurrentState == PlayerState.IsChopping)
            {
                Debug.Log($"[DEBUG: CANCELLATION] Player interaction state reset to Idle from {_playerMovement.CurrentState}.");
                //_playerMovement.SetPlayerState(PlayerState.IsIdle);
                return; 
            }
            
            // Re-fetch targets on input to ensure the list is fresh (optional, but safer)
            // _detector.UpdateInteractionCandidate(forceUpdate: true); 

            IInteractable interactable = _detector.CurrentInteractable;
            //ITreeTarget chopTarget = _detector.CurrentChopTarget;
            //ICollectible collectible = _detector.CurrentCollectible;
            
            // --- PRIORITY 1: GENERAL INTERACTABLE (TENT, FIREPLACE, ETC.) ---
            if (interactable != null)
            {
                // NOTE: Wood check logic will be restored here once PlayerInventory is active
                bool interactionAllowed = true; 

                if (interactionAllowed)
                {
                    Debug.Log($"[DEBUG: INTERACT] Starting long interaction with: {interactable.InteractionPrompt}.");
                    
                    _playerMovement.SetPlayerState(PlayerState.IsInteracting); // Generic long interaction state
                    interactable.Interact(gameObject, _playerMovement);
                }
                else
                {
                    Debug.Log("[DEBUG: INTERACT FAIL] Cannot interact. Player lacks required inventory item.");
                }
                return; 
            }
            
            // --- PRIORITY 2: CHOPPING (Requires Axe) ---
            // if (chopTarget != null && _playerInventory.HasAxe) // When inventory is ready
            if (chopTarget != null) // Placeholder for now
            {
                Debug.Log($"[DEBUG: CHOP] Starting wood chopping interaction with: {chopTarget.InteractionPrompt}.");
                
                _playerMovement.SetPlayerState(PlayerState.IsChopping); // Specific chopping state
                chopTarget.Chop(gameObject, _playerMovement);
                
                return;
            }

            // --- PRIORITY 3: INVENTORY DROP ACTIONS ---
            // This entire block handles drops and requires PlayerInventory

            /*
            if (_playerInventory.HasLantern)
            {
                // Drop Lantern Logic
                return;
            }
            if (_playerInventory.HasAxe && chopTarget == null)
            {
                // Drop Axe Logic
                return;
            }
            if (_playerInventory.HasWood)
            {
                // Drop Wood Logic
                return;
            }

            // --- PRIORITY 4: INSTANT COLLECTIBLE ---
            if (collectible != null)
            {
                Debug.Log($"[DEBUG: COLLECT] Executing instant collection on: {collectible.CollectionPrompt}.");
                collectible.Collect(gameObject, _playerMovement); 
                
                // Force prompt update after collection
                _detector.OnInteractionPromptChange?.Invoke(null);
                return;
            }
            
            // --- PRIORITY 5: Failed Chop (No Axe) ---
            if (chopTarget != null /* && !_playerInventory.HasAxe )
            {
                Debug.Log("[DEBUG: CHOP FAIL] Cannot chop. Player lacks axe.");
                return;
            }
        }
    }
}
*/