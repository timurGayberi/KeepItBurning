using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Managers.GeneralManagers; 
using Interfaces;

namespace General
{
    public class InputReader : MonoBehaviour, IInputService, InputSystem_Actions.IPlayerActions, InputSystem_Actions.IUIActions
    {
        // IInputService Events
        public event Action<Vector2> OnMoveEvent;
        public event Action OnSprintStarted;
        public event Action OnSprintCanceled;
        public event Action OnInteractEvent;
        public event Action OnPauseEvent;
        
        public event Action<ControlDevice> OnControlSchemeChange;
        
        public ControlDevice CurrentControlDevice { get; private set; } = ControlDevice.Unknown;

        private InputSystem_Actions _inputsInstance;
        

        private void Awake()
        {
            _inputsInstance = new InputSystem_Actions();
            
            _inputsInstance.Player.SetCallbacks(this);
            _inputsInstance.UI.SetCallbacks(this); 
            
            try
            {
                ServiceLocator.RegisterService<IInputService>(this);
            }
            catch (Exception e)
            {
                Debug.LogError("InputReader: Failed to register IInputService: " + e.Message);
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            CurrentControlDevice = Gamepad.all.Count > 0 ? ControlDevice.Gamepad : ControlDevice.KeyboardMouse;
            
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
            // This is useful for future features, like dynamic UI prompts based on device
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());
            CheckAndReportDevice(context); 
        }
        
        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started) OnSprintStarted?.Invoke();
            if (context.canceled) OnSprintCanceled?.Invoke();
            
            CheckAndReportDevice(context); 
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started) OnInteractEvent?.Invoke();
            
            CheckAndReportDevice(context); 
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnLook(InputAction.CallbackContext context) { }
        public void OnJump(InputAction.CallbackContext context) { }
        public void OnAttack(InputAction.CallbackContext context) { }
        
        // --- IUIActions Implementation ---

        public void OnPauseGame(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                //Debug.Log($"[INPUT] Pause input received from {context.control.device.displayName}.");
                OnPauseEvent?.Invoke();
                CheckAndReportDevice(context); 
            }
        }
        
        // Dummy/Unused UI Callbacks (Required for interface completion)
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
        public void OnOnPauseGame(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        // --- IInputService Implementation (Control Flow) ---

        public ControlDevice currentControlDevice { get; }

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
        
        // --- Device Tracking Logic ---
        
        private void CheckAndReportDevice(InputAction.CallbackContext context)
        {
            if (context.control == null) return;
    
            var detectedDevice = GetDeviceFromControl(context.control);

            // Only update and invoke the event if the control device has actually changed
            if (CurrentControlDevice != detectedDevice)
            {
                SetNewControlDevice(detectedDevice);
            }
        }

        private void SetNewControlDevice(ControlDevice newDevice)
        {
            CurrentControlDevice = newDevice;
            OnControlSchemeChange?.Invoke(CurrentControlDevice);
            //Debug.Log($"Control scheme switched to: {CurrentControlDevice}");
        }

        private ControlDevice GetDeviceFromControl(InputControl control)
        {
            if (control.device is Gamepad) return ControlDevice.Gamepad;
            // Check for Keyboard OR Mouse input
            if (control.device is Keyboard || control.device is Mouse) return ControlDevice.KeyboardMouse;
            return ControlDevice.Unknown;
        }
    }
}