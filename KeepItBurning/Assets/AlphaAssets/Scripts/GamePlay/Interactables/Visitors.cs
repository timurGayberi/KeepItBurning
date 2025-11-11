using UnityEngine;
using UnityEngine.UI;
using Interfaces;
using PlayerScripts;
using System.Collections;

namespace GamePlay.Interactables
{
    public enum VisitorStatus
    {
        Idle,
        RequestMarshmallow,
        RequestHotChocolate,
        RequestSausage
    }

    public class VisitorTarget : MonoBehaviour, IInteractable
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

        [SerializeField] private string interactionPrompt = "Talk to Visitor";
        public string InteractionPrompt => interactionPrompt;

        private void Start()
        {
            SetIdle();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
                SetIdle();
        }
    

        public void Interact(GameObject interactor, PlayerMovement playerMovement)
        {
            /* string heldItem = timur pls help i dont understand interactable script

            if (IsCorrectItem(heldItem))
            {
                SetIdle();
                //add score
                //addhappiness
            }
            else
            {
                SetIdle();
                //add less score
                //remove happiness
            }*/
        }

        private void ChooseRandomRequest()
        {
            int random = Random.Range(0, 3);
            VisitorStatus newStatus = (VisitorStatus)(random + 1);
            SetVisitorVisuals(newStatus);
        }

        public void SetIdle()
        {
            currentVisitorStatus = VisitorStatus.Idle;
            SetVisitorVisuals(VisitorStatus.Idle);
            if (idleRoutine != null)
                StopCoroutine(idleRoutine);

            idleRoutine = StartCoroutine(RandomRequestAfterIdle());
        }

        private IEnumerator RandomRequestAfterIdle()
        {
            float waitTime = Random.Range(20f, 40f);
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

            bool isCorrect = false;

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
                requestCanva.SetActive(false);
                return;
            }

            popupRoutine = StartCoroutine(ShowAlertThenIcon(newStatus));
        }

        private IEnumerator ShowAlertThenIcon(VisitorStatus status) // i cant anymore im just going to hard code it and fuck beeing the corect way
        {
            requestCanva.SetActive(true);
            alertIcon.SetActive(true);

            yield return new WaitForSeconds(alertDuration);

            switch (status)
            {
                case VisitorStatus.RequestMarshmallow:
                    SetIconsToFalse();
                    marshmallowIcon.SetActive(true);
                    break;
                case VisitorStatus.RequestHotChocolate:
                    SetIconsToFalse();
                    hotChocolateIcon.SetActive(true);
                    break;
                case VisitorStatus.RequestSausage:
                    SetIconsToFalse();
                    sausageIcon.SetActive(true);
                    break;
            }
        }
        
        private void SetIconsToFalse()
        {
            alertIcon.SetActive(false);
            marshmallowIcon.SetActive(false);
            hotChocolateIcon.SetActive(false);
            sausageIcon.SetActive(false);
        }
    }
}

