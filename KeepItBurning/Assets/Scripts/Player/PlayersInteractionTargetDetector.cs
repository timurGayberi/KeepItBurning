using UnityEngine;
using Interfaces;

namespace Player
{
    public class PlayersInteractionTargetDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float      detectionRadius ;
        [SerializeField] private float      detectionDistance ;
        
        [Tooltip("The tags we consider valid targets (e.g., 'Interactable', 'Collectible').")]
        [SerializeField] private string[] targetTags = { "Interactable", "Collectible" }; 
        
        // Expose both potential interaction types
        public IInteractable currentInteractable { get; private set; }
        public ICollectible currentCollectible { get; private set; } // New property for collectibles
        
        private void Update()
        {
            DetectTarget();
        }
        
        private void DetectTarget()
        {
            
            currentInteractable = null;
            currentCollectible = null;
            
            var ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            
            if (Physics.SphereCast(ray, detectionRadius, out hit, detectionDistance, ~0)) 
            {
                var hitTag = hit.collider.gameObject.tag;
                var isTargetTag = false;
                

                foreach (string tag in targetTags)
                {
                    if (hitTag.Equals(tag))
                    {
                        isTargetTag = true;
                        break;
                    }
                }
                
                if (isTargetTag)
                {
                    var collectible = hit.collider.GetComponent<ICollectible>();
                    if (collectible != null)
                    {
                        currentCollectible = collectible;
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.blue); 
                        return; 

                    }
                    
                    var interactable = hit.collider.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        currentInteractable = interactable;
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                    }
                }
                else
                {
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * detectionDistance, Color.red);
            }
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = (currentCollectible != null || currentInteractable != null) ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position + transform.forward * detectionDistance, detectionRadius);
        }
    }
}