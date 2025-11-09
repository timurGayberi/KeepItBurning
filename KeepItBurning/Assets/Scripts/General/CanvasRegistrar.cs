using UnityEngine;
using System;
using System.Collections.Generic;

namespace General
{
    public class CanvasRegistrar : MonoBehaviour
    {
        public static event Action<CanvasRegistrar> OnCanvasRegistered;
        public Canvas rootCanvas { get; private set; }

        [Serializable] 
        public struct PanelEntry
        {
            public UIPanelID panelId; 
            public GameObject panelObject;
        }

        [Tooltip("Assign all child panel GameObjects and their corresponding IDs.")]
        public List<PanelEntry> registeredPanels = new List<PanelEntry>();

        public Dictionary<UIPanelID, GameObject> panelMap { get; private set; } = new Dictionary<UIPanelID, GameObject>();

        private void Awake()
        {
            //Debug.Log($"[CanvasRegistrar] AWAKE: Canvas initialized in scene '{gameObject.scene.name}'. Hiding panels at order -100.");
            rootCanvas = GetComponent<Canvas>();
            
            foreach (var entry in registeredPanels)
            {
                if (entry.panelObject != null)
                {
                    panelMap[entry.panelId] = entry.panelObject;
                    entry.panelObject.SetActive(false);
                    //Debug.Log($"[CanvasRegistrar] Hiding panel: {entry.panelId}");
                }
            }

            OnCanvasRegistered?.Invoke(this); 
            //Debug.Log($"[CanvasRegistrar] EVENT: CanvasRegistered invoked for scene '{gameObject.scene.name}'.");
        }

        private void OnDestroy()
        {
            //Debug.Log($"[CanvasRegistrar] DESTROYED: {gameObject.scene.name}. Object is being destroyed.");
        }
    }
}