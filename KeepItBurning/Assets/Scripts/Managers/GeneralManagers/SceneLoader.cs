using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers.GeneralManagers
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        // EXPOSED IN INSPECTOR: Names of scenes to load
        [Tooltip("The actual name of the Sample Scene file (e.g., SampleScene)")]
        public string SampleScene;

        [Tooltip("The actual name of the Main Menu Scene file (e.g., MainMenu)")]
        public string MainMenuScene;

        [Tooltip("The actual name of the Gameplay Scene file (e.g., GameScene)")]
        public string GameplaySceneName;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[SceneLoader] Scene loaded: {scene.name}");
            
            
            var newState = GameStateManager.GameState.Default;

            if (scene.name == MainMenuScene)
            {
                newState = GameStateManager.GameState.MainMenu;
            }
            else if (scene.name == GameplaySceneName || scene.name == SampleScene)
            {
                newState = GameStateManager.GameState.GamePlay;
            }
            
            GameStateManager.instance.ForceUpdateState(newState);
        }
        
        public void LoadSampleScene()
        {
            SceneManager.LoadScene(SampleScene);
        }

        public void LoadMainMenuScene()
        {
            SceneManager.LoadScene(MainMenuScene);
        }

        public void LoadGameScene()
        {
            SceneManager.LoadScene(GameplaySceneName);
        }
        
        public void ReloadCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}