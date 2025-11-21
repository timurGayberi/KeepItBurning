using System;
using System.Collections;
using UnityEngine;
using General;
using Interfaces;
using GamePlay.Interactables;
using ScriptableObjects;

namespace Player
{
    public class InteractionHandler : MonoBehaviour
    {
        [Header("Data Sources")]
        [SerializeField] private PlayerStatsSo _playerStats;
        [SerializeField] private ActionConfigSo _chopConfig;

        [Header("Components")]
        private PlayerAnimatorController _animatorController;
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
            _animatorController = GetComponent<PlayerAnimatorController>();

            // Safety Check
            if (_chopConfig == null) Debug.LogError("Chop Config SO is missing on InteractionHandler!");
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
                if (_playerActivities.currentState == PlayerState.IsInteracting) ResetInteractionState();
            }
        }

        private void HandleSinglePressInteraction()
        {
            // USE DATA: Check Cooldown using the SO
            if (Time.time < _lastChopTime + _chopConfig.Cooldown) return;

            if (_detector.currentCollectible != null)
            {
                _detector.currentCollectible.Collect(this.gameObject);
                return;
            }

            var nearby = _detector.GetAllNearbyInteractables();
            if (nearby != null && nearby.Count > 0)
            {
                // Prioritize the closest interactable for chopping
                if (_detector.currentInteractable is TreeToCut tree)
                {
                    if (tree.currentTreeStatus == TreeStatus.Uncut)
                    {
                        StartChopSequence(tree); // New simplified method
                        return;
                    }
                }

                // ... (Your existing standard interaction logic here) ...
                HandleStandardInteractions(nearby);
            }
            else
            {
                if (_inventory.HasWood) _inventory.DropWood();
            }
        }

        private void HandleAutoChop()
        {
            if (!(_detector.currentInteractable is TreeToCut tree)) return;

            if (_inputService.IsInteractPressed)
            {
                // USE DATA: Check Cooldown
                if (Time.time >= _lastChopTime + _chopConfig.Cooldown)
                {
                    // Add check to prevent chopping cut trees
                    if (tree.currentTreeStatus == TreeStatus.Uncut)
                    {
                        StartChopSequence(tree);
                    }
                }
            }
            else
            {
                // USE DATA: Wait for animation to finish
                if (_playerActivities.currentState == PlayerState.IsChopping &&
                    Time.time > _lastChopTime + _chopConfig.Cooldown)
                {
                    _playerActivities.SetPlayerState(PlayerState.IsIdle);
                    _activeInteractable = null;
                }
            }
        }

        private void StartChopSequence(TreeToCut tree)
        {
            // Double check to prevent chopping cut trees
            if (tree.currentTreeStatus != TreeStatus.Uncut) return;

            _activeInteractable = tree;
            _playerActivities.SetPlayerState(PlayerState.IsChopping);
            _lastChopTime = Time.time;

            // 1. Trigger Animation
            if (_animatorController != null) _animatorController.TriggerChopAnimation();

            // 2. Start the Timer based on Data
            StartCoroutine(ExecuteChopImpact(tree));
        }

        private IEnumerator ExecuteChopImpact(TreeToCut tree)
        {
            // USE DATA: Wait exactly as long as the SO says
            yield return new WaitForSeconds(_chopConfig.impactDelay);

            // USE DATA: Play the specific sound defined in the SO
            SoundManager.Play(_chopConfig.impactSound);

            var damage = _playerStats != null ? _playerStats.damageRate : 25f;

            if (tree != null)
            {
                tree.ApplyDamage(damage, this.transform.position);
            }
        }

        private void HandleStandardInteractions(System.Collections.Generic.List<IInteractable> nearby)
        {
            // (Paste your logic for fireplace/tables here to keep file clean)
            bool hasWood = _inventory.HasWood;
            foreach (var interactable in nearby)
            {
                if (interactable is TreeToCut) continue; // Skip trees (handled separately)

                if (hasWood && interactable is FoodTable)
                {
                    Debug.Log("[INTERACTION] Can't take food while holding wood");
                    continue;
                }

                if (interactable is FireplaceInteraction fireplace)
                {
                    fireplace.TryAddFuel(this.gameObject);
                    // Continue to allow other interactions (like user's script)
                    continue;
                }

                _playerActivities.SetPlayerState(PlayerState.IsInteracting);
                _activeInteractable = interactable;
                interactable.Interact();
                ResetInteractionState();
                // Continue to allow other interactions
            }
        }

        private void ResetInteractionState()
        {
            _activeInteractable = null;
            _playerActivities.SetPlayerState(PlayerState.IsIdle);
        }

        public void HandleMovementInterruption()
        {
            if (_playerActivities.currentState == PlayerState.IsChopping) ResetInteractionState();
        }
    }
}