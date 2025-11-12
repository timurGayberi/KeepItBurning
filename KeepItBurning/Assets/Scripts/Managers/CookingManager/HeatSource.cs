using UnityEngine;
using Interfaces;
using System.Collections;
using Player;

namespace GamePlay.Interactibles
{
    public class HeatSource : MonoBehaviour, IInteractable
    {
        public CookableItems cookableItem;
        public CollectiblesLogic collectiblesLogic;

        public Transform cookPoint;

        public bool isCooking = false;

        private string interactionPrompt = "Start Cooking";

        private bool hasChocolate = false;

        void Update()
        {

        }

        public InteractionData GetInteractionData()
        {
            string prompt = interactionPrompt;
            float duration = 0f;

            return new InteractionData
            { 
                promptText = prompt,
                actionDuration = duration
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