using System.Collections;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;
using General;
using Player;

namespace GamePlay.Interactables
{
    public class CookingStation : MonoBehaviour, IInteractable
    {
        [Header("Cooking Configuration")]
        [SerializeField] private float baseTotalTime = 8f;

        [Header("UI References")]
        [SerializeField] private Canvas cookingCanvas;
        [SerializeField] private Slider cookingProgressSlider;

        [Header("Progress Bar Colors")]
        [SerializeField] private Image sliderFillImage;
        [SerializeField] private Color rawColor = new Color(1f, 0.5f, 0f);
        [SerializeField] private Color cookedColor = Color.green;
        [SerializeField] private Color burningColor = Color.yellow;
        [SerializeField] private Color burntColor = Color.red;
        [SerializeField] private float warningThreshold = 0.7f;

        [Header("Cooking Indicators")]
        [SerializeField] private RectTransform cookingIndicators;

        [Header("Food Visuals")]
        [SerializeField] private GameObject marshmallowRaw;
        [SerializeField] private GameObject marshmallowCooked;
        [SerializeField] private GameObject marshmallowBurnt;
        [SerializeField] private GameObject hotChocolateRaw;
        [SerializeField] private GameObject hotChocolateCooked;
        [SerializeField] private GameObject sausageRaw;
        [SerializeField] private GameObject sausageCooked;
        [SerializeField] private GameObject sausageBurnt;

        private bool isCooking = false;
        private int currentFoodID = CollectibleIDs.DEFAULT_ITEM;
        private CollectibleBase.CookState currentCookState = CollectibleBase.CookState.Raw;
        private float cookingTimer = 0f;
        private float cookTime = 0f;
        private float burnTime = 0f;

        private void Start()
        {
            if (cookingCanvas != null)
            {
                cookingCanvas.enabled = false;
            }

            if (cookingProgressSlider != null)
            {
                cookingProgressSlider.minValue = 0f;
                cookingProgressSlider.maxValue = 1f;
                cookingProgressSlider.value = 0f;
            }

            HideAllFoodVisuals();
        }

        private void Update()
        {
            if (Camera.main != null && cookingCanvas != null && cookingCanvas.enabled)
            {
                cookingCanvas.transform.LookAt(Camera.main.transform);
                cookingCanvas.transform.rotation = Quaternion.LookRotation(cookingCanvas.transform.position - Camera.main.transform.position);
            }

            if (isCooking)
            {
                cookingTimer += Time.deltaTime;
                UpdateProgressBar();

                if (currentCookState == CollectibleBase.CookState.Raw && cookingTimer >= cookTime)
                {
                    currentCookState = CollectibleBase.CookState.Cooked;
                }
                else if (currentCookState == CollectibleBase.CookState.Cooked && cookingTimer >= burnTime)
                {
                    if (currentFoodID == CollectibleIDs.HOT_CHOCOLATE)
                    {
                        isCooking = false;
                    }
                    else
                    {
                        currentCookState = CollectibleBase.CookState.Burnt;
                        UpdateFoodVisual();
                        UpdateProgressBar();
                        isCooking = false;
                    }
                }

                UpdateFoodVisual();
            }
        }

        public InteractionData GetInteractionData()
        {
            if (currentFoodID == CollectibleIDs.DEFAULT_ITEM)
            {
                PlayerInventory inventory = ServiceLocator.GetService<PlayerInventory>();
                if (inventory != null && inventory.IsHoldingFoodItem())
                {
                    return new InteractionData
                    {
                        promptText = "Place Food on Fire",
                        actionDuration = 0f
                    };
                }

                return new InteractionData
                {
                    promptText = "",
                    actionDuration = -1f
                };
            }
            else
            {
                string stateText = GetCookStateText();
                return new InteractionData
                {
                    promptText = $"Take {stateText} Food",
                    actionDuration = 0f
                };
            }
        }

        public void Interact()
        {
            PlayerInventory inventory = ServiceLocator.GetService<PlayerInventory>();
            if (inventory == null) return;

            if (currentFoodID == CollectibleIDs.DEFAULT_ITEM)
            {
                PlaceFoodOnFire(inventory);
            }
            else
            {
                TakeFoodFromFire(inventory);
            }
        }

        public void StopInteraction() { }

        private void PlaceFoodOnFire(PlayerInventory inventory)
        {
            SoundManager.Play(SoundAction.FoodDuringCookingOnCampfire);
            if (inventory.GetCurrentFoodCookState() == CollectibleBase.CookState.Cooked) return;
            if (inventory.GetCurrentFoodCookState() == CollectibleBase.CookState.Burnt) return;

            if (!inventory.IsHoldingFoodItem()) return;

            currentFoodID = inventory.GetCurrentHeldFoodItemID();
            currentCookState = inventory.GetCurrentFoodCookState();
            inventory.ClearHeldFoodItem();

            switch (currentFoodID)
            {
                case CollectibleIDs.MARSHMALLOW:
                    cookTime = baseTotalTime * 0.15f;
                    burnTime = baseTotalTime * 0.735f;
                    break;
                case CollectibleIDs.SAUSAGE:
                    cookTime = baseTotalTime * 0.374f;
                    burnTime = baseTotalTime * 0.864f;
                    break;
                case CollectibleIDs.HOT_CHOCOLATE:
                    cookTime = baseTotalTime * 0.585f;
                    burnTime = baseTotalTime * 1.0f;
                    break;
                default:
                    cookTime = 5f;
                    burnTime = 8f;
                    break;
            }

            isCooking = true;
            cookingTimer = 0f;

            if (cookingCanvas != null)
            {
                cookingCanvas.enabled = true;
            }

            UpdateCookingIndicatorPosition();
            UpdateFoodVisual();
        }

