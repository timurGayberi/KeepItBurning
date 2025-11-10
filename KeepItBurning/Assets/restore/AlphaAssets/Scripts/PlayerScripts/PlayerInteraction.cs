using UnityEngine;
using Interfaces;
using System;
using General;

namespace PlayerScripts
{
    public class PlayerInteraction : MonoBehaviour
    {
        #region References & Data
        
        [Header("Interaction Settings")]
        [Tooltip("Max distance the player can interact from.")]
        [SerializeField] private float interactionDistance = 2f;
        
        [Tooltip("The tag used to identify general interactable objects (e.g., Tents).")]
        [SerializeField] private string interactableTag = "Interactable";

        [Tooltip("The tag used to identify instant collectible objects (e.g., axe, lantern).")]
        [SerializeField] private string collectibleTag = "Collectible";
        
        [Tooltip("The tag used to identify objects that require chopping (e.g., trees).")]
        [SerializeField] private string chopTargetTag = "ChopTarget";
        
        private PlayerMovement _playerMovement;
        private PlayerInventory _playerInventory;
        private IInputService _inputService;
        private IInteractable _currentInteractable;
        private ICollectible _currentCollectible;
        private ITreeTarget _currentChopTarget;
        
        public event Action<string> OnInteractionPromptChange; 
        
        #endregion
        
        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _playerInventory = GetComponent<PlayerInventory>();
            
            if (_playerMovement == null)
            {
                Debug.LogError("PlayerInteraction requires a PlayerMovement component on the same GameObject.");
            }
            if (_playerInventory == null)
            {
                Debug.LogError("PlayerInteraction requires a PlayerInventory component on the same GameObject.");
            }
            
            if (string.IsNullOrEmpty(interactableTag) || string.IsNullOrEmpty(collectibleTag) || string.IsNullOrEmpty(chopTargetTag))
            {
                Debug.LogWarning("One or more interaction tags are not set. Please set them in the Inspector.", this);
            }
        }
        
