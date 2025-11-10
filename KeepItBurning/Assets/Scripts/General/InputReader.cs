using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Managers; 
using Interfaces;

namespace General
{
    public class InputReader : MonoBehaviour, IInputService, InputSystemActions.IPlayerActions, InputSystemActions.IUIActions
    {
        // IInputService Events
        public event Action<Vector2> OnMoveEvent;
        public event Action OnInteractEvent;
        public event Action OnPauseEvent;
        
        public event Action<ControlDevice> OnControlSchemeChange;
        public ControlDevice currentControlDevice { get; private set; } = ControlDevice.Unknown;

        private InputSystemActions _inputsInstance;
        private IDisposable _playerCallbackHandle;
        private IDisposable _uiCallbackHandle;

        private void Awake()
        {
            
            _inputsInstance = new InputSystemActions();
            
            _inputsInstance.Player.SetCallbacks(this);
            _inputsInstance.UI.SetCallbacks(this); 
            
            try
            {
                ServiceLocator.RegisterService<IInputService>(this);
            }
            catch (Exception _)
            {
                //Debug.LogError("InputReader: Failed to register IInputService: " + e.Message);
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            currentControlDevice = Gamepad.all.Count > 0 ? ControlDevice.Gamepad : ControlDevice.KeyboardMouse;
            //Debug.Log($"InputReader Initialized. Device: {CurrentControlDevice}");
            
            if (GameStateManager.instance != null)
            {
                GameStateManager.OnGameStateChanged += OnGameStateChanged;
            }
        }

        private void OnEnable()
        {
            InputSystem.onDeviceChange += OnInputDeviceChange;
            EnablePlayerInput(); 
            DisableUIInput();
        }

        private void OnDisable()
        {
            if (_inputsInstance == null) return;
            InputSystem.onDeviceChange -= OnInputDeviceChange;
            
            if (GameStateManager.instance != null)
            {
                GameStateManager.OnGameStateChanged -= OnGameStateChanged;
            }

            _inputsInstance.Player.Disable();
            _inputsInstance.UI.Disable();
            _inputsInstance.Dispose(); 
        }
        
        private void OnGameStateChanged(GameStateManager.GameState newState)
        {
            switch (newState)
            {
                case GameStateManager.GameState.GamePlay:
                    EnablePlayerInput();
                    DisableUIInput();
                    break;
                case GameStateManager.GameState.Paused:
                    DisablePlayerInput();
                    EnableUIInput();
                    break;
                case GameStateManager.GameState.MainMenu:
                case GameStateManager.GameState.Default:
                    DisablePlayerInput();
                    EnableUIInput();
                    break;
            }
        }

        private void OnInputDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Disconnected)
            {
                //Debug.Log($"System device change: {device.displayName} was {change}.");
            }
        }
        
        public void OnMove(InputAction.CallbackContext _ )
        {
            OnMoveEvent?.Invoke(_.ReadValue<Vector2>());
        }
        

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started) OnInteractEvent?.Invoke();
        }
        

        public void OnPauseGame(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                //Debug.Log($"[INPUT] Pause input received from {context.control.device.displayName}.");
                OnPauseEvent?.Invoke();
                CheckAndReportDevice(context); 
            }
        }
        

        public void DisablePlayerInput()
        {
            _inputsInstance.Player.Disable();
            //Debug.Log("[Input] Player Input Disabled.");
        }

        public void EnablePlayerInput()
        {
            _inputsInstance.Player.Enable();
            //Debug.Log("[Input] Player Input Enabled.");
        }

        public void EnableUIInput()
        {
            _inputsInstance.UI.Enable();
            //Debug.Log("[Input] UI Input Enabled.");
        }
        
        public void DisableUIInput()
        {
             _inputsInstance.UI.Disable();
            //Debug.Log("[Input] UI Input Disabled.");
        }
        
        
        private void CheckAndReportDevice(InputAction.CallbackContext context)
        {
            if (context.control == null) return;
    
            var detectedDevice = GetDeviceFromControl(context.control);

            if (currentControlDevice != detectedDevice)
            {
                SetNewControlDevice(detectedDevice);
            }
        }

        private void SetNewControlDevice(ControlDevice newDevice)
        {
            currentControlDevice = newDevice;
            OnControlSchemeChange?.Invoke(currentControlDevice);
            //Debug.Log($"Control scheme switched to: {CurrentControlDevice}");
        }

        private ControlDevice GetDeviceFromControl(InputControl control)
        {
            if (control.device is Gamepad) return ControlDevice.Gamepad;
            if (control.device is Keyboard || control.device is Mouse) return ControlDevice.KeyboardMouse;
            return ControlDevice.Unknown;
        }

        #region Callbacks
        
        // Dummy/Unused Callbacks (Required for interface completion)
        public void OnLook(InputAction.CallbackContext context) { }
        public void OnJump(InputAction.CallbackContext context) { }
        public void OnAttack(InputAction.CallbackContext context) { }
        public void OnPrevious(InputAction.CallbackContext context) { }
        public void OnNext(InputAction.CallbackContext context) { }
        public void OnNavigate(InputAction.CallbackContext context) { }
        public void OnSubmit(InputAction.CallbackContext context) { }
        public void OnCancel(InputAction.CallbackContext context) { }
        public void OnPoint(InputAction.CallbackContext context) { }
        public void OnClick(InputAction.CallbackContext context) { }
        public void OnScrollWheel(InputAction.CallbackContext context) { }
        public void OnMiddleClick(InputAction.CallbackContext context) { }
        public void OnRightClick(InputAction.CallbackContext context) { }
        public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
        
        #endregion
    }
}