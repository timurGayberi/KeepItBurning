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

        public InteractionData GetInteractionData()
        {
            return new InteractionData
            {
                promptText = isCooking ? "Stop Cooking" : "Start Cooking",
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

        public void StopInteraction() { }

        void StartCooking()
        {
            if (cookableItem == null || collectiblesLogic == null) return;

            if (collectiblesLogic.HasChocolate)
            {
                cookableItem.collectiblesLogic = collectiblesLogic;
                cookableItem.StartCooking();
                collectiblesLogic.chocolateVisual.SetActive(true);
                isCooking = true;
                Debug.Log("Started cooking CHOCOLATE.");
            }
            else if (collectiblesLogic.HasMarshmallow)
            {
                Debug.Log("Started cooking MARSHMALLOW (todo)");
            }
            else if (collectiblesLogic.HasSausage)
            {
                Debug.Log("Started cooking SAUSAGE (todo)");
            }
            else
            {
                Debug.Log("No cookable item in hand.");
            }
        }

        void StopCooking()
        {
            if (cookableItem == null) return;

            cookableItem.StopCooking();
            isCooking = false;

            collectiblesLogic.chocolateVisual.SetActive(false);
            collectiblesLogic.hotChocolateVisual.SetActive(false);
            collectiblesLogic.burnedHotChocolateVisual.SetActive(false);
        }
    }
}