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
        
        public Component CurrentCandidate
        {
            get
            {
                // Priority 1: Collectible
                if (currentCollectible != null)
                {
                    return currentCollectible as Component;
                }
                // Priority 2: Interactable
                if (currentInteractable != null)
                {
                    return currentInteractable as Component;
                }
                return null;
            }
        }

        private void Update()
        {
            DetectTarget();
        }

        private void DetectTarget()
        {
            currentInteractable = null;
            currentCollectible = null;

            float effectiveRadius = detectionDistance; 
            int layerMask = ~ignoreLayers;

            // Use player's position for the sphere center
            Vector3 centerPosition = transform.position;
            
            // 1. FIND ALL CANDIDATES IN PROXIMITY
            Collider[] colliders = Physics.OverlapSphere(centerPosition, effectiveRadius, layerMask, QueryTriggerInteraction.Collide); 

            float minDistance = float.MaxValue;
            IInteractable closestInteractable = null;
            ICollectible closestCollectible = null;
            Vector3? closestTargetPosition = null; 

            // --- Simplified Priority Tracking ---
            ICollectible closestCollectibleCandidate = null;
            
            foreach (var collider in colliders) 
            {
                // Skip self-collider or colliders attached to the player's root
                if (collider.transform.root == transform.root) continue;

                // Simple check for unsupported colliders (prevents crashes)
                if (collider is MeshCollider meshCollider && !meshCollider.convex) continue;
                if (collider is TerrainCollider) continue;
                
                // Get the distance to the closest point on the collider
                float distance;
                try
                {
                    distance = Vector3.Distance(transform.position, collider.ClosestPoint(transform.position));
                }
                catch (System.Exception) 
                {
                    continue; // Skip any other physics exceptions silently
                }
                
                // Only evaluate if it's within the overall proximity range
                if (distance < effectiveRadius) 
                {
                    // Check if the tag is valid
                    bool isTargetTag = false;
                    foreach (string tag in targetTags)
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
                        
                        // Priority Check: Find the absolute closest overall target
                        if (distance < minDistance)
                        {
                            minDistance = distance;

                            // P1: Collectible check
                            if (collectible != null)
                            {
                                closestCollectibleCandidate = collectible;
                                // Temporarily prioritize the collectible for the final check
                                closestInteractable = null; 
                            }
                            // P2: Interactable check
                            else if (interactable != null)
                            {
                                closestInteractable = interactable;
                                // Only set this as the primary candidate if no collectible is closer
                                if (closestCollectibleCandidate == null)
                                {
                                    closestTargetPosition = collider.transform.position;
                                }
                            }
                        }
                        
                        // Keep track of the closest overall collectible found
                        if (collectible != null && (closestCollectibleCandidate == null || distance < Vector3.Distance(transform.position, (closestCollectibleCandidate as Component).transform.position)))
                        {
                            closestCollectibleCandidate = collectible;
                            closestTargetPosition = collider.transform.position;
                        }
                    }
                }
            }
            
            // --- FINAL PRIORITY RESOLUTION ---
            if (closestCollectibleCandidate != null)
            {
                // If ANY collectible was found nearby, it wins the final selection.
                currentCollectible = closestCollectibleCandidate;
                currentInteractable = null;
            }
            else
            {
                // Otherwise, use the closest interactable found (which may be null).
                currentCollectible = null;
                currentInteractable = closestInteractable;
            }
            // --- END FINAL RESOLUTION ---


            // SIMPLIFIED VISUAL FEEDBACK (Debugging)
            if (currentCollectible != null || currentInteractable != null)
            {
                // Get the position of the selected target for the debug line
                Vector3 targetPos = (currentCollectible as Component)?.transform.position ?? (currentInteractable as Component)?.transform.position ?? transform.position;
                
                var color = (currentCollectible != null) ? Color.blue : Color.green;
                // Draw a line from player's eye level down to the target
                Debug.DrawLine(transform.position + transform.up * 1.5f, targetPos, color);
            }
            else
            {
                // Draw red if nothing is found
                Debug.DrawRay(transform.position, transform.forward * effectiveRadius, Color.red);
            }
        }

        void OnDrawGizmosSelected()
        {
            // Gizmo shows the proximity sphere around the player
            Gizmos.color = (currentCollectible != null || currentInteractable != null) ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionDistance);
        }
    }
}