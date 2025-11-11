using UnityEngine;
using Interfaces;

namespace Player
{
    public class PlayersInteractionTargetDetector : MonoBehaviour
    {
        [Header("Detection Settings")]
        [SerializeField] private float detectionRadius ;
        [SerializeField] private float detectionDistance ;
        
        [Tooltip("The tags we consider valid targets (e.g., 'Interactable', 'Collectible').")]
        [SerializeField] private string[] targetTags = { "Interactable", "Collectible" }; 
        
        public IInteractable currentInteractable { get; private set; }

        private void Update()
        {
            DetectTarget();
        }

        private void DetectTarget()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            
            if (Physics.SphereCast(ray, detectionRadius, out hit, detectionDistance, ~0)) 
            {
                string hitTag = hit.collider.gameObject.tag;
                bool isTargetTag = false;
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
                    IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                    
                    currentInteractable = interactable;
                    
                    if (interactable != null)
                    {
                        Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                    }
                }
                else
                {
                    currentInteractable = null;
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
                }
            }
            else
            {
                currentInteractable = null;
                Debug.DrawRay(ray.origin, ray.direction * detectionDistance, Color.red);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = currentInteractable != null ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position + transform.forward * detectionDistance, detectionRadius);
        }
    }
}