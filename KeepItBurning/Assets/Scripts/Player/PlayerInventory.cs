using UnityEngine;
using System;
using General;
using Interfaces;
using GamePlay.Collectibles;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        private IInputService _inputService;
        
        
        [Header("Resources")]
        [Tooltip("The current number of wood logs the player is carrying.")]
        [SerializeField]
        private int _woodCount = 0;

        [Tooltip("The maximum number of wood logs the player can carry.")]
        [SerializeField]
        private int maxWoodCount = 3;

        [Header("Visual Attachments")]
        [Tooltip("The GameObject representing the wood log visual, child of the player (active if WoodCount > 0).")]
        [SerializeField]
        private GameObject woodVisual; 

        [Header("Drop Settings")]
        [Tooltip("The Log Prefab that is instantiated when the player drops wood.")]
        [SerializeField]
        private GameObject woodLogPrefabForDropping;
        
        public bool HasWood => _woodCount > 0; 
        public int WoodCount => _woodCount;
        public int MaxWoodCount => maxWoodCount;
        public bool IsWoodInventoryFull => _woodCount >= maxWoodCount;
        
        public event Action<int> OnWoodCountChanged;
        
        private void OnEnable()
        {
            try
            {
                _inputService = ServiceLocator.GetService<IInputService>();
                _inputService.OnDropEvent += DropWood;
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError("IInputService not found. Error: " + e.Message);
            }
        }

        private void OnDisable()
        {
            if (_inputService != null)
            {
                _inputService.OnDropEvent -= DropWood;
            }
        }
        
        #region Service Locator Registration

        private void Awake()
        {
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
            
            if (woodVisual != null)
            {
                woodVisual.SetActive(HasWood);
            }
        }
        
        public bool AddCollectible(CollectibleData data)
        {
            if (data.ID == CollectibleIDs.FIREWOOD_LOGS)
            {
                if (IsWoodInventoryFull)
                {
                    Debug.Log("Cannot add wood, inventory is full.");
                    return false;
                }
                
                _woodCount++;
                UpdateWoodState();
                return true;
            }
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
            Debug.Log($"Inventory: Wood count set to {_woodCount}.");
        }
        
        /*
        public void ConsumeWood()
        {
            if (HasWood)
            {
                _woodCount--;
                UpdateWoodState();
                Debug.Log("Inventory: Wood consumed.");
            }
        }
        */
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
                Debug.Log("Inventory: Wood dropped.");
            }
            else
            {
                Debug.Log("Tried to drop wood, but have none.");
            }
        }
        
        #endregion
        
    }
}