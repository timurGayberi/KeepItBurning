using Interfaces;
using UnityEngine;
using General;
using Player;

namespace GamePlay.Interactables
{
    /// <summary>
    /// WC (Toilet) interaction - disposes of held food items.
    /// </summary>
    public class WC : MonoBehaviour, IInteractable
    {
        private const string INTERACTION_PROMPT = "Use WC";
        private const string NO_FOOD_PROMPT = "Nothing to dispose";

        public InteractionData GetInteractionData()
        {
            PlayerInventory inventory = ServiceLocator.GetService<PlayerInventory>();

            if (inventory != null && inventory.IsHoldingFoodItem())
            {
                return new InteractionData
                {
                    promptText = INTERACTION_PROMPT,
                    actionDuration = 0f
                };
            }

            return new InteractionData
            {
                promptText = NO_FOOD_PROMPT,
                actionDuration = -1f // Can't interact if not holding food
            };
        }

        public void Interact()
        {
            PlayerInventory inventory = ServiceLocator.GetService<PlayerInventory>();

            if (inventory == null)
            {
                Debug.LogError("[WC] Could not find PlayerInventory service!");
                return;
            }

            if (inventory.IsHoldingFoodItem())
            {
                string foodName = inventory.GetCurrentHeldFoodItemName();
                inventory.ClearHeldFoodItem();

                // Play disposal sound (using drop item sound for now)
                SoundManager.Play(SoundAction.DropItem);
            }
        }

        public void StopInteraction()
        {
            // Not applicable for instant action
        }
    }
}
