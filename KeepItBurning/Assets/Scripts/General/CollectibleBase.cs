using UnityEngine;
using Interfaces;
using Player;

namespace General
{
    public abstract class CollectibleBase : MonoBehaviour, ICollectible
    {
        #region Visual Settings
        
        [Header("Visual Effects")]
        [Tooltip("How high the object bobs (vertical distance).")]
        [SerializeField] protected float amplitude = 0.2f;
        [Tooltip("How fast the object bobs (cycles per second).")]
        [SerializeField] protected float frequency = 1f;
        [Tooltip("Speed of rotation (degrees per second).")]
        [SerializeField] protected float rotationSpeed = 60f;
        
        private Vector3 _initialSpawnPosition; 
        private Vector3 _basePosition; 
        
        #endregion

        #region Collectible Settings
        [Header("Collectible Settings")]
        [Tooltip("A unique ID for this item (using constants from CollectibleIDs).")]
        [SerializeField] 
        protected int collectibleID = CollectibleIDs.DEFAULT_ITEM; 
        
        [Tooltip("The text prompt shown when the player looks at this object.")]
        [SerializeField] 
        protected string collectionPrompt = "Pick up Item";

        public string CollectionPrompt => collectionPrompt; 
        #endregion
        
        public virtual CollectibleData GetCollectibleData()
        {
            return new CollectibleData(collectibleID, 0f); 
        }
        
        public bool Collect(GameObject interactor)
        {
            if (interactor.TryGetComponent(out PlayerInventory inventory))
            {
                bool success = OnCollectedWithInstance(interactor); 
                
                if (success)
                {
                    OnCollected(); // Hook for sound/VFX
                    Debug.Log($"Collected: {gameObject.name} (ID: {collectibleID}).");
                    Destroy(gameObject); 
                }
                
                return success; 
            }
            else
            {
                Debug.LogError($"Interactor '{interactor.name}' is missing the PlayerInventory component! Cannot collect.");
                return false;
            }
        }
        
        public virtual void Drop(GameObject interactor)
        {
            Debug.Log($"Drop method called on {gameObject.name}.");
        }
        
        
        protected virtual void OnCollected() { }
        
        protected virtual bool OnCollectedWithInstance(GameObject interactor) 
        {
            return true;
        } 
        
        
        protected virtual void Awake()
        {
            _initialSpawnPosition = transform.position;
        }
        
        protected virtual void OnEnable()
        {
            _basePosition = transform.position;
        }

        protected virtual void Update() 
        {
            float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = _basePosition + new Vector3(0, yOffset, 0);
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}