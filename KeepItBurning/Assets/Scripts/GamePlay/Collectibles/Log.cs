using UnityEngine;
using System.Collections;
using General;

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
                Debug.LogError("Collider not found!");
            }
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
        
        protected override void OnCollectedWithInstance(GameObject collectedObject)
        {
            //inventory.SetHasWood(true, collectedObject);
        }
    }
}