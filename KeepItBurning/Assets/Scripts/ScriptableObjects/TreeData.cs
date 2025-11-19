using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Interactable'sStats", menuName = "GameData/TreeStats")]
    public class TreeData : ScriptableObject
    {
        [Header("Tree Health")]
        public float treesHealth;
        
        [Header("Action Timings")]
        [Tooltip("Time (in seconds) for the tree to regrow after being chopped.")]
        public float regrowthTime = 30f;
        
        [Header("Action Timings")]
        [Tooltip("If the player stops hitting the tree, how long before health resets to full?")]
        public float damageResetTime = 5.0f; // <=== Formerly chopDuration
        
        [Tooltip("The number of logs that will be spawned.")]
        public int numberOfLogs = 3;

        [Tooltip("Maximum radius logs will scatter from the tree's position.")]
        public float scatterRadius = 0.5f;

        [Header("Interaction Prompt")]
        [Tooltip("The text prompt shown to the player when the tree is uncut.")]
        public string interactionPrompt = "Chop Tree";
    }
}