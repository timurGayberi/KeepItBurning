using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Player;
using General;

namespace UI
{
    public class CarryUIManager : MonoBehaviour
    {
        [Header("Wood UI Elements")]
        [SerializeField] private GameObject logsSingleUI;
        [SerializeField] private GameObject logsDoubleUI;
        [SerializeField] private GameObject logsTripleUI;
        [SerializeField] private TextMeshProUGUI woodCountText;

        [Header("Marshmallow UI Elements")]
        [SerializeField] private GameObject marshmallowRawUI;
        [SerializeField] private GameObject marshmallowCookedUI;
        [SerializeField] private GameObject marshmallowBurnedUI;

        [Header("Hot Chocolate UI Elements")]
        [SerializeField] private GameObject hotChocolateRawUI;
        [SerializeField] private GameObject hotChocolateCookedUI;
        // No burned state for hot chocolate

        [Header("Sausage UI Elements")]
        [SerializeField] private GameObject sausageRawUI;
        [SerializeField] private GameObject sausageCookedUI;
        [SerializeField] private GameObject sausageBurnedUI;

        private PlayerInventory playerInventory;

        private void Start()
        {
            // Get PlayerInventory from ServiceLocator
            playerInventory = ServiceLocator.GetService<PlayerInventory>();

            if (playerInventory == null)
            {
                Debug.LogError("[CarryUIManager] Could not find PlayerInventory!");
                return;
            }

            // Subscribe to wood count changes
            playerInventory.OnWoodCountChanged += OnWoodCountChanged;

            // Initial update
            UpdateUI();
        }

        private void OnDestroy()
        {
            if (playerInventory != null)
            {
                playerInventory.OnWoodCountChanged -= OnWoodCountChanged;
            }
        }

        private void Update()
        {
            // Update UI every frame to catch food changes
            // (We could optimize this with events later)
            UpdateUI();
        }

        private void OnWoodCountChanged(int newCount)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (playerInventory == null) return;

            // Hide everything first
            HideAllUI();

            // Check what player is holding
            int woodCount = playerInventory.WoodCount;
            int foodID = playerInventory.GetCurrentHeldFoodItemID();

            // Show wood UI if carrying wood
            if (woodCount > 0)
            {
                ShowWoodUI(woodCount);
            }
            // Show food UI if carrying food
            else if (foodID != CollectibleIDs.DEFAULT_ITEM)
            {
                ShowFoodUI(foodID);
            }
        }

        private void ShowWoodUI(int count)
        {
            // Show appropriate wood visual based on count
            if (logsSingleUI != null && count == 1)
                logsSingleUI.SetActive(true);
            else if (logsDoubleUI != null && count == 2)
                logsDoubleUI.SetActive(true);
            else if (logsTripleUI != null && count >= 3)
                logsTripleUI.SetActive(true);

            // Update wood count text
            if (woodCountText != null)
            {
                woodCountText.text = count.ToString();
            }
        }

        private void ShowFoodUI(int foodID)
        {
            // Get cook state from PlayerInventory
            var cookState = playerInventory.GetCurrentFoodCookState();

            switch (foodID)
            {
                case CollectibleIDs.MARSHMALLOW:
                    ShowMarshmallowUI(cookState);
                    break;
                case CollectibleIDs.HOT_CHOCOLATE:
                    ShowHotChocolateUI(cookState);
                    break;
                case CollectibleIDs.SAUSAGE:
                    ShowSausageUI(cookState);
                    break;
            }
        }

        private void ShowMarshmallowUI(CollectibleBase.CookState state)
        {
            switch (state)
            {
                case CollectibleBase.CookState.Raw:
                    if (marshmallowRawUI != null) marshmallowRawUI.SetActive(true);
                    break;
                case CollectibleBase.CookState.Cooked:
                    if (marshmallowCookedUI != null) marshmallowCookedUI.SetActive(true);
                    break;
                case CollectibleBase.CookState.Burnt:
                    if (marshmallowBurnedUI != null) marshmallowBurnedUI.SetActive(true);
                    break;
            }
        }

        private void ShowHotChocolateUI(CollectibleBase.CookState state)
        {
            switch (state)
            {
                case CollectibleBase.CookState.Raw:
                    if (hotChocolateRawUI != null) hotChocolateRawUI.SetActive(true);
                    break;
                case CollectibleBase.CookState.Cooked:
                    if (hotChocolateCookedUI != null) hotChocolateCookedUI.SetActive(true);
                    break;
                // Hot chocolate cannot be burned
            }
        }

        private void ShowSausageUI(CollectibleBase.CookState state)
        {
            switch (state)
            {
                case CollectibleBase.CookState.Raw:
                    if (sausageRawUI != null) sausageRawUI.SetActive(true);
                    break;
                case CollectibleBase.CookState.Cooked:
                    if (sausageCookedUI != null) sausageCookedUI.SetActive(true);
                    break;
                case CollectibleBase.CookState.Burnt:
                    if (sausageBurnedUI != null) sausageBurnedUI.SetActive(true);
                    break;
            }
        }

        private void HideAllUI()
        {
            // Hide all wood UI
            if (logsSingleUI != null) logsSingleUI.SetActive(false);
            if (logsDoubleUI != null) logsDoubleUI.SetActive(false);
            if (logsTripleUI != null) logsTripleUI.SetActive(false);

            // Hide all marshmallow UI
            if (marshmallowRawUI != null) marshmallowRawUI.SetActive(false);
            if (marshmallowCookedUI != null) marshmallowCookedUI.SetActive(false);
            if (marshmallowBurnedUI != null) marshmallowBurnedUI.SetActive(false);

            // Hide all hot chocolate UI
            if (hotChocolateRawUI != null) hotChocolateRawUI.SetActive(false);
            if (hotChocolateCookedUI != null) hotChocolateCookedUI.SetActive(false);

            // Hide all sausage UI
            if (sausageRawUI != null) sausageRawUI.SetActive(false);
            if (sausageCookedUI != null) sausageCookedUI.SetActive(false);
            if (sausageBurnedUI != null) sausageBurnedUI.SetActive(false);

            // Hide wood count text
            if (woodCountText != null) woodCountText.text = "";
        }
    }
}
