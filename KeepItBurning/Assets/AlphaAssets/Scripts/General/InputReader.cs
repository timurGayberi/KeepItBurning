using UnityEngine;

using UnityEngine.InputSystem;

using System;



namespace General

{

    public enum ControlDevice

    {

        KeyboardMouse,

        Gamepad,

        Unknown 

    }

    

    public class InputReader : MonoBehaviour, IInputService, InputSystem_Actions.IPlayerActions, InputSystem_Actions.IUIActions

    {

        private static InputReader Instance;

        

        public event Action<ControlDevice> OnControlSchemeChange;

        public event Action<ControlDevice> OnControlStateChange;

        public ControlDevice CurrentControlDevice { get; private set; } = ControlDevice.Unknown;

        public event Action<Vector2> OnMoveEvent;

        public event Action OnSprintStarted;

        public event Action OnSprintCanceled;

        public event Action<Vector2> OnSprintEvent;

        

        private bool _isUIInputActive = false;



        public event Action OnInteractEvent;

        

        // NEW: Pause event (matches IInputService)

        public event Action OnPauseEvent;

        // -------------------------------------

        

        private InputSystem_Actions _inputsInstance;



        private void Awake()

        {

            

            if (Instance == null)

            {

                // This is the correct, first instance. Set it and protect it.

                Instance = this;

            }

            else

            {

                // This is a duplicate created by a scene reload. Destroy it immediately.

                Debug.LogWarning("InputReader: Found duplicate instance on scene load. Destroying clone.");

                Destroy(gameObject);

                return; // Stop execution for this duplicate instance

            }

            

            _inputsInstance = new InputSystem_Actions();

            _inputsInstance.Player.SetCallbacks(this);

            // NEW: Set callbacks for the UI action map

            _inputsInstance.UI.SetCallbacks(this); 

            

            try

            {

                ServiceLocator.RegisterService<IInputService>((IInputService)this);

            }

            catch (Exception e)

            {

                Debug.LogError("Failed to register IInputService: " + e.Message);

                Destroy(gameObject);

                return;

            }



            DontDestroyOnLoad(gameObject);

        }



        private void Start()

        {

            if (Gamepad.all.Count > 0)

            {

                CurrentControlDevice = ControlDevice.Gamepad;

            }

            else

            {

                CurrentControlDevice = ControlDevice.KeyboardMouse;

            }

            Debug.Log($"Initial Device State: {CurrentControlDevice}");

        }

        private void OnEnable()

        {

            _inputsInstance.Player.Enable();
            

             _inputsInstance.UI.Enable(); 

            InputSystem.onDeviceChange += OnInputDeviceChange;

        }

        private void OnDisable()

        {

            if (_inputsInstance == null) return;
            // --------------------------------------------------------
            
            _inputsInstance.Player.Disable();
            _inputsInstance.UI.Disable();
            InputSystem.onDeviceChange -= OnInputDeviceChange;

        }

        private void OnInputDeviceChange(InputDevice device, InputDeviceChange change)

        {

            if (change == InputDeviceChange.Added || change == InputDeviceChange.Disconnected)

            {

                Debug.Log($"System device change: {device.displayName} was {change}. Next input will determine scheme.");

            }

        }

        

        #region Movement

        public void OnMove(InputAction.CallbackContext context)

        {
            if (context.started) 

            {

               // CheckAndReportDevice(context);

            }
            
            OnMoveEvent?.Invoke(context.ReadValue<Vector2>());

        }
        
        public void OnSprint(InputAction.CallbackContext context)

        {

            if (context.started)

            {
                //CheckAndReportDevice(context);

                OnSprintStarted?.Invoke();
            }
            

            if (context.canceled)
            {
                OnSprintCanceled?.Invoke();
            }

        }
        
        #endregion

        

        #region General Interaction

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started) 

            {
                OnInteractEvent?.Invoke();

                //CheckAndReportDevice(context);
            }
        }

        #endregion

        

        #region UI Actions

        public void OnPauseGame(InputAction.CallbackContext context)

        {

            if (context.started)

            {
                Debug.Log($"[INPUT] Pause input received from {context.control.device.displayName}.");
                OnPauseEvent?.Invoke();
            }
            
            //OnPauseEvent?.Invoke();
            //CheckAndReportDevice(context);

        }
        
        public void EnableUIInput(bool enabled)

        {

            _isUIInputActive = enabled;



            if (enabled)

                _inputsInstance.UI.Enable();

            else

                _inputsInstance.UI.Disable();



            Debug.Log($"[Input] UI Input Set to: {enabled}");

        }
        

        public void DisablePlayerInput()
        {
            _inputsInstance.Player.Disable();
            _inputsInstance.UI.Enable();
            Debug.Log("[Input] Player Input Disabled, UI Enabled.");
        }

        public void EnablePlayerInput()
        {
            _inputsInstance.Player.Enable();
            _inputsInstance.UI.Disable();
            Debug.Log("[Input] Player Input Enabled, UI Disabled.");
        }
        

        #endregion

        

        private void CheckAndReportDevice(InputAction.CallbackContext context)
        {
            if (context.control == null)
            {
                Debug.LogWarning("[INPUT WARNING] Input action context triggered but context.control is null. Skipping device check.");
                return;
            }
    
            ControlDevice detectedDevice = GetDeviceFromControl(context.control);

            if (CurrentControlDevice != detectedDevice)
            {
                SetNewControlDevice(detectedDevice);
            }
        }

        private void SetNewControlDevice(ControlDevice newDevice)

        {

            CurrentControlDevice = newDevice;
            

            OnControlStateChange?.Invoke(CurrentControlDevice);

            Debug.Log($"Control scheme switched to: {CurrentControlDevice}");

        }

        private ControlDevice GetDeviceFromControl(InputControl control)

        {
            

            if (control.device is Gamepad)

            {

                return ControlDevice.Gamepad;

            }
            
            
            if (control.device is Keyboard || control.device is Mouse)
            {

                return ControlDevice.KeyboardMouse;

            }

            return ControlDevice.Unknown;

        }

        

        #region ImplementInFuture?

        public void OnLook(InputAction.CallbackContext context) { }

        public void OnJump(InputAction.CallbackContext context) { }

        public void OnAttack(InputAction.CallbackContext context) { }

        public void OnPrevious(InputAction.CallbackContext context) { }

        public void OnNext(InputAction.CallbackContext context) { }

        

        // UI Action Callbacks (Must exist to implement IUIActions)

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