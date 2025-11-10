using UnityEngine;

namespace Managers.GeneralManagers
{
    public static class ManagerBootstrapper
    {
        private const string ManagersPrefabPath = "Managers"; 

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Execute()
        {
            var managersPrefab = Resources.Load<GameObject>(ManagersPrefabPath);

            if (managersPrefab == null)
            {
                //Debug.LogError($"[Bootstrapper] FAILED: Manager prefab could not be loaded from 'Resources/{ManagersPrefabPath}'. Check folder structure and file name.");
                return;
            }
            
            var managersInstance = GameObject.Instantiate(managersPrefab);
            
            Object.DontDestroyOnLoad(managersInstance);
            
            //Debug.Log("[Bootstrapper] SUCCESS: Core Managers loaded and marked DontDestroyOnLoad.");
        }
    }
}