        private void UpdateInteractionCandidate(bool forceUpdate = false)
        {
            if (_playerMovement.CurrentState == PlayerState.IsInteracting && !forceUpdate)
            {
                return;
            }

            IInteractable closestInteractable = null;
            ICollectible closestCollectible = null;
            ITreeTarget closestChopTarget = null;
            string newPrompt = null;
            
            var colliders = Physics.OverlapSphere(transform.position, interactionDistance);

            var minDistance = float.MaxValue;
            
            foreach (var collider in colliders)
            {
                var distance = Vector3.Distance(transform.position, collider.transform.position);

                if (distance < minDistance)
                {
                    if (collider.CompareTag(collectibleTag) && collider.TryGetComponent(out ICollectible collectible))
                    {
                        minDistance = distance;
                        closestCollectible = collectible;
                        closestInteractable = null;
                        closestChopTarget = null;
                        newPrompt = collectible.CollectionPrompt;
                    }
                    else if (collider.CompareTag(chopTargetTag) && collider.TryGetComponent(out ITreeTarget chopTarget))
                    {
                        if (chopTarget is MonoBehaviour chopMb && !chopMb.enabled)
                        {
                            continue; 
                        }
                        minDistance = distance;
                        closestChopTarget = chopTarget;
                        closestCollectible = null;
                        closestInteractable = null;
                        newPrompt = _playerInventory.HasAxe ? "Chop Wood" : "Need Axe to Chop"; 
                    }
                    else if (collider.CompareTag(interactableTag) && collider.TryGetComponent(out IInteractable interactable))
                    {
                        minDistance = distance;
                        closestInteractable = interactable;
                        closestCollectible = null;
                        closestChopTarget = null;
                        newPrompt = interactable.InteractionPrompt;
                    }
                }
            }
            
            if (closestInteractable != _currentInteractable || closestCollectible != _currentCollectible || closestChopTarget != _currentChopTarget)
            {
                _currentInteractable = closestInteractable;
                _currentCollectible = closestCollectible;
                _currentChopTarget = closestChopTarget;


                if (_currentInteractable != null)
                {
                    OnInteractionPromptChange?.Invoke(_currentInteractable.InteractionPrompt);
                }

                else if (_playerInventory.HasLantern)
                {
                    OnInteractionPromptChange?.Invoke("Drop Lantern");
                }
                else if (_playerInventory.HasAxe)
                {
                    if (_currentChopTarget != null) 
                    {
                        OnInteractionPromptChange?.Invoke("Chop Wood"); 
                    }
                    else
                    {
                        OnInteractionPromptChange?.Invoke("Drop Axe");
                    }
                }
                else if (_playerInventory.HasWood) 
                {
                    OnInteractionPromptChange?.Invoke("Drop Wood");
                }
                else
                {
                    OnInteractionPromptChange?.Invoke(newPrompt);
                }
            }
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
            if (_playerMovement.CurrentState == PlayerState.IsInteracting)
            {
                Debug.Log("[DEBUG: CANCELLATION] Player interaction state reset to Idle.");
                _playerMovement.SetPlayerState(PlayerState.IsIdle);
                return; 
            }
            
            UpdateInteractionCandidate(forceUpdate: true);
            
            // PRIORITY 1: GENERAL INTERACTABLE (TENT, FIREPLACE, ETC.)
            if (_currentInteractable != null)
            {
                // The Tent ("Tent" prompt) does NOT require wood. 
                // Any other interactable (like a Fireplace) is assumed to require wood.
                bool isWoodRequired = (_currentInteractable.InteractionPrompt != "Tent"); 
                
                // Interaction is allowed if wood is NOT required, OR if wood IS required and the player has it.
                bool interactionAllowed = !isWoodRequired || _playerInventory.HasWood;

                if (interactionAllowed)
                {
                    Debug.Log($"[DEBUG: INTERACT] Starting long interaction with: {_currentInteractable.InteractionPrompt}. Player state locked to IsInteracting.");
                    
                    _playerMovement.SetPlayerState(PlayerState.IsInteracting);
                    _currentInteractable.Interact(gameObject, _playerMovement);
                }
                else
                {
                    // Log failure for the wood-requiring interaction only.
                    Debug.Log("[DEBUG: INTERACT FAIL] Cannot interact. Player lacks required inventory item (wood).");
                }
                
                // CRITICAL: Stop processing other actions (like dropping axes/logs) below.
                return; 
            }
            
            // The rest of the logic handles inventory drops, chopping, and collecting.

            if (_playerInventory.HasLantern)
            {
                Debug.Log("[DEBUG: DROP] Dropping the currently held lantern.");
                Vector3 dropPosition = transform.position + transform.forward * 1f; 
                _playerInventory.DropLantern(dropPosition);
                OnInteractionPromptChange?.Invoke(null); 
                return;
            }
            
            if (_currentChopTarget != null && _playerInventory.HasAxe)
            {
                Debug.Log($"[DEBUG: CHOP] Starting wood chopping interaction with: {_currentChopTarget.InteractionPrompt}. Player state locked to IsInteracting.");
                
                _playerMovement.SetPlayerState(PlayerState.IsInteracting);
                _currentChopTarget.Chop(gameObject, _playerMovement);
                
                return;
            }
            
            if (_playerInventory.HasAxe && _currentChopTarget == null)
            {
                Debug.Log("[DEBUG: DROP] Dropping the currently held axe.");
                
                Vector3 dropPosition = transform.position + transform.forward * 1f; 
                _playerInventory.DropAxe(dropPosition);
                
                OnInteractionPromptChange?.Invoke(null); 
                return;
            }
            
            if (_playerInventory.HasWood)
            {
                Debug.Log("[DEBUG: DROP] Dropping the currently held wood log.");
    
                Vector3 dropPosition = transform.position + transform.forward * 1f; 
                _playerInventory.DropWood(dropPosition); 
    
                OnInteractionPromptChange?.Invoke(null); 
                return;
            }
            
            if (_currentCollectible != null)
            {
                Debug.Log($"[DEBUG: COLLECT] Executing instant collection on: {_currentCollectible.CollectionPrompt}. Player state remains controllable (Idle/Moving).");
                _currentCollectible.Collect(gameObject, _playerMovement); 
                _currentCollectible = null;
                OnInteractionPromptChange?.Invoke(null);
                return;
            }
            
            if (_currentChopTarget != null && !_playerInventory.HasAxe)
            {
                Debug.Log("[DEBUG: CHOP FAIL] Cannot chop. Player lacks axe.");
                return;
            }
        }
        private void Update()
        {
            UpdateInteractionCandidate();
        }
    }
}
