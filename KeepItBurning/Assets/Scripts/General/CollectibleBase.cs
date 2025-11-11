using UnityEngine;
using Interfaces;
using Player;

namespace General
{
    public abstract class CollectibleBase : MonoBehaviour, ICollectible
    {
        [Header("Visual Effects")]
        [Tooltip("How high the object bobs (vertical distance).")]
        [SerializeField] protected float amplitude = 0.2f;
        [Tooltip("How fast the object bobs (cycles per second).")]
        [SerializeField] protected float frequency = 1f;
        [Tooltip("Speed of rotation (degrees per second).")]
        [SerializeField] protected float rotationSpeed = 60f;
        
        private Vector3 _initialSpawnPosition; 
        private Vector3 _basePosition; 

        [Header("Collectible Settings")]
        [Tooltip("The text prompt shown when the player looks at this object.")]
        [SerializeField] 
        protected string collectionPrompt = "Pick up Item";

        [Header("Alpha Spawner Settings")]
        //[HideInInspector] public AlphaSpawnManager originSpawner;
        //[HideInInspector] public SpawnPointData originPoint;

        public string CollectionPrompt => collectionPrompt; 
        
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
        
        public void Collect(GameObject interactor)
        {
            //PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
            
            /*

            if (inventory != null)
            {
                // Call the virtual method that includes the collected instance.
                //OnCollectedWithInstance(inventory, this.gameObject); 
                
                Debug.Log($"Collected: {gameObject.name}. Inventory state updated.");
            }
            else
            {
                Debug.LogError($"Interactor '{interactor.name}' is missing the PlayerInventory component!");
            }
            
            */
        }
        
        protected virtual void OnCollected() { }
        protected virtual void OnCollectedWithInstance( GameObject collectedObject)
        {
            // By default, just call the simple hook for tools like the Axe/Lantern
            //OnCollected(inventory);
        }
    }
}
