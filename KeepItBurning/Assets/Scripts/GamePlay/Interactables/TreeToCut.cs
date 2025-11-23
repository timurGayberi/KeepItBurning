using System.Collections;
using Interfaces;
using UnityEngine;
using ScriptableObjects;
using General;
using System.Collections.Generic;

namespace GamePlay.Interactables
{
    public enum TreeStatus
    {
        Default,
        Cut,
        Uncut
    }

    public class TreeToCut : InteractableBase
    {
        [Header("Configuration")]
        [SerializeField] private TreeData treeData;

        [Header("Visual References")]
        [SerializeField] private GameObject logPrefab;
        [SerializeField] private GameObject _trunk;
        [SerializeField] private GameObject _leaves;

        [Header("Log Spawn Settings")]
        [Tooltip("Optional: Drag the spawn point child object here. If set, logs will spawn at this position instead of being calculated.")]
        [SerializeField] private Transform logSpawnPoint;

        [Header("Outline Control")]
        [Tooltip("Drag ONLY the Trunk renderer here (not Leaves)")]
        [SerializeField] private List<MeshRenderer> outlineRenderers = new List<MeshRenderer>();

        private Material[] _materialInstances;

        public TreeStatus currentTreeStatus = TreeStatus.Default;

        private float _currentHealth;
        private float _regrowTimer;
        private Coroutine _resetHealthCoroutine;
        private Collider _treeCollider;

        private Vector3 _lastHitterPosition;

        private void Awake()
        {
            _treeCollider = GetComponent<Collider>();

            if (currentTreeStatus == TreeStatus.Default) currentTreeStatus = TreeStatus.Uncut;

            if (treeData != null)
            {
                _currentHealth = treeData.treesHealth;
            }
            else
            {
                Debug.LogError($"[TreeToCut] TreeData missing on {gameObject.name}!");
            }

            // Fallback: If outlineRenderers is empty, try to auto-assign from _trunk
            if (outlineRenderers == null || outlineRenderers.Count == 0)
            {
                if (outlineRenderers == null) outlineRenderers = new List<MeshRenderer>();

                if (_trunk != null)
                {
                    var trunkRenderer = _trunk.GetComponent<MeshRenderer>();
                    if (trunkRenderer != null)
                    {
                        outlineRenderers.Add(trunkRenderer);
                        Debug.Log($"[TreeToCut] Auto-assigned trunk renderer from '_trunk' reference.");
                    }
                }

                // If still empty, try getting renderer from self
                if (outlineRenderers.Count == 0)
                {
                    var selfRenderer = GetComponent<MeshRenderer>();
                    if (selfRenderer != null)
                    {
                        outlineRenderers.Add(selfRenderer);
                        Debug.Log($"[TreeToCut] Auto-assigned renderer from self.");
                    }
                }
            }

            if (outlineRenderers != null && outlineRenderers.Count > 0)
            {
                _materialInstances = new Material[outlineRenderers.Count];
                for (int i = 0; i < outlineRenderers.Count; i++)
                {
                    if (outlineRenderers[i] != null)
                    {
                        _materialInstances[i] = outlineRenderers[i].material;
                    }
                }
            }

            SetTreeVisuals(currentTreeStatus);
            CloseOutline(); // Ensure outline is off at start
        }

        public override InteractionData GetInteractionData()
        {
            if (treeData == null) return new InteractionData { promptText = "Error", actionDuration = -1f };

            if (currentTreeStatus != TreeStatus.Uncut)
            {
                float remaining = treeData.regrowthTime - _regrowTimer;
                return new InteractionData { promptText = $"Regrowing ({Mathf.CeilToInt(remaining)}s)", actionDuration = -1f };
            }

            return new InteractionData { promptText = treeData.interactionPrompt, actionDuration = 0f };
        }

        public override void Interact() { }

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
                Vector3 dropZoneCenter;

                // Use spawn point if assigned, otherwise calculate from player position
                if (logSpawnPoint != null)
                {
                    dropZoneCenter = logSpawnPoint.position;
                }
                else
                {
                    Vector3 rawDirection = _lastHitterPosition - transform.position;
                    rawDirection.y = 0;
                    Vector3 directionToPlayer = rawDirection.normalized;
                    dropZoneCenter = transform.position + (directionToPlayer * 2.0f);
                }

                for (int i = 0; i < treeData.numberOfLogs; i++)
                {
                    Vector2 randomScatter = Random.insideUnitCircle * treeData.scatterRadius;

                    Vector3 finalPos = new Vector3(
                        dropZoneCenter.x + randomScatter.x,
                        dropZoneCenter.y + 0.5f,
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
                CloseOutline(); // Turn off outline when tree is cut
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

        #region Outline setup

        private void CloseOutline()
        {
            if (_materialInstances == null) return;

            for (int i = 0; i < _materialInstances.Length; i++)
            {
                if (_materialInstances[i] != null)
                {
                    _materialInstances[i].SetFloat("_OutlineSize", 0f);
                }
            }
        }

        private void OpenOutline()
        {
            if (_materialInstances == null)
            {
                Debug.LogError($"[TreeToCut] OpenOutline: _materialInstances is NULL on {gameObject.name}");
                return;
            }

            Debug.Log($"[TreeToCut] OpenOutline called on {gameObject.name}. Count: {_materialInstances.Length}");

            for (int i = 0; i < _materialInstances.Length; i++)
            {
                if (_materialInstances[i] != null)
                {
                    _materialInstances[i].SetFloat("_OutlineSize", 50f);
                    Debug.Log($"[TreeToCut] Set _OutlineSize to 50 on {_materialInstances[i].name}");
                }
                else
                {
                    Debug.LogError($"[TreeToCut] Material instance {i} is null!");
                }
            }
        }

        public override void OnFocus()
        {
            Debug.Log($"[TreeToCut] OnFocus called on {gameObject.name}. Status: {currentTreeStatus}");
            if (currentTreeStatus == TreeStatus.Uncut)
            {
                OpenOutline();
            }
        }

        public override void OnLoseFocus()
        {
            CloseOutline();
        }

        #endregion
    }
}