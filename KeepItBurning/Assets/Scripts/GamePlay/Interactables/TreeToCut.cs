using System.Collections;
using Interfaces;
using UnityEngine;
using ScriptableObjects;

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
        [SerializeField] private TreeData treeData;
        
        [Header("Visual References")]
        [SerializeField] private GameObject logPrefab;
        [SerializeField] private GameObject _trunk;
        [SerializeField] private GameObject _leaves;
        
        public TreeStatus currentTreeStatus = TreeStatus.Default;
        
        private float _currentHealth;
        private float _regrowTimer;
        private Coroutine _resetHealthCoroutine;
        private Collider _treeCollider; 
        
        // Stores where the player was standing during the last hit
        private Vector3 _lastHitterPosition;

        private void Awake()
        {
            _treeCollider = GetComponent<Collider>();
            
            if (currentTreeStatus == TreeStatus.Default) currentTreeStatus = TreeStatus.Uncut;
            
            // Safety check to prevent crash if Data is missing
            if (treeData != null) 
            {
                _currentHealth = treeData.treesHealth;
            }
            else
            {
                Debug.LogError($"[TreeToCut] TreeData missing on {gameObject.name}!");
            }
            
            SetTreeVisuals(currentTreeStatus);
        }
        
        public InteractionData GetInteractionData()
        {
            if (treeData == null) return new InteractionData { promptText = "Error", actionDuration = -1f };

            if (currentTreeStatus != TreeStatus.Uncut)
            {
                float remaining = treeData.regrowthTime - _regrowTimer;
                return new InteractionData { promptText = $"Regrowing ({Mathf.CeilToInt(remaining)}s)", actionDuration = -1f };
            }
            
            return new InteractionData { promptText = treeData.interactionPrompt, actionDuration = 0f };
        }
        
        public void Interact() { } 
        public void StopInteraction() { }

        public void ApplyDamage(float damageAmount, Vector3 playerPosition)
        {
            if (currentTreeStatus != TreeStatus.Uncut) return;
            
            _lastHitterPosition = playerPosition;
            _currentHealth -= damageAmount;

            if (_currentHealth <= 0)
            {
                if (_resetHealthCoroutine != null) StopCoroutine(_resetHealthCoroutine);
                CutDownTree();
            }
            else
            {
                if (_resetHealthCoroutine != null) StopCoroutine(_resetHealthCoroutine);
                _resetHealthCoroutine = StartCoroutine(ResetHealthRoutine());
            }
        }
        
        private void CutDownTree()
        {
            Debug.Log($"[CHOP COMPLETE] Tree Destroyed.");

            if (logPrefab != null)
            {
                // 1. Get Vector from Tree to Player
                Vector3 rawDirection = _lastHitterPosition - transform.position;
                
                // 2. Flatten Y BEFORE normalizing. 
                // This ensures we get a pure horizontal direction of length 1.
                rawDirection.y = 0; 
                Vector3 directionToPlayer = rawDirection.normalized;

                // 3. Determine the "Drop Zone Center"
                // 1.0f is the offset distance towards the player
                Vector3 dropZoneCenter = transform.position + (directionToPlayer * 1.0f);

                for (int i = 0; i < treeData.numberOfLogs; i++)
                {
                    // 4. Random Scatter
                    Vector2 randomScatter = Random.insideUnitCircle * treeData.scatterRadius;

                    Vector3 finalPos = new Vector3(
                        dropZoneCenter.x + randomScatter.x,
                        transform.position.y + 0.5f, // Lift up slightly so they don't clip ground
                        dropZoneCenter.z + randomScatter.y
                    );

                    Instantiate(logPrefab, finalPos, Random.rotation);
                }
            }
            else
            {
                Debug.LogError("[TreeToCut] Log Prefab is missing!");
            }

            SoundManager.Play(SoundAction.DropWood);
            SetTreeVisuals(TreeStatus.Cut);
        }

        private IEnumerator ResetHealthRoutine()
        {
            yield return new WaitForSeconds(treeData.damageResetTime);
            _currentHealth = treeData.treesHealth;
            _resetHealthCoroutine = null;
        }
        
        private void SetTreeVisuals(TreeStatus newStatus)
        {
            currentTreeStatus = newStatus;
            bool isActive = (newStatus == TreeStatus.Uncut);

            if (_trunk != null) _trunk.SetActive(isActive);
            if (_leaves != null) _leaves.SetActive(isActive);
            
            this.enabled = isActive;
            if (_treeCollider != null) _treeCollider.enabled = isActive;

            if (newStatus == TreeStatus.Cut)
            {
                StartCoroutine(RegrowCoroutine());
            }
            else if (newStatus == TreeStatus.Uncut)
            {
                if (treeData != null) _currentHealth = treeData.treesHealth;
            }
        }

        private IEnumerator RegrowCoroutine()
        {
            _regrowTimer = 0f;
            while (_regrowTimer < treeData.regrowthTime)
            {
                _regrowTimer += Time.deltaTime;
                yield return null;
            }
            SetTreeVisuals(TreeStatus.Uncut);
        }
    }
}