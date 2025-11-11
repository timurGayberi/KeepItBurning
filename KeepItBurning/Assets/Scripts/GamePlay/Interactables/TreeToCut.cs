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
        public string InteractionPrompt => interactionPrompt;

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

        private bool isChopping = false;
        private Coroutine chopCoroutineInstance;

        private void Awake()
        {
            if (currentTreeStatus == TreeStatus.Default)
            {
                currentTreeStatus = TreeStatus.Uncut;
            }

            SetTreeVisuals(currentTreeStatus);
        }
        
        public void Interact(GameObject interactor)
        {
            if (currentTreeStatus != TreeStatus.Uncut)
            {
                Debug.Log($"[CHOP BLOCKED] Tree is already cut ({currentTreeStatus}).");
                return;
            }

            if (isChopping)
            {
                Debug.Log($"[CHOP IN PROGRESS] Tree is already being chopped. Wait for completion.");
                return;
            }

            if (logPrefab == null)
            {
                Debug.LogError("[CHOP ERROR] Log Prefab is not assigned on TreeToCut component! Tree cannot drop resources.");
                return;
            }

            isChopping = true;
            

            Debug.Log($"[CHOP START] Player ({interactor.name}) started chopping! Will take {chopDuration} seconds.");
            chopCoroutineInstance = StartCoroutine(ChopTreeCoroutine(interactor));
        }
        
        public void StopInteraction()
        {
            if (chopCoroutineInstance != null)
            {
                StopCoroutine(chopCoroutineInstance);
                chopCoroutineInstance = null;
                isChopping = false; // Reset the internal flag
                Debug.Log("[TREE TO CUT] Chopping interaction successfully cancelled by player.");
            }
        }
        
        private IEnumerator ChopTreeCoroutine(GameObject interactor)
        {
            yield return new WaitForSeconds(chopDuration);
            
            
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
            
            isChopping = false;
            
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
            yield return new WaitForSeconds(regrowthTime);

            Debug.Log("[TREE TO CUT] Tree regrowth complete! Setting state to Uncut.");
            
            SetTreeVisuals(TreeStatus.Uncut);
        }
    }
}