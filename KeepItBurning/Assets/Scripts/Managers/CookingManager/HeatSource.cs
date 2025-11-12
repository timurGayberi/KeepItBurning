using UnityEngine;
using Interfaces;
using Player;

namespace GamePlay.Interactibles
{
    public class HeatSource : MonoBehaviour, IInteractable
    {
        [Header("References")]
        public CookableItems cookableItem;
        public CollectiblesLogic collectiblesLogic;

        public Transform cookPoint;

        public bool isCooking = false;
        private string interactionPrompt = "Start Cooking";
        private bool hasChocolate;

        public InteractionData GetInteractionData()
        {
            return new InteractionData
            {
                promptText = isCooking ? "Stop Cooking" : interactionPrompt,
                actionDuration = 0f
            };
        }

        public void Interact()
        {
            if (!isCooking)
                StartCooking();
            else
                StopCooking();
        }

        public void StopInteraction()
        {

        }

        void StartCooking()
        {
            if (cookableItem == null) return;

            collectiblesLogic.chocolateVisual.SetActive(hasChocolate);
            cookableItem.transform.position = cookPoint.position;

            cookableItem.StartCooking();
            isCooking = true;
        }

        void StopCooking()
        {
            if (cookableItem == null) return;
            cookableItem.StopCooking();
            isCooking = false;
        }
    }
}