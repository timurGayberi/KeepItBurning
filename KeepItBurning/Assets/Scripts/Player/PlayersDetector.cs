using General;
using UnityEngine;

namespace Player
{
    public class PlayersDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        public Transform detectionPoint;
        public float detectionRadius = 2f;
        public bool showGizmos = true;

        private PlayerInventory playerInventory;

        void Start()
        {
            playerInventory = ServiceLocator.GetService<PlayerInventory>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryCollect();
            }
        }

        private void TryCollect()
        {
            Collider[] hits = Physics.OverlapSphere(detectionPoint.position, detectionRadius);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Collectible")) continue;

                var collectible = hit.GetComponent<CollectibleBase>();
                if (collectible != null && playerInventory != null)
                {
                    CollectibleData data = collectible.GetCollectibleData();

                    bool added = playerInventory.AddCollectible(data);
                    if (added)
                    {
                        Debug.Log($"Collected: {data.ID}");
                        Destroy(hit.gameObject);
                    }
                    return;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos || detectionPoint == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionPoint.position, detectionRadius);
        }
    }
}