using UnityEngine;
using System;

namespace Interfaces
{
    public enum ControlDevice
    {
        Unknown,
        
        KeyboardMouse,
        Gamepad
    }
    
    public interface IInputService 
    {
        // Player Input Events
        event Action<Vector2> OnMoveEvent;
        event Action OnSprintStarted;
        event Action OnSprintCanceled;
        event Action OnInteractEvent;
        // more to come ?
        
        
        // UI/Menu Input Events
        event Action OnPauseEvent;
        
        // Control Scheme Management
        event Action<ControlDevice> OnControlSchemeChange;
        ControlDevice currentControlDevice { get; }
        
        void DisablePlayerInput(); 
        void EnablePlayerInput();
        void EnableUIInput();
        void DisableUIInput();
    }
}