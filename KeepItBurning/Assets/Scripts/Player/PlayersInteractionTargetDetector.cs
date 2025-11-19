using UnityEngine;
using Interfaces;
using GamePlay.Interactables;

namespace Player
{
    public class PlayersInteractionTargetDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius = 0.5f; 
        [SerializeField] private float detectionDistance = 3f; 
        [SerializeField] private string[] targetTags = { "Interactable", "Collectible" }; 
        [SerializeField] private LayerMask ignoreLayers;
        
        // We keep both active at the same time now
        public IInteractable currentInteractable { get; private set; }
        public ICollectible currentCollectible { get; private set; }

        private System.Collections.Generic.List<IInteractable> allNearbyInteractables = new System.Collections.Generic.List<IInteractable>();

        public System.Collections.Generic.List<IInteractable> GetAllNearbyInteractables()
        {
            return allNearbyInteractables;
        }

        private void Update()
        {
            DetectTarget();
        }

        private void DetectTarget()
        {
            // Reset everything
            currentInteractable = null;
            currentCollectible = null;
            allNearbyInteractables.Clear();

            var centerPosition = transform.position;
            var colliders = Physics.OverlapSphere(centerPosition, detectionDistance, ~ignoreLayers, QueryTriggerInteraction.Collide);

            var minInteractableDist = float.MaxValue;
            var minCollectibleDist = float.MaxValue;

            foreach (var collider in colliders) 
            {
                if (collider.transform.root == transform.root) continue; // Ignore self

                // Check Tags
                bool isTarget = false;
                foreach(var tag in targetTags) { if(collider.CompareTag(tag)) { isTarget = true; break; } }
                if (!isTarget) continue;

                // Get Distance
                float dist = Vector3.Distance(transform.position, collider.ClosestPoint(transform.position));
                if (dist > detectionDistance) continue;

                // Check Components
                var interactable = collider.GetComponent<IInteractable>();
                var collectible = collider.GetComponent<ICollectible>();

                // --- Logic for Interactables ---
                if (interactable != null)
                {
                    // Skip cut trees
                    if (interactable is TreeToCut tree && tree.currentTreeStatus == TreeStatus.Cut) continue;

                    allNearbyInteractables.Add(interactable);

                    if (dist < minInteractableDist)
                    {
                        minInteractableDist = dist;
                        currentInteractable = interactable;
                    }
                }

                // --- Logic for Collectibles ---
                if (collectible != null)
                {
                    if (dist < minCollectibleDist)
                    {
                        minCollectibleDist = dist;
                        currentCollectible = collectible;
                    }
                }
            }

            // DEBUG: Visualize what we see
            if (currentInteractable != null)
                Debug.DrawLine(transform.position + Vector3.up, (currentInteractable as Component).transform.position, Color.green);
            
            if (currentCollectible != null)
                Debug.DrawLine(transform.position + Vector3.up * 0.5f, (currentCollectible as Component).transform.position, Color.blue);
        }
    }
}