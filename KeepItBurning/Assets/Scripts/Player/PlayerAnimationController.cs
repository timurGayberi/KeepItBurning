using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimatorController : MonoBehaviour
    {
        private Animator animator;
        private PlayerMovement playerMovement;
        
        private readonly int stateHash = Animator.StringToHash("State"); 

        private void Awake()
        {
            animator = GetComponent<Animator>();
            playerMovement = GetComponent<PlayerMovement>();
            
            if (animator == null)
            {
                Debug.LogError("Animator component not found on PlayerAnimatorController GameObject.");
            }
            if (playerMovement == null)
            {
                Debug.LogError("PlayerMovement component not found. Cannot subscribe to state changes.");
            }
        }

        private void OnEnable()
        {
            if (playerMovement != null)
            {
                playerMovement.OnPlayerStateChange += SetAnimatorState;
                SetAnimatorState(playerMovement.CurrentState); 
            }
        }

        private void OnDisable()
        {
            if (playerMovement != null)
            {
                playerMovement.OnPlayerStateChange -= SetAnimatorState;
            }
        }
        
        private void SetAnimatorState(PlayerState newState)
        {
            animator.SetInteger(stateHash, (int)newState);
            Debug.Log($"[Animator] Setting state to: {(int)newState} ({newState})");
        }
    }
}