using UnityEngine;

namespace Player
{
    public class CollectiblesLogic : MonoBehaviour
    {
        #region Variables
        
        [Header("Wood State")]
        [Tooltip("True if the player is currently holding wood.")]
        [SerializeField]
        private bool hasWood = false;
        
        [Tooltip("The GameObject representing the wood log visual, usually a child of the player.")]
        [SerializeField]
        private GameObject woodVisual;

        [Header("Wood Drop References")] 
        [Tooltip("The Log Prefab that is instantiated when the player drops wood (if the original collected instance is lost).")]
        [SerializeField]
        private GameObject woodForDrop;
        
        private GameObject woodToCarry;
        
        public bool HasWood => hasWood;
        
        #endregion

        public void Awake()
        {
            if (woodVisual != null) woodVisual.SetActive(hasWood);
        }
        
        #region Wood Methods
        
        public void SetHasWood(bool state, GameObject collectedInstance = null) 
        {
            if (hasWood != state)
            {
                if (state == true)
                {
                    if (collectedInstance != null)
                    {
                        woodToCarry = collectedInstance;
                        woodToCarry.SetActive(false);
                    }
                }
                else
                {
                    woodToCarry = null;
                }

                hasWood = state;
                
                if (woodVisual != null) woodVisual.SetActive(hasWood);
                
                Debug.Log($"Wood state changed to: {hasWood}.");
            }
        }
        
        public void DropWood(Vector3 dropPosition) 
        {
            if (hasWood)
            {
                if (woodToCarry != null)
                {
                    woodToCarry.transform.position = dropPosition;
                    woodToCarry.SetActive(true);
                    SetHasWood(false);
                    Debug.Log("Wood dropped successfully (original instance returned).");
                    return;
                }
                
                if (woodForDrop == null)
                {
                    Debug.LogError("FATAL DROP ERROR: Wood Log Prefab For Dropping is not assigned!");
                    return;
                }
                
                Instantiate(woodForDrop, dropPosition, Quaternion.identity);
                SetHasWood(false);
                Debug.Log("Wood dropped successfully (instantiated new collectible).");
            }
        }
        
        public void ConsumeWood() 
        {
            if (hasWood && woodToCarry != null)
            {
                Destroy(woodToCarry);
                SetHasWood(false);
                Debug.Log("[INVENTORY] Successfully consumed and destroyed carried wood instance.");
            }
            else
            {
                Debug.LogWarning("[INVENTORY] Attempted to consume wood, but no trackable instance was carried.");
            }
        }
        
        public GameObject GetCarriedWoodInstance() 
        {
            return woodToCarry;
        }
        
        #endregion
    }
}