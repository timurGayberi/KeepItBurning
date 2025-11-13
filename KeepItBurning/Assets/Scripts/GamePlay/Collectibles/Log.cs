using UnityEngine;
using System.Collections;
using General;
using Player;

namespace GamePlay.Collectibles
{
    public class FireWoodLogs : CollectibleBase 
    {
        [Header("Fuel Settings")]
        [Tooltip("The amount of fuel added to the fireplace when consumed.")]
        [SerializeField] 
        public float FuelValue = 25f;
        
        private const float DropImmunityDuration = 0.5f; 
        private Collider _collider;

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider>();
            
            if (_collider == null)
            {
                Debug.LogError($"Collider not found on {gameObject.name}!");
            }
            
            collectibleID = CollectibleIDs.FIREWOOD_LOGS;
        }
        
        public void SetDropImmunity()
        {
            if (_collider != null) _collider.enabled = false;
            StartCoroutine(EnableCollectionAfterDelay(DropImmunityDuration));
        }

        private IEnumerator EnableCollectionAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_collider != null) _collider.enabled = true;
        }
        
        protected override bool OnCollectedWithInstance(GameObject interactor)
        {
            if (interactor.TryGetComponent(out PlayerInventory inventory))
            {
                CollectibleData data = GetCollectibleData();
                
                bool wasAdded = inventory.AddCollectible(data);
                
                return wasAdded;
            }
            
            return false;
        }

        public override CollectibleData GetCollectibleData()
        {
            return new CollectibleData(collectibleID, FuelValue);
        }
    }
}