using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "GameData/PlayerStats")]
    public class PlayerStatsSo : ScriptableObject
    {
        [Header("PlayerStats")]
        public float movementSpeed;
        public float sprintSpeed;
        
        [Header("Number of logs player  can carry")]
        [Tooltip("The number of logs player can carry at once")]
        public int maxNumberOfLogsCarry;
    }
}