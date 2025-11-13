using UnityEngine;
using Interfaces;
using System.Collections;
using General;
using Player;

namespace GamePlay.Interactables
{
    public enum VisitorStatus
    {
        Idle,
        RequestMarshmallowCooked,
        RequestHotChocolateCooked,
        RequestSausageCooked

        // Visitors only want cooked food!

    }

    public class Visitor : MonoBehaviour , IInteractable
    {
        [SerializeField] public GameObject requestCanva;
        [SerializeField] public GameObject alertIcon;
        [SerializeField] public GameObject marshmallowIcon;
        [SerializeField] public GameObject hotChocolateIcon;
        [SerializeField] public GameObject sausageIcon;

        [SerializeField] private VisitorStatus currentVisitorStatus;
        [SerializeField] private float alertDuration = 2f;

        private Coroutine popupRoutine;
        private Coroutine idleRoutine;

        private const string BASE_PROMPT = "Interact with visitor";
        
        public string InteractionPrompt => BASE_PROMPT;
        
        private void Start()
        {
            SetIdle();
        }
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
                SetIdle();
            if (Camera.main != null && requestCanva != null && requestCanva.activeSelf)
            {
                requestCanva.transform.LookAt(Camera.main.transform);
                requestCanva.transform.rotation = Quaternion.LookRotation(requestCanva.transform.position - Camera.main.transform.position);
            }
        }


        public InteractionData GetInteractionData()
        {
            var prompt = BASE_PROMPT;

            if (currentVisitorStatus != VisitorStatus.Idle)
            {
                var request = currentVisitorStatus.ToString().Replace("Request", "");
                prompt = $"Serve {request} to visitor";
            }

            return new InteractionData
            {
                actionDuration = 0f,
                promptText = prompt
            };
        }

        public void Interact()
        {
            if (currentVisitorStatus == VisitorStatus.Idle)
            {
                Debug.Log("Visitor is currently idle and not requesting anything. Nothing happens.");
                return;
            }

            // Get what the player is currently holding
            PlayerInventory playerInventory = ServiceLocator.GetService<PlayerInventory>();
            if (playerInventory == null)
            {
                Debug.LogError("Could not find PlayerInventory service!");
                return;
            }

            int heldItemID = playerInventory.GetCurrentHeldFoodItemID();
            string heldItemName = playerInventory.GetCurrentHeldFoodItemName();
            CollectibleBase.CookState cookState = playerInventory.GetCurrentFoodCookState();

            if (heldItemID == CollectibleIDs.DEFAULT_ITEM)
            {
                Debug.Log("Player is not holding any food to serve!");
                return;
            }

            if (IsCorrectItem(heldItemID, cookState))
            {
                Debug.Log($"SUCCESS! Served COOKED '{heldItemName}' to satisfy request: {currentVisitorStatus}. (+SCORE, +HAPPINESS)");
                playerInventory.ClearHeldFoodItem(); // Remove the item from player's inventory
                SetIdle();
                // TODO: Add score/happiness logic here
            }
            else
            {
                string reason = "";
                if (cookState != CollectibleBase.CookState.Cooked)
                {
                    reason = $"Food is {cookState}, but visitor wants COOKED food!";
                }
                else
                {
                    reason = "Wrong food type!";
                }

                Debug.Log($"FAILURE. {reason} Visitor requested: {currentVisitorStatus}. (-SCORE, -HAPPINESS)");
                playerInventory.ClearHeldFoodItem(); // Remove the item even if wrong
                SetIdle();
                // TODO: Add less score/remove happiness logic here
            }
        }

        public void StopInteraction()
        {
            //TODO: Stop interaction logic 
        }

        private void ChooseRandomRequest()
        {
            int random = Random.Range(0, 3); 
            VisitorStatus newStatus = (VisitorStatus)(random + 1);
            SetVisitorVisuals(newStatus);
        }

        public void SetIdle()
        {
            SetVisitorVisuals(VisitorStatus.Idle);
            if (idleRoutine != null)
                StopCoroutine(idleRoutine);

            idleRoutine = StartCoroutine(RandomRequestAfterIdle());
        }

        private IEnumerator RandomRequestAfterIdle()
        {
            float waitTime = Random.Range(5f, 10f);
            yield return new WaitForSeconds(waitTime);

            if (currentVisitorStatus == VisitorStatus.Idle)
            {
                ChooseRandomRequest();
            }
        }

        private bool IsCorrectItem(int itemID, CollectibleBase.CookState cookState)
        {
            // Visitors only accept COOKED food!
            if (cookState != CollectibleBase.CookState.Cooked)
            {
                return false;
            }

            switch (currentVisitorStatus)
            {
                case VisitorStatus.RequestMarshmallowCooked:
                    return itemID == CollectibleIDs.MARSHMALLOW;
                case VisitorStatus.RequestHotChocolateCooked:
                    return itemID == CollectibleIDs.HOT_CHOCOLATE;
                case VisitorStatus.RequestSausageCooked:
                    return itemID == CollectibleIDs.SAUSAGE;
                default:
                    return false;
            }
        }
        

        private void SetVisitorVisuals(VisitorStatus newStatus)
        {
            currentVisitorStatus = newStatus;

            if (requestCanva == null)
                return;

            if (popupRoutine != null)
                StopCoroutine(popupRoutine);

            if (newStatus == VisitorStatus.Idle)
            {
                SetIconsToFalse();
                requestCanva.SetActive(false);
                return;
            }

            popupRoutine = StartCoroutine(ShowAlertThenIcon(newStatus));
        }

        private IEnumerator ShowAlertThenIcon(VisitorStatus status)
        {
            requestCanva.SetActive(true);
            alertIcon.SetActive(true);
            SetIconsToFalse(alertOnly: true);

            yield return new WaitForSeconds(alertDuration);
            
            alertIcon.SetActive(false);

            switch (status)
            {
                case VisitorStatus.RequestMarshmallowCooked:
                    marshmallowIcon.SetActive(true);
                    break;
                case VisitorStatus.RequestHotChocolateCooked:
                    hotChocolateIcon.SetActive(true);
                    break;
                case VisitorStatus.RequestSausageCooked:
                    sausageIcon.SetActive(true);
                    break;
            }
        }
        
        private void SetIconsToFalse(bool alertOnly = false)
        {
            if (!alertOnly)
            {
                alertIcon.SetActive(false);
            }
            marshmallowIcon.SetActive(false);
            hotChocolateIcon.SetActive(false);
            sausageIcon.SetActive(false);
        }
    }
}