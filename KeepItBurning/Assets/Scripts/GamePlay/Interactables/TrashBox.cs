using Interfaces;
using UnityEngine;

namespace GamePlay.Interactables
{
    public class TrashBox : MonoBehaviour , IInteractable
    {
        public string InteractionPrompt { get; } = "Trash box";

        public void Interact(GameObject interactor)
        {
            Debug.Log($"Player ({interactor.name}) is interacting with the {InteractionPrompt}");
            //Destroy(gameObject);
        }
    }
}