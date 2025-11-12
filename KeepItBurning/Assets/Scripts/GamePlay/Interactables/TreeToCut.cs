using System.Collections;
using Interfaces;
using UnityEngine;
using ScriptableObjects; // Necessary to reference TreeData

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
        [Header("Configuration")]
        [Tooltip("The ScriptableObject containing all the tree's stats and output resources.")]
        [SerializeField]
        private TreeData treeData;
        
        [Header("Resource Output")]
        [Tooltip("The Log Prefab to be spawned when the tree is destroyed.")]
        public GameObject logPrefab;

        [Header("Visual References")]
        [SerializeField]
        private GameObject _trunk;

        [SerializeField]
        private GameObject _log; 

        [SerializeField]
        private GameObject _leaves;

        public TreeStatus currentTreeStatus = TreeStatus.Default;
        
        private float _regrowTimer;
        
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
            if (treeData == null)
            {
                Debug.LogError($"TreeData is not assigned on {gameObject.name}!");
                return new InteractionData { promptText = "Error: No Data", actionDuration = -1f };
            }

            // --- DATA RETRIEVED FROM SCRIPTABLE OBJECT ---
            string prompt = treeData.interactionPrompt;
            float duration = treeData.chopDuration;
            
            if (currentTreeStatus != TreeStatus.Uncut)
            {
                // Calculate remaining time using the local timer and the data's regrowth time
                float remainingTime = treeData.regrowthTime - _regrowTimer;

                prompt = $"Regrowing ({Mathf.CeilToInt(remainingTime)}s remaining)"; 
                
                // Block interaction if the tree is cut
                duration = -1f; 
            }

            return new InteractionData
            {
                promptText = prompt,
                actionDuration = duration
            };
        }
        
        public void Interact()
        {
            if (treeData == null)
            {
                Debug.LogError($"TreeData is not assigned on {gameObject.name}! Cannot interact.");
                return;
            }

            if (currentTreeStatus != TreeStatus.Uncut)
            {
                Debug.Log($"[CHOP BLOCKED] Tree is already cut ({currentTreeStatus}). Final interact step cancelled.");
                return;
            }

            if (logPrefab == null)
            {
                Debug.LogError("[CHOP ERROR] Log Prefab is not assigned in the TreeData Scriptable Object! Tree cannot drop resources.");
                return;
            }
            
            Debug.Log($"[CHOP COMPLETE] Spawning {treeData.numberOfLogs} logs.");

            // Spawning logic uses data from Scriptable Object
            for (int i = 0; i < treeData.numberOfLogs; i++)
            {
                var randomCircle = Random.insideUnitCircle * treeData.scatterRadius;
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
            // Reset the timer and start tracking regrowth time.
            _regrowTimer = 0f;
            
            // Regrowth time retrieved from Scriptable Object
            while (_regrowTimer < treeData.regrowthTime)
            {
                _regrowTimer += Time.deltaTime;
                yield return null;
            }

            Debug.Log("[TREE TO CUT] Tree regrowth complete! Setting state to Uncut.");
            
            SetTreeVisuals(TreeStatus.Uncut);
        }
    }
}