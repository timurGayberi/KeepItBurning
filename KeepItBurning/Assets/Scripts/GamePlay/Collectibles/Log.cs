using UnityEngine;
using System.Collections;
using General;
using Player;
using Unity.VisualScripting;

namespace GamePlay.Collectibles
{
    public class FireWoodLogs : CollectibleBase
    {
        [Header("Fuel Settings")]
        [Tooltip("The amount of fuel added to the fireplace when consumed.")]
        [SerializeField]
        public float FuelValue = 25f;

        [Header("Auto-Destroy Settings")]
        [Tooltip("Time in seconds before the log is automatically destroyed (0 = never).")]
        [SerializeField]
        private float autoDestroyTime = 60f;

        private const float DropImmunityDuration = 0.5f;
        private Collider _collider;
        private float _spawnTime;

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider>();
            _spawnTime = Time.time;

            if (_collider == null)
            {
                Debug.LogError($"Collider not found on {gameObject.name}!");
            }
            collectibleID = CollectibleIDs.FIREWOOD_LOGS;
        }

        protected override void Update()
        {
            base.Update();

            Vector3 pos = transform.position;
            if (pos.y != 1f)
            {
                pos.y = 1f;
                transform.position = pos;
            }

            if (autoDestroyTime > 0 && Time.time - _spawnTime >= autoDestroyTime)
            {
                Destroy(gameObject);
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

        protected override bool OnCollectedWithInstance(GameObject interactor)
        {
            if (interactor.TryGetComponent(out PlayerInventory inventory))
            {
                CollectibleData data = GetCollectibleData();

                // Adds the fuel value and ID to the inventory count
                bool wasAdded = inventory.AddCollectible(data);

                return wasAdded;
            }

            return false;
        }

        public override CollectibleData GetCollectibleData()
        {
            // Returns the unique ID and the fuel value for this log
            return new CollectibleData(collectibleID, FuelValue);
        }
    }
}