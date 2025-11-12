using UnityEngine;

namespace Player
{
    public class CollectiblesLogic : MonoBehaviour
    {
        #region Variables

        #region Wood_Variable

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

        #region Chocolate_Variables

        [Header("Chocolate State")]
        [Tooltip("True if the player is currently holding chocolate.")]
        [SerializeField] private bool hasChocolate = false;
        [SerializeField] private bool hasBurnedHotChocolate = false;
        [SerializeField] public bool hasHotChocolate = false;

        [Tooltip("The GameObject representing the chocolate visual, usually a child of the player.")]
        [SerializeField] public GameObject chocolateVisual;
        [SerializeField] public GameObject burnedHotChocolateVisual;
        [SerializeField] public GameObject hotChocolateVisual;

        [Header("Chocolate Drop References")]
        [Tooltip("The chocolate Prefab that is instantiated when the player drops chocolate (if the original collected instance is lost).")]
        [SerializeField] private GameObject chocolateForDrop;
        [SerializeField] private GameObject burnedHotChocolateForDrop;
        [SerializeField] private GameObject hotChocolateForDrop;

        private GameObject chocolateToCarry;
        private GameObject burnedHotChocolateToCarry;
        private GameObject hotChocolateToCarry;

        public bool HasChocolate => hasChocolate;
        public bool HasBurnedHotChocolate => hasBurnedHotChocolate;
        public bool HasHotChocolate => hasHotChocolate;

        #endregion

        #region Marshmallow_Variable

        [Header("Marshmallow State")]
        [Tooltip("True if the player is currently holding marshmallow.")]
        [SerializeField]
        private bool hasMarshmallow = false;

        [Tooltip("The GameObject representing the marshmallow visual, usually a child of the player.")]
        [SerializeField]
        private GameObject marshmallowVisual;

        [Header("Marshmallow Drop References")]
        [Tooltip("The Log Prefab that is instantiated when the player drops marshmallow (if the original collected instance is lost).")]
        [SerializeField]
        private GameObject marshmallowForDrop;

        private GameObject marshmallowToCarry;

        public bool HasMarshmallow => hasMarshmallow;

        #endregion

        #region Sausage_Variable

        [Header("Sausage State")]
        [Tooltip("True if the player is currently holding sausage.")]
        [SerializeField]
        private bool hasSausage = false;

        [Tooltip("The GameObject representing the sausage visual, usually a child of the player.")]
        [SerializeField]
        private GameObject sausageVisual;

        [Header("Sausage Drop References")]
        [Tooltip("The Sausage Prefab that is instantiated when the player drops sausage (if the original collected instance is lost).")]
        [SerializeField]
        private GameObject sausageForDrop;

        private GameObject sausageToCarry;

        public bool HasSausage => hasSausage;

        #endregion

        #endregion