        private void TakeFoodFromFire(PlayerInventory inventory)
        {
            if (inventory.IsHoldingFoodItem()) return;

            inventory.SetHeldFoodItem(currentFoodID, currentCookState);

            currentFoodID = CollectibleIDs.DEFAULT_ITEM;
            currentCookState = CollectibleBase.CookState.Raw;
            isCooking = false;
            cookingTimer = 0f;

            if (cookingCanvas != null)
            {
                cookingCanvas.enabled = false;
            }

            HideAllFoodVisuals();
        }

        private void HideAllFoodVisuals()
        {
            if (marshmallowRaw != null) marshmallowRaw.SetActive(false);
            if (marshmallowCooked != null) marshmallowCooked.SetActive(false);
            if (marshmallowBurnt != null) marshmallowBurnt.SetActive(false);
            if (hotChocolateRaw != null) hotChocolateRaw.SetActive(false);
            if (hotChocolateCooked != null) hotChocolateCooked.SetActive(false);
            if (sausageRaw != null) sausageRaw.SetActive(false);
            if (sausageCooked != null) sausageCooked.SetActive(false);
            if (sausageBurnt != null) sausageBurnt.SetActive(false);
        }

        private void UpdateFoodVisual()
        {
            HideAllFoodVisuals();

            switch (currentFoodID)
            {
                case CollectibleIDs.MARSHMALLOW:
                    if (currentCookState == CollectibleBase.CookState.Raw && marshmallowRaw != null)
                        marshmallowRaw.SetActive(true);
                    else if (currentCookState == CollectibleBase.CookState.Cooked && marshmallowCooked != null)
                        marshmallowCooked.SetActive(true);
                    else if (currentCookState == CollectibleBase.CookState.Burnt && marshmallowBurnt != null)
                        marshmallowBurnt.SetActive(true);
                    break;
                case CollectibleIDs.HOT_CHOCOLATE:
                    if (currentCookState == CollectibleBase.CookState.Raw && hotChocolateRaw != null)
                        hotChocolateRaw.SetActive(true);
                    else if (currentCookState == CollectibleBase.CookState.Cooked && hotChocolateCooked != null)
                        hotChocolateCooked.SetActive(true);
                    break;
                case CollectibleIDs.SAUSAGE:
                    if (currentCookState == CollectibleBase.CookState.Raw && sausageRaw != null)
                        sausageRaw.SetActive(true);
                    else if (currentCookState == CollectibleBase.CookState.Cooked && sausageCooked != null)
                        sausageCooked.SetActive(true);
                    else if (currentCookState == CollectibleBase.CookState.Burnt && sausageBurnt != null)
                        sausageBurnt.SetActive(true);
                    break;
            }
        }

        private void UpdateCookingIndicatorPosition()
        {
            if (cookingIndicators == null) return;

            float posX = 0f;

            switch (currentFoodID)
            {
                case CollectibleIDs.MARSHMALLOW:
                    posX = -2.2f;
                    break;
                case CollectibleIDs.SAUSAGE:
                    posX = 14.3f;
                    break;
                case CollectibleIDs.HOT_CHOCOLATE:
                    posX = 34.03f;
                    break;
            }

            Vector3 localPos = cookingIndicators.localPosition;
            localPos.x = posX;
            cookingIndicators.localPosition = localPos;
        }

        private void UpdateProgressBar()
        {
            if (cookingProgressSlider == null) return;

            float totalTime = burnTime;
            float progress = Mathf.Clamp01(cookingTimer / totalTime);
            cookingProgressSlider.value = progress;

            Color barColor = rawColor;

            if (currentCookState == CollectibleBase.CookState.Raw)
            {
                barColor = rawColor;
            }
            else if (currentCookState == CollectibleBase.CookState.Cooked)
            {
                if (currentFoodID == CollectibleIDs.HOT_CHOCOLATE)
                {
                    barColor = cookedColor;
                }
                else
                {
                    float timeSinceCooking = cookingTimer - cookTime;
                    float timeUntilBurn = burnTime - cookTime;
                    float burnProgress = timeSinceCooking / timeUntilBurn;

                    if (burnProgress >= warningThreshold)
                    {
                        barColor = burningColor;
                    }
                    else
                    {
                        barColor = cookedColor;
                    }
                }
            }
            else if (currentCookState == CollectibleBase.CookState.Burnt)
            {
                barColor = burntColor;
            }

            if (sliderFillImage != null)
            {
                sliderFillImage.color = barColor;
            }
        }

        private string GetCookStateText()
        {
            switch (currentCookState)
            {
                case CollectibleBase.CookState.Raw:
                    return "Raw";
                case CollectibleBase.CookState.Cooked:
                    return "Cooked";
                case CollectibleBase.CookState.Burnt:
                    return "Burnt";
                default:
                    return "";
            }
        }

        private string GetFoodName(int foodID)
        {
            switch (foodID)
            {
                case CollectibleIDs.MARSHMALLOW:
                    return "Marshmallow";
                case CollectibleIDs.HOT_CHOCOLATE:
                    return "Hot Chocolate";
                case CollectibleIDs.SAUSAGE:
                    return "Sausage";
                default:
                    return "Food";
            }
        }
    }
}
