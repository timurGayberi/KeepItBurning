using UnityEngine;
using GamePlay.Interactables; 

namespace Managers
{
    public class TentManager : MonoBehaviour
    {
        [Header("Tent References")]
        [Tooltip("Drag all Tent components that need global management into this array.")]
        [SerializeField] 
        private Tent[] allManagedTents;
        
        public void SetAllTentsState(TentState newState)
        {
            if (allManagedTents == null || allManagedTents.Length == 0)
            {
                Debug.LogWarning("TentManager has no Tent references assigned.");
                return;
            }

            foreach (Tent tent in allManagedTents)
            {
                if (tent != null)
                {
                    tent.SetState(newState);
                }
            }
            Debug.Log($"TentManager successfully set all {allManagedTents.Length} tents to state: {newState}.");
        }

        private void Awake()
        {
             if (allManagedTents == null || allManagedTents.Length == 0)
             {
                 allManagedTents = GetComponentsInChildren<Tent>();
                 Debug.Log($"TentManager automatically found {allManagedTents.Length} Tents in children.");
             }
        }
    }
}