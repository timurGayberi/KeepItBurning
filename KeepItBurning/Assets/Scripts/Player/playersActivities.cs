using UnityEngine;

namespace Player
{
    public class PlayersActivities : MonoBehaviour
    {
        public PlayerState currentState { get; private set; } = PlayerState.IsIdle;
        
        private PlayerAnimatorController _animatorController;

        private void Awake()
        {
            _animatorController = GetComponent<PlayerAnimatorController>();
            if (_animatorController == null)
            {
                Debug.LogError("PlayerAnimatorController component not found on PlayersActivities GameObject. Make sure it's attached.");
            }
        }
        
        public void SetPlayerState(PlayerState newState)
        {
            if (currentState == newState) return;
            
            currentState = newState;
            Debug.Log($"[PlayerState] Changed to: {newState}");
            
            if (_animatorController != null)
            {
                _animatorController.SetAnimatorState(newState); 
            }
        }
    }
}