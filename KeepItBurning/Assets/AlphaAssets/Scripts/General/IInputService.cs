// In Assets/Scripts/General/IInputService.cs

using UnityEngine;
using System;

namespace General
{
    public interface IInputService 
    {
        event Action<Vector2> OnMoveEvent;
        
        public event Action OnSprintStarted;
        public event Action OnSprintCanceled;
        public event Action OnInteractEvent;
        
        event Action<ControlDevice> OnControlSchemeChange;
        ControlDevice CurrentControlDevice { get; }
        
        event Action OnPauseEvent;
        
        void EnablePlayerInput(); 
        void DisablePlayerInput(); 
        void EnableUIInput(bool enabled); 
        
    }
}