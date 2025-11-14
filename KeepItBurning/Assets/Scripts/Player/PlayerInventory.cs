using UnityEngine;
using System;
using General;
using GamePlay.Collectibles;

namespace Player
{
    public enum CarryingType
    {
        None,
        Wood,
        Mushrooms,
        //More to come.
    }
    
    
    public class PlayerInventory : MonoBehaviour
    {
        
        private PlayersActivities _playersActivities;
        
        [Header("Resources")]
        [SerializeField]
        private int _woodCount = 0;
        [SerializeField]
        private int maxWoodCount = 3;

        [Header("Visual Attachments")]
        [SerializeField]
        private GameObject woodVisual;

        [Header("Hot Chocolate Visuals")]
        public GameObject chocolateVisual;
        public GameObject hotChocolateVisual;
        public GameObject burnedHotChocolateVisual;

        [Header("Food Items")]
        [SerializeField]
        private int currentHeldFoodItemID = CollectibleIDs.DEFAULT_ITEM; // 0 = nothing, 2 = marshmallow, 3 = hot chocolate, 4 = sausage

        [SerializeField]
        private CollectibleBase.CookState currentFoodCookState = CollectibleBase.CookState.Raw;

        [Header("Drop Settings")]
        [SerializeField]
        private GameObject woodLogPrefabForDropping;

        public bool HasWood => _woodCount > 0;
        public int WoodCount => _woodCount;
        public int MaxWoodCount => maxWoodCount;
        public bool IsWoodInventoryFull => _woodCount >= maxWoodCount;
        public event Action<int> OnWoodCountChanged;
        
        [Header("Current State")]
        [Tooltip("What is the player currently holding?")]
        public CarryingType CurrentCarryingType { get; private set; } = CarryingType.None;
        
        
        #region Service Locator Registration

        private void Awake()
        {
            _playersActivities = GetComponent<PlayersActivities>();
            if (_playersActivities == null)
            {
                Debug.LogError("PlayerInventory is missing a reference to PlayersActivities!");
            }
            
            try
            {
                ServiceLocator.RegisterService<PlayerInventory>(this);
            }
            catch (System.InvalidOperationException e)
            {
                Debug.LogError($"Failed to register PlayerInventory. Is one already registered? Error: {e.Message}");
            }
        }

        private void OnDestroy()
        {
            try
            {
                ServiceLocator.UnregisterService<PlayerInventory>(this);
            }
            catch (System.InvalidOperationException e)
            {
                Debug.LogWarning($"Failed to unregister PlayerInventory. Was it ever registered? Error: {e.Message}");
            }
        }

        #endregion

        private void Start()
        {
            UpdateWoodState();
        }
        
        private void Update()
        {
            if (_playersActivities == null) return;

            var currentState = _playersActivities.currentState;
            
            if (CurrentCarryingType != CarryingType.None)
            {
                if (currentState == PlayerState.IsIdle)
                {
                    _playersActivities.SetPlayerState(PlayerState.IsCarrying);
                }
            }
            else
            {
                if (currentState == PlayerState.IsCarrying)
                {
                    _playersActivities.SetPlayerState(PlayerState.IsIdle);
                }
            }
        }
        
        
        
        public bool AddCollectible(CollectibleData data)
        {

            if (data.ID == CollectibleIDs.FIREWOOD_LOGS)
            {
                // Cannot pick up wood while holding food
                if (IsHoldingFoodItem())
                {
                    return false;
                }

                if (IsWoodInventoryFull)
                {
                    return false;
                }
                SoundManager.Play(SoundAction.PickUpWood);
                _woodCount++;
                UpdateWoodState();
                return true;
            }
            
            // Consumebles logic to come 
            // else if (data.ID == CollectibleIDs.MUSHROOM)
            // {
            //    ...
            //    CurrentCarryingType = CarryingType.Mushrooms;
            //    ...
            // }
            
            return false;
        }
        
        #region WoodLogic
        
