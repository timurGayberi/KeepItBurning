using System.Collections;
using Interfaces;
using UnityEngine;

namespace GamePlay.Interactables
{
    public enum TreeStatus
    {
        Default,
        Cut,
        Uncut 
    }

    public class TreeToCut : MonoBehaviour, IInteractable
    {
        [Header("Visual References")]
        [SerializeField]
        private GameObject _trunk;

        [SerializeField]
        private GameObject _log;

        [SerializeField]
        private GameObject _leaves;

        public TreeStatus currentTreeStatus = TreeStatus.Default;

        [Header("Interaction Settings")]
        [Tooltip("The text prompt shown to the player.")]
        [SerializeField]
        private string interactionPrompt = "Chop Tree";
        

        [Header("Action Timings")] 
        [Tooltip("Time (in seconds) for the tree to regrow after being chopped.")]
        [SerializeField]
        private float regrowthTime = 30f;

        [Tooltip("The time (in seconds) required to fully chop down the tree.")]
        [SerializeField]
        private float chopDuration = 1.0f;

        [Header("Resource Output")]
        [Tooltip("The Log Prefab to be spawned when the tree is destroyed.")]
        [SerializeField]
        private GameObject logPrefab;

        [Tooltip("The number of logs that will be spawned.")]
        [SerializeField]
        private int numberOfLogs = 3;

        [Tooltip("Maximum radius logs will scatter from the tree's position.")]
        [SerializeField]
        private float scatterRadius = 0.5f;
        

        private void Awake()
        {
            if (currentTreeStatus == TreeStatus.Default)
            {
                currentTreeStatus = TreeStatus.Uncut;
            }

            SetTreeVisuals(currentTreeStatus);
        }
        
        public InteractionData GetInteractionData()
        {
            string prompt = interactionPrompt;
            float duration = chopDuration;
            
            if (currentTreeStatus != TreeStatus.Uncut)
            {
                prompt = $"Regrowing ({Mathf.CeilToInt(regrowthTime)}s remaining)"; // Placeholder prompt for regrowing
                // Note: The timer logic for regrowth is still internal to this component.
            }

            return new InteractionData
            {
                promptText = prompt,
                actionDuration = currentTreeStatus == TreeStatus.Uncut ? duration : -1f // Return -1 or 0 to block interaction if uncut
            };
        }
        
        public void Interact()
        {
            if (currentTreeStatus != TreeStatus.Uncut)
            {
                Debug.Log($"[CHOP BLOCKED] Tree is already cut ({currentTreeStatus}). Final interact step cancelled.");
                return;
            }

            if (logPrefab == null)
            {
                Debug.LogError("[CHOP ERROR] Log Prefab is not assigned on TreeToCut component! Tree cannot drop resources.");
                return;
            }
            
            Debug.Log($"[CHOP COMPLETE] Spawning {numberOfLogs} logs.");

            for (int i = 0; i < numberOfLogs; i++)
            {
                var randomCircle = Random.insideUnitCircle * scatterRadius;
                var spawnPosition = new Vector3(
                    transform.position.x + randomCircle.x,
                    transform.position.y + 0.1f, 
                    transform.position.z + randomCircle.y
                );

                Instantiate(logPrefab, spawnPosition, Quaternion.identity);
            }
            
            SetTreeVisuals(TreeStatus.Cut);
        }
        
        public void StopInteraction()
        {
            Debug.Log("[TREE TO CUT] Chopping interaction successfully cancelled by player.");
        }
        
        private void SetTreeVisuals(TreeStatus newStatus)
        {
            currentTreeStatus = newStatus;

            if (newStatus == TreeStatus.Cut)
            {
                if (_trunk != null) _trunk.SetActive(false);
                if (_leaves != null) _leaves.SetActive(false);
                
                this.enabled = false;
                Debug.Log("[TREE TO CUT] Interaction disabled. Tree is now a stump.");

                StartCoroutine(RegrowCoroutine());
            }
            else if (newStatus == TreeStatus.Uncut)
            {
                if (_trunk != null) _trunk.SetActive(true);
                if (_leaves != null) _leaves.SetActive(true);
                
                this.enabled = true;
            }
        }

        private IEnumerator RegrowCoroutine()
        {
            float timer = 0f;
            while (timer < regrowthTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            Debug.Log("[TREE TO CUT] Tree regrowth complete! Setting state to Uncut.");
            
            SetTreeVisuals(TreeStatus.Uncut);
        }
    }
}