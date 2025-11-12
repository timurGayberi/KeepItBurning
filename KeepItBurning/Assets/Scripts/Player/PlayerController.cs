using System;
using General;
using Interfaces;
using UnityEngine;
using ScriptableObjects;

namespace Player
{
    
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        #region References & Data
        
        [Header("Players Stats")] 
        [SerializeField]
        private PlayerStatsSo data;
        
        private CharacterController _characterController;
        private Vector3 _currentMoveDirection; 
        private bool _isSprinting;
        private IInputService _inputService; 
        
        // private PlayerInventory _playerInventory; 
        
        #endregion
        
        //  ------------------Actions-------------------------//
        
        public event Action OnInteractionAttempt; 
        public event Action <PlayerState> OnPlayerStateChange;
        
        // --------------------------------------------------//
        
        public PlayerState CurrentState { get; private set; } = PlayerState.IsIdle;
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            
            if (_characterController == null)
            {
                Debug.LogError("PlayerMovement requires a CharacterController component.");
            }
            if (data == null)
            {
                Debug.LogError("MovementData (PlayerStatsSo) is not assigned to PlayerMovement. Movement will fail.");
            }
        }
        
        private void OnEnable()
        {
            try
            {
                _inputService = ServiceLocator.GetService<IInputService>();
                _inputService.OnMoveEvent += HandleMoveInput; 
                _inputService.OnSprintStarted += HandleSprintStarted;
                _inputService.OnSprintCanceled += HandleSprintCanceled;
                
                OnPlayerStateChange?.Invoke(CurrentState); 
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
                _inputService.OnMoveEvent -= HandleMoveInput;
                _inputService.OnSprintStarted -= HandleSprintStarted;
                _inputService.OnSprintCanceled -= HandleSprintCanceled;
            }
        }

        private void SetPlayerState(PlayerState newState)
        {
            if (CurrentState != newState)
            {
                CurrentState = newState;
                OnPlayerStateChange?.Invoke(CurrentState); 
            }
        }

        private void HandleMoveInput(Vector2 inputVector)
        {
            if (CurrentState != PlayerState.IsInteracting) 
            {
                _currentMoveDirection = new Vector3(inputVector.x, 0f, inputVector.y).normalized;
            }
        }

        private void HandleSprintStarted()
        {
            if (CurrentState != PlayerState.IsInteracting)
            {
                _isSprinting = true;
            }
        }

        private void HandleSprintCanceled()
        {
            _isSprinting = false;
        }
        
        private void Update()
        {
            if (_characterController == null || data == null) return;
            
            var moveDirection = _currentMoveDirection;
            float currentSpeed;
            
            if (CurrentState == PlayerState.IsInteracting)
            {
                _characterController.Move(Vector3.zero); 
                return; 
            }
            
            // --- MOVEMENT AND STATE LOGIC ---

            if (moveDirection.sqrMagnitude > 0.01f) // Player is moving
            {
                // --- SPRINTING CHECK ---
                if (_isSprinting)
                {
                    currentSpeed = data.sprintSpeed;
                    if (CurrentState != PlayerState.IsSprinting) SetPlayerState(PlayerState.IsSprinting);
                }
                // --- WALKING CHECK ---
                else 
                {
                    currentSpeed = data.movementSpeed;
                    if (CurrentState != PlayerState.IsWalking) SetPlayerState(PlayerState.IsWalking); 
                }
                
                // --- EXECUTE MOVEMENT ---
                // This line applies the movement speed to the cached direction vector
                _characterController.Move(moveDirection * (currentSpeed * Time.deltaTime));
                
                // --- HANDLE ROTATION ---
                var camForward = Camera.main.transform.forward;
                camForward.y = 0f;
                camForward.Normalize();
                
                var camRotation = Quaternion.LookRotation(camForward);
                var finalMoveDirection = camRotation * moveDirection;
                var targetRotation = Quaternion.LookRotation(finalMoveDirection);
                
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    Time.deltaTime * 10f); 
            }
            
            // --- IDLE CHECK ---
            else 
            {
                _isSprinting = false; 
                if (CurrentState != PlayerState.IsIdle)
                {
                    SetPlayerState(PlayerState.IsIdle);
                }
            }
        }
    }
}