using Interfaces;
using UnityEngine;

namespace GamePlay.Interactables
{
    public class TrashBox : MonoBehaviour , IInteractable
    {
        private const string INTERACTION_PROMPT = "Use Trash Box";
        
        public InteractionData GetInteractionData()
        {
            return new InteractionData
            {
                promptText = INTERACTION_PROMPT,
                actionDuration = 0f // <-- Key change: 0 duration means instant execution
            };
        }

        public void Interact()
        {
            Debug.Log($"Trashing item in the {INTERACTION_PROMPT}.");
        }

        public void StopInteraction()
        {
            //Debug.Log("[TRASH BOX] Interaction stopped (not applicable for instant action).");
        }
    }
}