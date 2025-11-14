using UnityEngine;
using Interfaces;

namespace Player
{
    public class PlayersInteractionTargetDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [Tooltip("Radius of the detection sphere cast forward.")]
        [SerializeField] private float detectionRadius = 0.5f; 
        [Tooltip("Maximum distance the sphere is cast.")]
        [SerializeField] private float detectionDistance = 3f; 

        [Tooltip("The tags we consider valid targets (e.g., 'Interactable', 'Collectible').")]
        [SerializeField] private string[] targetTags = { "Interactable", "Collectible" }; 

        [Header("Physics Filter")]
        [Tooltip("Layers to IGNORE during raycasting (Should include 'Player').")]
        [SerializeField] private LayerMask ignoreLayers;
        
        public IInteractable currentInteractable { get; private set; }
        public ICollectible currentCollectible { get; private set; }

        private System.Collections.Generic.List<IInteractable> allNearbyInteractables = new System.Collections.Generic.List<IInteractable>();

        public Component CurrentCandidate
        {
            get
            {
                if (currentCollectible != null)
                {
                    return currentCollectible as Component;
                }
                if (currentInteractable != null)
                {
                    return currentInteractable as Component;
                }
                return null;
            }
        }

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
            currentInteractable = null;
            currentCollectible = null;
            allNearbyInteractables.Clear();

            var effectiveRadius = detectionDistance;
            var layerMask = ~ignoreLayers;

            Vector3 centerPosition = transform.position;

            Collider[] colliders = Physics.OverlapSphere(centerPosition, effectiveRadius, layerMask, QueryTriggerInteraction.Collide);

            float minDistance = float.MaxValue;
            IInteractable closestInteractable = null;
            ICollectible closestCollectible = null;
            Vector3? closestTargetPosition = null;

            ICollectible closestCollectibleCandidate = null;
            
            foreach (var collider in colliders) 
            {
                if (collider.transform.root == transform.root) continue;

                if (collider is MeshCollider meshCollider && !meshCollider.convex) continue;
                if (collider is TerrainCollider) continue;
                
                float distance;
                try
                {
                    distance = Vector3.Distance(transform.position, collider.ClosestPoint(transform.position));
                }
                catch (System.Exception) 
                {
                    continue;
                }
                
                if (distance < effectiveRadius) 
                {
                    var isTargetTag = false;
                    foreach (var tag in targetTags)
                    {
                        if (collider.CompareTag(tag))
                        {
                            isTargetTag = true;
                            break;
                        }
                    }
                    
                    if (isTargetTag)
                    {
                        var collectible = collider.GetComponent<ICollectible>();
                        var interactable = collider.GetComponent<IInteractable>();

                        // Collect ALL interactables in range for multi-interaction
                        if (interactable != null)
                        {
                            allNearbyInteractables.Add(interactable);
                        }

                        if (distance < minDistance)
                        {
                            minDistance = distance;

                            if (collectible != null)
                            {
                                closestCollectibleCandidate = collectible;
                                closestInteractable = null;
                            }
                            else if (interactable != null)
                            {
                                closestInteractable = interactable;
                                if (closestCollectibleCandidate == null)
                                {
                                    closestTargetPosition = collider.transform.position;
                                }
                            }
                        }

                        if (collectible != null && (closestCollectibleCandidate == null || distance < Vector3.Distance(transform.position, (closestCollectibleCandidate as Component).transform.position)))
                        {
                            closestCollectibleCandidate = collectible;
                            closestTargetPosition = collider.transform.position;
                        }
                    }
                }
            }
            
            if (closestCollectibleCandidate != null)
            {
                currentCollectible = closestCollectibleCandidate;
                currentInteractable = null;
            }
            else
            {
                currentCollectible = null;
                currentInteractable = closestInteractable;
            }

            
            if (currentCollectible != null || currentInteractable != null)
            {
                var targetPos = (currentCollectible as Component)?.transform.position ?? (currentInteractable as Component)?.transform.position ?? transform.position;
                var color = (currentCollectible != null) ? Color.blue : Color.green;
                Debug.DrawLine(transform.position + transform.up * 1.5f, targetPos, color);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.forward * effectiveRadius, Color.red);
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = (currentCollectible != null || currentInteractable != null) ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionDistance);
        }
    }
}