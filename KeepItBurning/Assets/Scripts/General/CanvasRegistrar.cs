using UnityEngine;
using System;
using System.Collections.Generic; 

namespace General
{
    [Serializable] 
    public struct PanelEntry
    {
        public UIPanelID panelId;
        public GameObject panelObject;
    }

    public class CanvasRegistrar : MonoBehaviour
    {
        public static event Action<CanvasRegistrar> OnCanvasRegistered;
        public Canvas RootCanvas { get; private set; }
        
        [Tooltip("Assign all child panel GameObjects and their corresponding IDs.")]
        public List<PanelEntry> registeredPanels = new List<PanelEntry>();
        
        public Dictionary<UIPanelID, GameObject> PanelMap { get; private set; } = new Dictionary<UIPanelID, GameObject>();

        void Awake()
        {
            RootCanvas = GetComponent<Canvas>();
            
            foreach (var entry in registeredPanels)
            {
                if (entry.panelObject != null)
                {
                    PanelMap[entry.panelId] = entry.panelObject;
                    entry.panelObject.SetActive(false);
                }
            }
            
            OnCanvasRegistered?.Invoke(this); 
        }
    }
}