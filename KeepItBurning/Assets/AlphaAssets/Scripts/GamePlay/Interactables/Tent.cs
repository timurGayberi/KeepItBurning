using UnityEngine;
using Interfaces;
using UnityEngine.Serialization;

namespace GamePlay.Interactables
{
    public enum TentState
    {
        Default, // All visuals disabled
        Normal,
        Damaged,
        Burned
    }
    
    public class Tent : MonoBehaviour , IInteractable
    {
        
        [Header("Visual States")]
        [Tooltip("The GameObject representing the healthy tent.")]
        [SerializeField] 
        private GameObject tentsNormalState;
        
        [Tooltip("The GameObject representing the damaged or destroyed tent.")]
        [SerializeField] 
        private GameObject tentsDamagedState;
        
        [Tooltip("The GameObject representing the burned tent.")]
        [SerializeField] 
        private GameObject tentsBurnedState;

        [FormerlySerializedAs("_currentTentState")]
        [Header("Current State")]
        [SerializeField] 
        private TentState currentTentState = TentState.Normal;
        
        public string InteractionPrompt { get; } = "Tent";
        
        public void SetState(TentState newState)
        {
            UpdateVisuals(newState);
        }
        
        private void Awake()
        {
            UpdateVisuals(currentTentState);
            if (currentTentState != TentState.Normal)
            {
                currentTentState = TentState.Normal;
            }
        }
        
        private void UpdateVisuals(TentState newState)
        {
            tentsNormalState.SetActive(false);
            tentsDamagedState.SetActive(false);
            tentsBurnedState.SetActive(false);
            
            switch (newState)
            {
                case TentState.Normal:
                    tentsNormalState.SetActive(true);
                    break;
                case TentState.Damaged:
                    tentsDamagedState.SetActive(true);
                    break;
                case TentState.Burned:
                    tentsBurnedState.SetActive(true);
                    break;
                case TentState.Default:
                    // Default state means all are off. No action needed here.
                    break;
            }
            
            currentTentState = newState;
        }
        
        public void Interact(GameObject interactor, PlayerScripts.PlayerMovement playerMovement)
        {
            Debug.Log($"Player ({interactor.name}) is interacting with the {InteractionPrompt}. Current state: {currentTentState}");
            
            // if (currentTentState == TentState.Normal)
            // {
            //     SetState(TentState.Damaged); // Now uses the new public method
            // }
        }
        private void OnValidate()
        {
            if (tentsNormalState && tentsDamagedState && tentsBurnedState)
            {
                UpdateVisuals(currentTentState);
            }
        }
    }
}
