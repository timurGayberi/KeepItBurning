using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace General 
{
    public class InputSystemActions : IDisposable
    {

        public interface IPlayerActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context); 
            void OnAttack(InputAction.CallbackContext context); 
            void OnInteract(InputAction.CallbackContext context);
        }

        public interface IUIActions
        {
            void OnPauseGame(InputAction.CallbackContext context);
            void OnPrevious(InputAction.CallbackContext context);
            void OnNext(InputAction.CallbackContext context);
            void OnNavigate(InputAction.CallbackContext context);
            void OnSubmit(InputAction.CallbackContext context); 
            void OnCancel(InputAction.CallbackContext context);   
            void OnPoint(InputAction.CallbackContext context);    
            void OnClick(InputAction.CallbackContext context); 
            void OnScrollWheel(InputAction.CallbackContext context); 
            void OnMiddleClick(InputAction.CallbackContext context); 
            void OnRightClick(InputAction.CallbackContext context);  
            void OnTrackedDevicePosition(InputAction.CallbackContext context); 
            void OnTrackedDeviceOrientation(InputAction.CallbackContext context); 
        }
        
        public class PlayerActionMap { public void SetCallbacks(IPlayerActions instance) {} public void Enable() {} public void Disable() {} }
        public class UIActionMap { public void SetCallbacks(IUIActions instance) {} public void Enable() {} public void Disable() {} }
        
        public PlayerActionMap Player = new PlayerActionMap();
        public UIActionMap UI = new UIActionMap();

        public void Dispose() { } 
    }
}