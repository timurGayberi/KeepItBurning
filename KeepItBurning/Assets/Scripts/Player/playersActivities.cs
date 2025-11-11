using UnityEngine;

namespace Player
{
    public class PlayersActivities : MonoBehaviour
    {
        public PlayerState currentState { get; private set; } = PlayerState.IsIdle;
        public void SetPlayerState(PlayerState newState)
        {
            currentState = newState;
            Debug.Log($"[PlayerState] Changed to: {newState}");
        }
    }
}