        private void UpdateWoodState()
        {
            if (woodVisual != null)
            {
                woodVisual.SetActive(HasWood);
            }
            OnWoodCountChanged?.Invoke(_woodCount);

            if (HasWood)
            {
                CurrentCarryingType = CarryingType.Wood;
            }
            else
            {
                if (CurrentCarryingType == CarryingType.Wood)
                {
                    CurrentCarryingType = CarryingType.None;
                }
            }
        }

        public bool ConsumeWood()
        {
            if (HasWood)
            {
                _woodCount--;
                UpdateWoodState();
                SoundManager.Play(SoundAction.DropWoodOnFire);
                return true;
            }

            return false;
        }
        
        public float GetWoodFuelValue()
        {
            if (woodLogPrefabForDropping == null)
            {
                Debug.LogError("[FUEL GET] woodLogPrefabForDropping is not assigned. Cannot determine fuel value.");
                return 0f;
            }
            
            if (woodLogPrefabForDropping.TryGetComponent(out FireWoodLogs logScript))
            {
                return logScript.FuelValue;
            }
            
            Debug.LogError("[FUEL GET] woodLogPrefabForDropping is missing FireWoodLogs component. Cannot determine fuel value.");
            return 0f;
        }
        
        public void DropWood()
        {
            if (HasWood)
            {
                if (woodLogPrefabForDropping == null)
                {
                    Debug.LogError("DropWood failed: 'woodLogPrefabForDropping' is not assigned! Cannot drop wood.");
                    return;
                }
                
                var dropPosition = transform.position + transform.forward * 1.5f;
                var droppedLog = Instantiate(woodLogPrefabForDropping, dropPosition, Quaternion.identity);

                if (droppedLog.TryGetComponent(out FireWoodLogs logScript))
                {
                    logScript.SetDropImmunity();
                }
                
                _woodCount--;
                UpdateWoodState();
                SoundManager.Play(SoundAction.DropWood);
            }
        }

        #endregion

        #region Food Item Logic

        /// <summary>
        /// Gets the ID of the currently held food item.
        /// </summary>
        public int GetCurrentHeldFoodItemID()
        {
            return currentHeldFoodItemID;
        }

        /// <summary>
        /// Gets the name of the currently held food item, or null if not holding any food.
        /// </summary>
        public string GetCurrentHeldFoodItemName()
        {
            switch (currentHeldFoodItemID)
            {
                case CollectibleIDs.MARSHMALLOW:
                    return "Marshmallow";
                case CollectibleIDs.HOT_CHOCOLATE:
                    return "HotChocolate";
                case CollectibleIDs.SAUSAGE:
                    return "Sausage";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Sets what food item the player is currently holding.
        /// </summary>
        public void SetHeldFoodItem(int foodItemID, CollectibleBase.CookState cookState = CollectibleBase.CookState.Raw)
        {
            currentHeldFoodItemID = foodItemID;
            currentFoodCookState = cookState;
        }

        /// <summary>
        /// Gets the cook state of the currently held food item.
        /// </summary>
        public CollectibleBase.CookState GetCurrentFoodCookState()
        {
            return currentFoodCookState;
        }

        /// <summary>
        /// Updates the cook state of the currently held food (for cooking mechanics).
        /// </summary>
        public void SetCurrentFoodCookState(CollectibleBase.CookState cookState)
        {
            currentFoodCookState = cookState;
        }

        /// <summary>
        /// Clears the currently held food item.
        /// </summary>
        public void ClearHeldFoodItem()
        {
            currentHeldFoodItemID = CollectibleIDs.DEFAULT_ITEM;
            currentFoodCookState = CollectibleBase.CookState.Raw;
        }

        /// <summary>
        /// Checks if the player is currently holding any food item.
        /// </summary>
        public bool IsHoldingFoodItem()
        {
            return currentHeldFoodItemID != CollectibleIDs.DEFAULT_ITEM;
        }

        #endregion
    }
}