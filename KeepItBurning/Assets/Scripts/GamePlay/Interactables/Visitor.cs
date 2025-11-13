using UnityEngine;
using Interfaces;
using System.Collections;

namespace GamePlay.Interactables
{
    public enum VisitorStatus
    {
        Idle,
        RequestMarshmallow,
        RequestHotChocolate,
        RequestSausage
        
        // more to come ?
        
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
        
        public void Interact(GameObject interactor)
        {
            Debug.Log($"Player ({interactor.name}) is interacting with the {InteractionPrompt}");
        }

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
            var heldItem = "Marshmallow"; 

            if (currentVisitorStatus == VisitorStatus.Idle)
            {
                Debug.Log("Visitor is currently idle and not requesting anything. Nothing happens.");
                return;
            }

            if (IsCorrectItem(heldItem))
            {
                Debug.Log($"SUCCESS! Served '{heldItem}' to satisfy request: {currentVisitorStatus}. (+SCORE, +HAPPINESS)");
                SetIdle();
                // TODO: Add score/happiness logic here
            }
            else
            {
                Debug.Log($"FAILURE. Player attempted to serve '{heldItem}', but visitor requested: {currentVisitorStatus}. (-SCORE, -HAPPINESS)");
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

        private bool IsCorrectItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return false;

            var isCorrect = false;
            
            switch (currentVisitorStatus)
            {
                case VisitorStatus.RequestMarshmallow:
                    isCorrect = itemName.Equals("Marshmallow", System.StringComparison.OrdinalIgnoreCase);
                    break;
                case VisitorStatus.RequestHotChocolate:
                    isCorrect = itemName.Equals("HotChocolate", System.StringComparison.OrdinalIgnoreCase);
                    break;
                case VisitorStatus.RequestSausage:
                    isCorrect = itemName.Equals("Sausage", System.StringComparison.OrdinalIgnoreCase);
                    break;
                
                default:
                    isCorrect = false;
                    break;
            }

            return isCorrect;
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
                case VisitorStatus.RequestMarshmallow:
                    marshmallowIcon.SetActive(true);
                    break;
                case VisitorStatus.RequestHotChocolate:
                    hotChocolateIcon.SetActive(true);
                    break;
                case VisitorStatus.RequestSausage:
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