        public void Awake()
        {
            if (woodVisual != null) woodVisual.SetActive(hasWood);
            if (chocolateVisual != null) chocolateVisual.SetActive(hasChocolate);
            if (burnedHotChocolateVisual != null) burnedHotChocolateVisual.SetActive(hasBurnedHotChocolate);
            if (hotChocolateVisual != null) hotChocolateVisual.SetActive(hotChocolateVisual);
            if (marshmallowVisual != null) marshmallowVisual.SetActive(hasMarshmallow);
            if (sausageVisual != null) sausageVisual.SetActive(hasSausage);
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

        #region Chocolate_Methods

        #region Chocolate

        public void SetHasChocolate(bool state, GameObject collectedInstance = null)
        {
            if (hasChocolate != state)
            {
                if (state == true)
                {
                    if (collectedInstance != null)
                    {
                        chocolateToCarry = collectedInstance;
                        chocolateToCarry.SetActive(false);
                    }
                }
                else
                {
                    chocolateToCarry = null;
                }

                hasChocolate = state;

                if (chocolateVisual != null) chocolateVisual.SetActive(hasChocolate);

                Debug.Log($"Chocolate state changed to: {hasChocolate}.");
            }
        }

        public void DropChocolate(Vector3 dropPosition)
        {
            if (hasChocolate)
            {
                if (chocolateToCarry != null)
                {
                    chocolateToCarry.transform.position = dropPosition;
                    chocolateToCarry.SetActive(true);
                    SetHasChocolate(false);
                    Debug.Log("Chocolate dropped successfully (original instance returned).");
                    return;
                }

                if (chocolateForDrop == null)
                {
                    Debug.LogError("FATAL DROP ERROR: Chocolate Prefab For Dropping is not assigned!");
                    return;
                }

                Instantiate(chocolateForDrop, dropPosition, Quaternion.identity);
                SetHasChocolate(false);
                Debug.Log("Chocolate dropped successfully (instantiated new collectible).");
            }
        }

        public void ConsumeChocolate()
        {
            if (hasChocolate && chocolateToCarry != null)
            {
                Destroy(chocolateToCarry);
                SetHasChocolate(false);
                Debug.Log("[INVENTORY] Successfully consumed and destroyed carried chocolate instance.");
            }
            else
            {
                Debug.LogWarning("[INVENTORY] Attempted to consume chocolate, but no trackable instance was carried.");
            }
        }

        public GameObject GetCarriedChocolateInstance()
        {
            return chocolateToCarry;
        }

        #endregion

        #region Burned Hot Chocolate

        public void SetHasBurnedHotChocolate(bool state, GameObject collectedInstance = null)
        {
            if (hasBurnedHotChocolate != state)
            {
                if (state == true)
                {
                    if (collectedInstance != null)
                    {
                        burnedHotChocolateToCarry = collectedInstance;
                        burnedHotChocolateToCarry.SetActive(false);
                    }
                }
                else
                {
                    burnedHotChocolateToCarry = null;
                }

                hasBurnedHotChocolate = state;

                if (burnedHotChocolateVisual != null) burnedHotChocolateVisual.SetActive(hasBurnedHotChocolate);

                Debug.Log($"Chocolate state changed to: {hasBurnedHotChocolate}.");
            }
        }

        public void DropBurnedHotChocolate(Vector3 dropPosition)
        {
            if (hasBurnedHotChocolate)
            {
                if (burnedHotChocolateToCarry != null)
                {
                    burnedHotChocolateToCarry.transform.position = dropPosition;
                    burnedHotChocolateToCarry.SetActive(true);
                    SetHasBurnedHotChocolate(false);
                    Debug.Log("Burned Hot Chocolate dropped successfully (original instance returned).");
                    return;
                }

                if (burnedHotChocolateForDrop == null)
                {
                    Debug.LogError("FATAL DROP ERROR: Chocolate Prefab For Dropping is not assigned!");
                    return;
                }

                Instantiate(burnedHotChocolateForDrop, dropPosition, Quaternion.identity);
                SetHasBurnedHotChocolate(false);
                Debug.Log("Burned Hot Chocolate dropped successfully (instantiated new collectible).");
            }
        }

        public void ConsumeBurnedHotChocolate()
        {
            if (hasBurnedHotChocolate && burnedHotChocolateToCarry != null)
            {
                Destroy(burnedHotChocolateToCarry);
                SetHasBurnedHotChocolate(false);
                Debug.Log("[INVENTORY] Successfully consumed and destroyed carried chocolate instance.");
            }
            else
            {
                Debug.LogWarning("[INVENTORY] Attempted to consume chocolate, but no trackable instance was carried.");
            }
        }

        public GameObject GetCarriedBurnedHotChocolateInstance()
        {
            return burnedHotChocolateToCarry;
        }

        #endregion

        #region Hot Chocolate

        public void SetHasHotChocolate(bool state, GameObject collectedInstance = null)
        {
            if (hasHotChocolate != state)
            {
                if (state == true)
                {
                    if (collectedInstance != null)
                    {
                        hotChocolateToCarry = collectedInstance;
                        hotChocolateToCarry.SetActive(false);
                    }
                }
                else
                {
                    hotChocolateToCarry = null;
                }

                hasHotChocolate = state;

                if (hotChocolateVisual != null) hotChocolateVisual.SetActive(hasHotChocolate);

                Debug.Log($"Chocolate state changed to: {hasHotChocolate}.");
            }
        }

        public void DropHotChocolate(Vector3 dropPosition)
        {
            if (hasHotChocolate)
            {
                if (hotChocolateToCarry != null)
                {
                    hotChocolateToCarry.transform.position = dropPosition;
                    hotChocolateToCarry.SetActive(true);
                    SetHasHotChocolate(false);
                    Debug.Log("Hot Chocolate dropped successfully (original instance returned).");
                    return;
                }

                if (hotChocolateForDrop == null)
                {
                    Debug.LogError("FATAL DROP ERROR: Chocolate Prefab For Dropping is not assigned!");
                    return;
                }

                Instantiate(hotChocolateForDrop, dropPosition, Quaternion.identity);
                SetHasHotChocolate(false);
                Debug.Log("Hot Chocolate dropped successfully (instantiated new collectible).");
            }
        }

        public void ConsumeHotChocolate()
        {
            if (hasHotChocolate && hotChocolateToCarry != null)
            {
                Destroy(hotChocolateToCarry);
                SetHasHotChocolate(false);
                Debug.Log("[INVENTORY] Successfully consumed and destroyed carried chocolate instance.");
            }
            else
            {
                Debug.LogWarning("[INVENTORY] Attempted to consume chocolate, but no trackable instance was carried.");
            }
        }

        public GameObject GetCarriedHotChocolateInstance()
        {
            return hotChocolateToCarry;
        }

        #endregion

        #endregion

        #region Marshmallow_Methods

        public void SetHasMarshmallow(bool state, GameObject collectedInstance = null)
        {
            if (hasMarshmallow != state)
            {
                if (state == true)
                {
                    if (collectedInstance != null)
                    {
                        marshmallowToCarry = collectedInstance;
                        marshmallowToCarry.SetActive(false);
                    }
                }
                else
                {
                    marshmallowToCarry = null;
                }

                hasMarshmallow = state;

                if (marshmallowVisual != null) marshmallowVisual.SetActive(hasMarshmallow);

                Debug.Log($"Marshmallow state changed to: {hasMarshmallow}.");
            }
        }

        public void DropMarshmallow(Vector3 dropPosition)
        {
            if (hasMarshmallow)
            {
                if (marshmallowToCarry != null)
                {
                    marshmallowToCarry.transform.position = dropPosition;
                    marshmallowToCarry.SetActive(true);
                    SetHasMarshmallow(false);
                    Debug.Log("Marshmallow dropped successfully (original instance returned).");
                    return;
                }

                if (marshmallowForDrop == null)
                {
                    Debug.LogError("FATAL DROP ERROR: Marshmallow Prefab For Dropping is not assigned!");
                    return;
                }

                Instantiate(marshmallowForDrop, dropPosition, Quaternion.identity);
                SetHasMarshmallow(false);
                Debug.Log("Marshmallow dropped successfully (instantiated new collectible).");
            }
        }

        public void ConsumeMarshmallow()
        {
            if (hasMarshmallow && marshmallowToCarry != null)
            {
                Destroy(marshmallowToCarry);
                SetHasMarshmallow(false);
                Debug.Log("[INVENTORY] Successfully consumed and destroyed carried marshmallow instance.");
            }
            else
            {
                Debug.LogWarning("[INVENTORY] Attempted to consume marshmallow, but no trackable instance was carried.");
            }
        }

        public GameObject GetCarriedMarshmallowInstance()
        {
            return marshmallowToCarry;
        }

        #endregion

        #region Sausage_Methods

        public void SetHasSausage(bool state, GameObject collectedInstance = null)
        {
            if (hasSausage != state)
            {
                if (state == true)
                {
                    if (collectedInstance != null)
                    {
                        sausageToCarry = collectedInstance;
                        sausageToCarry.SetActive(false);
                    }
                }
                else
                {
                    sausageToCarry = null;
                }

                hasSausage = state;

                if (sausageVisual != null) sausageVisual.SetActive(hasSausage);

                Debug.Log($"Sausage state changed to: {hasSausage}.");
            }
        }

        public void DropSausage(Vector3 dropPosition)
        {
            if (hasSausage)
            {
                if (sausageToCarry != null)
                {
                    sausageToCarry.transform.position = dropPosition;
                    sausageToCarry.SetActive(true);
                    SetHasSausage(false);
                    Debug.Log("Sausage dropped successfully (original instance returned).");
                    return;
                }

                if (sausageForDrop == null)
                {
                    Debug.LogError("FATAL DROP ERROR: Sausage Prefab For Dropping is not assigned!");
                    return;
                }

                Instantiate(sausageForDrop, dropPosition, Quaternion.identity);
                SetHasSausage(false);
                Debug.Log("Sausage dropped successfully (instantiated new collectible).");
            }
        }

        public void ConsumeSausage()
        {
            if (hasSausage && sausageToCarry != null)
            {
                Destroy(sausageToCarry);
                SetHasSausage(false);
                Debug.Log("[INVENTORY] Successfully consumed and destroyed carried sausage instance.");
            }
            else
            {
                Debug.LogWarning("[INVENTORY] Attempted to consume sausage, but no trackable instance was carried.");
            }
        }

        public GameObject GetCarriedSausageInstance()
        {
            return sausageToCarry;
        }

        #endregion
    }
}