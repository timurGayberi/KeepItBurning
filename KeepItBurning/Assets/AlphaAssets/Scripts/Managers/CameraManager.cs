using UnityEngine;
using General;
using Unity.Cinemachine; 

using PlayerScripts; 
using System;

namespace Managers
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraLockController : MonoBehaviour
    {
        private InputReader _inputReader;
        private PlayerMovement _playerMovement;
        private CinemachineCamera _vCam;
        
        // **CORRECT COMPONENT:** We are using the Orbital Follow body
        private CinemachineOrbitalFollow _orbitalFollow;
        
        private Vector2 _currentMoveInput; 
        private CinemachineCamera VCam
        {
            get
            {
                if (_vCam == null)
                {
                    _vCam = GetComponent<CinemachineCamera>();
                }
                return _vCam;
            }
        }

        [Header("FOV Settings")]
        [SerializeField]
        private float interactionFOV = 25f;

        [Header("Zoom speed")]
        [SerializeField]
        private float zoomSpeed = 5f;
        
        [Header("Directional Camera Offset")] 
        [SerializeField]
        // This is the max shift on the horizontal plane (X or Z axis)
        private float maxLookAheadDistance = 0.5f; 
        [SerializeField]
        private float offsetAdjustSpeed = 5f; 

        private float _defaultFOV, _targetFOV;
        
        // Store the initial offset (Target Offset) to which we will add the peek
        private Vector3 _defaultOffset; 
        // Stores the final calculated offset
        private Vector3 _targetOffset; 

        private void Awake()
        {
            if (VCam != null)
            {
                _defaultFOV = VCam.Lens.FieldOfView;
                _targetFOV = _defaultFOV;
                
                // Get the Orbital Follow component
                //_orbitalFollow = VCam.GetCinemachineComponent<CinemachineOrbitalFollow>();

                if (_orbitalFollow != null)
                {
                    // Store the initial Target Offset
                   // _defaultOffset = _orbitalFollow.m_FollowOffset;
                    _targetOffset = _defaultOffset;
                }
                else
                {
                    Debug.LogError("CinemachineCamera does not have a CinemachineOrbitalFollow component. Directional movement will not work.");
                }
            }
        }

        private void OnEnable()
        {
            _playerMovement = FindObjectOfType<PlayerMovement>();
            _inputReader = FindObjectOfType<InputReader>();

            if (_playerMovement == null || _inputReader == null)
            {
                Debug.LogError("Dependencies missing.");
                return;
            }
            
            _playerMovement.OnPlayerStateChange += HandlePlayerStateChange;
            _inputReader.OnMoveEvent += HandleMoveInput;
            
            HandlePlayerStateChange(_playerMovement.CurrentState);
        }

        private void OnDisable()
        {
            if (_playerMovement != null)
            {
                _playerMovement.OnPlayerStateChange -= HandlePlayerStateChange;
            }
            if (_inputReader != null)
            {
                _inputReader.OnMoveEvent -= HandleMoveInput;
            }
        }
        
        private void HandleMoveInput(Vector2 moveInput)
        {
            _currentMoveInput = moveInput;
        }

        private void HandlePlayerStateChange(PlayerState newState)
        {
            if (newState == PlayerState.IsInteracting)
            {
                _targetFOV = interactionFOV;
            }
            else
            {
                _targetFOV = _defaultFOV;
            }
        }

        private void FixedUpdate()
        {
            if (VCam == null || _orbitalFollow == null) return;
            
            // 1. FOV Interpolation
            VCam.Lens.FieldOfView = Mathf.Lerp
            (
                VCam.Lens.FieldOfView,
                _targetFOV,
                Time.fixedDeltaTime * zoomSpeed
            );
            
            // 2. Directional Offset (Peek/Push) Logic
            Vector2 moveInput = _currentMoveInput; 
            
            // Calculate the desired offset shift (X and Z axes only)
            
            // X-axis offset (Left/Right peek)
            // If the player moves right (positive X input), shift the camera left (negative X offset) to peek ahead.
            float targetOffsetX = moveInput.x * -maxLookAheadDistance;
            
            // Z-axis offset (Forward/Backward peek)
            // If the player moves forward (positive Y input), shift the camera forward (positive Z offset) to follow closer.
            // Note: Orbital Follow Z-offset usually means distance. Adjusting this might feel like the camera moves *through* the player.
            // We use a positive sign here to add to the Z-distance, pushing the camera slightly back/forward.
            float targetOffsetZ = moveInput.y * maxLookAheadDistance;
            
            // Construct the final offset change vector (Y is 0 for input-based adjustment)
            Vector3 offsetChange = new Vector3(targetOffsetX, 0, targetOffsetZ);
            
            if (_targetFOV == _defaultFOV)
            {
                // Set the target offset relative to the default offset
                _targetOffset = _defaultOffset + offsetChange;
            }
            else
            {
                // Reset the offset back to default when interacting/zooming
                _targetOffset = _defaultOffset;
            }
            
            // Smoothly apply the target offset to the Orbital Follow component
            //_orbitalFollow.m_FollowOffset = Vector3.Lerp(
               // _orbitalFollow.m_FollowOffset, 
                //_targetOffset, 
                //Time.fixedDeltaTime * offsetAdjustSpeed
            //);
        }
    }
}