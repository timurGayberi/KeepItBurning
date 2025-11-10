using UnityEngine;
using System.Collections;
using Interfaces;
using PlayerScripts;

namespace GamePlay.Interactables
{

    public enum TreeStatus
    {
        Default,
        Cuted,
        UnCuted
    }

    public class ChopTarget : MonoBehaviour, ITreeTarget
    {
        [Header("")]
        [SerializeField]
        private GameObject _trunk;

        [SerializeField]
        private GameObject _log;

        [SerializeField]
        private GameObject _leaves;

        public TreeStatus currentTreeStatus = TreeStatus.Default;

        [Header("Interaction Settings (ChopTarget)")]
        [Tooltip("The text prompt shown to the player.")]
        [SerializeField]
        private string interactionPrompt = "Chop Tree";
        public string InteractionPrompt => interactionPrompt;

        [Header("Regeneration Settings")] // 🚨 NEW HEADER
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

        private void Awake()
        {
            if (currentTreeStatus == TreeStatus.Default)
            {
                currentTreeStatus = TreeStatus.UnCuted;
            }

            SetTreeVisuals(currentTreeStatus);
        }

        public void Chop(GameObject interactor, PlayerMovement playerMovement)
        {
            // Add check to ensure the tree is choppable (not already cut)
            if (currentTreeStatus != TreeStatus.UnCuted)
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
                Debug.LogError("[CHOP ERROR] Log Prefab is not assigned on ChopTarget component! Tree cannot drop resources.");
                playerMovement.SetPlayerState(PlayerState.IsIdle);
                return;
            }

            isChopping = true;

            playerMovement.SetPlayerState(PlayerState.IsInteracting);

            Debug.Log($"[CHOP START] Player ({interactor.name}) started chopping! Will take {chopDuration} seconds.");

            StartCoroutine(ChopTreeCoroutine(playerMovement));
        }

        private IEnumerator ChopTreeCoroutine(PlayerMovement playerMovement)
        {
            yield return new WaitForSeconds(chopDuration);

            // 1. Spawn Logs (Resource Output)
            Debug.Log($"[CHOP COMPLETE] Spawning {numberOfLogs} logs.");

            for (int i = 0; i < numberOfLogs; i++)
            {
                var randomCircle = Random.insideUnitCircle * scatterRadius;
                var spawnPosition = new Vector3(
                    transform.position.x + randomCircle.x,
                    transform.position.y + 0.1f, // Add slight vertical offset to avoid logs spawning inside the ground
                    transform.position.z + randomCircle.y
                );

                Instantiate(logPrefab, spawnPosition, Quaternion.identity);
            }

            // 2. Change Visual State (The requested change!)
            SetTreeVisuals(TreeStatus.Cuted);

            // 3. Reset Player State
            playerMovement.SetPlayerState(PlayerState.IsIdle);

            isChopping = false;
        }

        private void SetTreeVisuals(TreeStatus newStatus)
        {
            currentTreeStatus = newStatus;

            // This object remains, but its visuals are controlled
            bool isActive = (newStatus == TreeStatus.UnCuted);

            if (_trunk != null) _trunk.SetActive(isActive);
            if (_leaves != null) _leaves.SetActive(isActive);

            if (newStatus == TreeStatus.Cuted)
            {
                // Ensure visuals are explicitly off
                if (_trunk != null) _trunk.SetActive(false);
                if (_leaves != null) _leaves.SetActive(false);



                // **DISABLE THE CHOPTARGET COMPONENT**
                // This prevents the player's interaction system from finding this object 
                // as a valid target via the IInteractable interface.
                this.enabled = false;
                Debug.Log("[CHOP TARGET] Interaction disabled. Tree is now a stump.");

                StartCoroutine(RegrowCoroutine());
            }
            else
            {
                // Ensure component is enabled if it's UnCuted
                this.enabled = true;
                StopAllCoroutines();
            }
        }

        private IEnumerator RegrowCoroutine()
        {
            // Wait for the specified regrowth time (in real seconds, not scaled time)
            // It's safer to use WaitForSecondsRealtime if the game can be paused.
            // Assuming your game is not paused during this, WaitForSeconds is fine, 
            // but use WaitForSecondsRealtime if the game time scale can be zero elsewhere.
            yield return new WaitForSeconds(regrowthTime);

            Debug.Log("[CHOP TARGET] Tree regrowth complete! Setting state to UnCuted.");

            // Transition back to the choppable state
            SetTreeVisuals(TreeStatus.UnCuted);
        }

        public void Interact(GameObject interactor, PlayerMovement playerMovement)
        {
            Debug.Log($"[GENERIC INTERACT DEBUG] Player is near {InteractionPrompt}. The player may need the Axe to initiate the Chop action.");
            // If the player is not holding an axe, they should be idle after the debug message.
            if (playerMovement.CurrentState != PlayerState.IsInteracting)
            {
                playerMovement.SetPlayerState(PlayerState.IsIdle);
            }
        }
    }
}
