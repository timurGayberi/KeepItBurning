using UnityEngine;
using Interfaces;

namespace General
{

    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [Header("Interaction Settings")]
        [Tooltip("The text to display when the player looks at this object.")]
        [SerializeField] protected string interactionPrompt = "Interact";

        [Tooltip("How long the interaction takes (0 for instant).")]
        [SerializeField] protected float interactionDuration = 0f;

        public virtual InteractionData GetInteractionData()
        {
            return new InteractionData
            {
                promptText = interactionPrompt,
                actionDuration = interactionDuration
            };
        }

        public abstract void Interact();

        public virtual void StopInteraction()
        {
            // Default: Do nothing
        }

        public virtual void OnFocus()
        {
            var outline = GetComponent<OutlineColorLerp>();
            if (outline != null) outline.enabled = true;
        }

        public virtual void OnLoseFocus()
        {
            var outline = GetComponent<OutlineColorLerp>();
            if (outline != null) outline.enabled = false;
        }
    }
}
