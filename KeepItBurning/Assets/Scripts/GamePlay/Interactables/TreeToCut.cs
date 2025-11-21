using System.Collections;
using Interfaces;
using UnityEngine;
using ScriptableObjects;
using General;

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

            SetTreeVisuals(currentTreeStatus);
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
                Vector3 rawDirection = _lastHitterPosition - transform.position;

                rawDirection.y = 0;
                Vector3 directionToPlayer = rawDirection.normalized;

                Vector3 dropZoneCenter = transform.position + (directionToPlayer * 2.0f);

                for (int i = 0; i < treeData.numberOfLogs; i++)
                {
                    Vector2 randomScatter = Random.insideUnitCircle * treeData.scatterRadius;

                    Vector3 finalPos = new Vector3(
                        dropZoneCenter.x + randomScatter.x,
                        transform.position.y + 0.5f,
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