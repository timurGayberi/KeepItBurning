using UnityEngine;

namespace Interfaces
{
    public struct InteractionData
    {
        public string promptText;
        public float actionDuration;
    }

    public interface IInteractable
    {
        InteractionData GetInteractionData();
        
        void Interact(); 
        
        void StopInteraction();
    }
}