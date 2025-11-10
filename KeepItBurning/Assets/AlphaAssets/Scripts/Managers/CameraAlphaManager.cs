using UnityEngine;
using Unity.Cinemachine;
using PlayerScripts;
using General;

namespace Managers
{
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraAlphaController : MonoBehaviour
    {
        private InputReader _inputReader;
        private PlayerMovement _playerMovement;
        private PlayerInventory _playerInventory;
        private CinemachineCamera _vCam;
        private CinemachineOrbitalFollow _orbitalFollow;

        private Vector2 _currentMoveInput;
        private float _defaultFOV;
        private float _targetFOV;

        private Vector3 _defaultOffset;
        private Vector3 _targetOffset;

        [Header("FOV Settings")]
        [SerializeField] private float interactionFOV = 25f;

        [Header("Zoom Speed")]
        [SerializeField] private float zoomSpeed = 5f;
        
        [Header("Carrying Offset Settings")]
        [Tooltip("The max offset distance to use when the player is in the IsInteracting state.")]
        [SerializeField] private float interactionOffsetDistance = 0.1f;

        [Header("Directional Camera Offset")]
        [SerializeField] private float maxOffsetDistance = 0.5f;
        [SerializeField] private float offsetAdjustSpeed = 5f;

        private CinemachineCamera VCam
        {
            get
            {
                if (_vCam == null)
                    _vCam = GetComponent<CinemachineCamera>();
                return _vCam;
            }
        }

        private void Awake()
        {
            if (VCam == null) return;

            _defaultFOV = VCam.Lens.FieldOfView;
            _targetFOV = _defaultFOV;

            var bodyComponent = VCam.GetCinemachineComponent(CinemachineCore.Stage.Body);
            _orbitalFollow = bodyComponent as CinemachineOrbitalFollow;

            if (_orbitalFollow != null)
            {
                _defaultOffset = _orbitalFollow.TargetOffset;
                _targetOffset = _defaultOffset;
            }
            else
            {
                Debug.LogError("Camera body must be CinemachineOrbitalFollow to control TargetOffset.");
            }
        }

        private void OnEnable()
        {
            _playerMovement = FindObjectOfType<PlayerMovement>();
            _inputReader = FindObjectOfType<InputReader>();
            _playerInventory = FindObjectOfType<PlayerInventory>();

            if (_playerMovement == null || _inputReader == null || _playerInventory == null)
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
                _playerMovement.OnPlayerStateChange -= HandlePlayerStateChange;

            if (_inputReader != null)
                _inputReader.OnMoveEvent -= HandleMoveInput;
        }

        private void HandleMoveInput(Vector2 moveInput)
        {
            _currentMoveInput = moveInput;
        }

        private void HandlePlayerStateChange(PlayerState newState)
        {
            //_targetFOV = (newState == PlayerState.IsInteracting)
            //    ? interactionFOV
            //    : _defaultFOV;
        }

        private void FixedUpdate()
        {
            if (VCam == null || _orbitalFollow == null || _playerInventory == null) return;
            
            float currentMaxOffset;

            if (_playerInventory.IsCarryingBurden) 
            {
                currentMaxOffset = interactionOffsetDistance;
            }
            else
            {
                currentMaxOffset = maxOffsetDistance;
            }
            // ----------------------------------------------------

            // 2. FOV Interpolation
            VCam.Lens.FieldOfView = Mathf.Lerp(
                VCam.Lens.FieldOfView,
                _targetFOV,
                Time.fixedDeltaTime * zoomSpeed
            );
            
            
            
            if (VCam == null || _orbitalFollow == null) return;
            
            VCam.Lens.FieldOfView = Mathf.Lerp(
                VCam.Lens.FieldOfView,
                _targetFOV,
                Time.fixedDeltaTime * zoomSpeed
            );
            
            var targetOffsetX = _currentMoveInput.x * currentMaxOffset;
            var targetOffsetZ = _currentMoveInput.y * currentMaxOffset;

            Vector3 offsetChange = new Vector3(targetOffsetX, 0, targetOffsetZ);

            if (_playerMovement.CurrentState == PlayerState.IsInteracting)
            {
                _targetOffset = _defaultOffset;
            }
            else
            {
                _targetOffset = _defaultOffset + offsetChange;
            }
            
            
            _orbitalFollow.TargetOffset = Vector3.Lerp(
                _orbitalFollow.TargetOffset,
                _targetOffset,
                Time.fixedDeltaTime * offsetAdjustSpeed
            );
        }
    }
}
