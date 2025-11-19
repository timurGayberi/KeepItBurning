using System;
using System.Collections.Generic;
using UnityEngine;
using General;
using Interfaces;
using GamePlay.Interactables; 
using ScriptableObjects; 

namespace Player
{
    public class InteractionHandler : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private PlayerStatsSo _playerStats;

        [Header("Settings")]
        [SerializeField] private float _chopCooldown = 0.6f; 

        private PlayersInteractionTargetDetector _detector;
        private PlayerInventory _inventory;
        private PlayersActivities _playerActivities;
        private IInputService _inputService; 
        
        private IInteractable _activeInteractable = null;
        private float _lastChopTime;

        private void Awake()
        {
            _playerActivities = GetComponent<PlayersActivities>();
            _detector = GetComponent<PlayersInteractionTargetDetector>();
            _inventory = GetComponent<PlayerInventory>();
        }

        private void OnEnable()
        {
            try
            {
                _inputService = ServiceLocator.GetService<IInputService>();
                _inputService.OnInteractEvent += HandleSinglePressInteraction;
            }
            catch (Exception e) { Debug.LogError(e); }
        }

        private void OnDisable()
        {
            if (_inputService != null) _inputService.OnInteractEvent -= HandleSinglePressInteraction;
        }

        private void Update()
        {
            HandleAutoChop();
            
            if (_activeInteractable != null && _detector.currentInteractable != _activeInteractable)
            {
                if(_playerActivities.currentState == PlayerState.IsInteracting)
                {
                    ResetInteractionState();
                }
            }
        }

        // --- FIXED LOGIC HERE ---
        private void HandleSinglePressInteraction()
        {
            // 1. Check Collectibles (Highest Priority)
            if (_detector.currentCollectible != null)
            {
                _detector.currentCollectible.Collect(this.gameObject);
                return; 
            }

            var nearby = _detector.GetAllNearbyInteractables();
            bool interactionSuccessful = false;

            // 2. Check Nearby Objects
            if (nearby != null && nearby.Count > 0)
            {
                // A. Check Trees first
                foreach (var item in nearby)
                {
                    if (item is TreeToCut tree)
                    {
                        PerformChop(tree);
                        return; // Stop here, we chopped
                    }
                }

                // B. Check Standard Interactables
                bool hasWood = _inventory.HasWood;
                
                foreach (var interactable in nearby)
                {
                    // Logic: If we have wood, we skip FoodTables so we can drop the wood instead
                    if (hasWood && interactable is FoodTable) continue;
                    
                    // Standard Interaction
                    if (interactable is FireplaceInteraction fireplace)
                    {
                        fireplace.TryAddFuel(this.gameObject);
                        interactionSuccessful = true;
                        break; 
                    }
                    
                    // Generic Interaction
                    _playerActivities.SetPlayerState(PlayerState.IsInteracting);
                    _activeInteractable = interactable;
                    interactable.Interact();
                    ResetInteractionState();
                    
                    interactionSuccessful = true;
                    break;
                }
            }

            // 3. FALLBACK: Drop Wood
            // If we didn't collect anything, and we didn't successfully interact with anything...
            if (!interactionSuccessful)
            {
                if (_inventory.HasWood)
                {
                    _inventory.DropWood();
                }
                else
                {
                    Debug.Log("[INTERACTION] Nothing to interact with, and nothing to drop.");
                }
            }
        }

        private void HandleAutoChop()
        {
            if (!(_detector.currentInteractable is TreeToCut tree)) return;

            if (_inputService.IsInteractPressed)
            {
                if (Time.time >= _lastChopTime + _chopCooldown)
                {
                    PerformChop(tree);
                }
            }
            else
            {
                if (_playerActivities.currentState == PlayerState.IsChopping && 
                    Time.time > _lastChopTime + _chopCooldown)
                {
                    _playerActivities.SetPlayerState(PlayerState.IsIdle);
                    _activeInteractable = null;
                }
            }
        }

        private void PerformChop(TreeToCut tree)
        {
            _activeInteractable = tree;
            _playerActivities.SetPlayerState(PlayerState.IsChopping);
            _lastChopTime = Time.time;

            SoundManager.Play(SoundAction.ChopWood);

            var damage = _playerStats != null ? _playerStats.damageRate : 25f;
            
            tree.ApplyDamage(damage, transform.position);
        }

        private void ResetInteractionState()
        {
            _activeInteractable = null;
            _playerActivities.SetPlayerState(PlayerState.IsIdle);
        }
        
        public void HandleMovementInterruption()
        {
            if (_playerActivities.currentState == PlayerState.IsChopping)
            {
                ResetInteractionState();
            }
        }
    }
}