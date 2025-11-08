using UnityEngine;

namespace Managers
{
    public static class ManagerBootstrapper 
    {
        private const string ManagersPrefabPath = "Managers";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute()
        {
            var managersPrefab = Resources.Load(ManagersPrefabPath);
        
            if (managersPrefab == null)
            {
                Debug.LogError($"[Bootstrapper] Failed to load Managers Prefab from Resources/{ManagersPrefabPath}.prefab. " +
                               "Please ensure the Managers GameObject is a Prefab inside a folder named 'Resources'.");
                return;
            }
            
            GameObject managersInstance = Object.Instantiate(managersPrefab) as GameObject;

            if (managersInstance != null)
            {
                managersInstance.name = "Managers (DDOL)";
                Object.DontDestroyOnLoad(managersInstance);
                Debug.Log("[Bootstrapper] Managers loaded and persistent.");
            }
        }
    }
}
