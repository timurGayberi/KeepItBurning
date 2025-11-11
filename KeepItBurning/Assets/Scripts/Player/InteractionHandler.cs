using System;
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
        }
        
        private void HandleInteractionInput()
        {
            if ( _playerActivities.currentState == PlayerState.IsInteracting ||  _playerActivities.currentState == PlayerState.IsChopping)
            {
                Debug.Log($"[INTERACTION: CANCELLATION] Player interaction state reset to Idle from { _playerActivities.currentState}.");
                
                if (_detector.currentInteractable is TreeToCut treeToCut)
                {
                    treeToCut.StopInteraction(); 
                }
                
                _playerActivities.SetPlayerState(PlayerState.IsIdle);
                return; 
            }
            
            IInteractable interactable = _detector.currentInteractable;
            
            if (interactable != null)
            {
                
                Debug.Log($"[INTERACTION: START] Starting long interaction with: {interactable.InteractionPrompt}.");
                
                _playerActivities.SetPlayerState(PlayerState.IsInteracting); 
                
                interactable.Interact(gameObject); 
                
                return; 
            }
            
            Debug.Log("[INTERACTION: NO TARGET] No interactable target found.");
        }
    }
}