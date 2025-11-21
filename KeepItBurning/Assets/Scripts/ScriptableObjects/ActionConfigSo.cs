using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "NewActionConfig", menuName = "GameData/Action Config")]
    public class ActionConfigSo : ScriptableObject
    {
        [Header("Timing Synchronization")]
        [Tooltip("How long (in seconds) from button press until the axe hits the tree.")]
        public float impactDelay = 0.4f;

        [Tooltip("The total time (in seconds) before the player can chop again.")]
        public float totalDuration = 1.0f;

        [Header("Audio")]
        [Tooltip("The sound to play at the moment of impact.")]
        public SoundAction impactSound = SoundAction.ChopWood;
        
        public float Cooldown => Mathf.Max(totalDuration, impactDelay + 0.1f);
    }
}