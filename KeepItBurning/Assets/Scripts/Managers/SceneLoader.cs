using UnityEngine;
using UnityEngine.SceneManagement;
using Managers; 

namespace Managers
{
    [DefaultExecutionOrder(-100)] 
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
            
            // CRITICAL: Subscribe to the static scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded; // <--- This now correctly uses the Unity class
        }

        void OnDestroy()
        {
            // CRITICAL: Unsubscribe to prevent memory leaks
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        // This method is called automatically AFTER a scene loads
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[SceneLoader] Scene loaded: {scene.name}");
            
            // Determine the GameState based on the loaded scene name
            GameStateManager.GameState newState = GameStateManager.GameState.Default;

            if (scene.name == MainMenuScene)
            {
                newState = GameStateManager.GameState.MainMenu;
            }
            else if (scene.name == GameplaySceneName || scene.name == SampleScene)
            {
                // Assuming GameScene or SampleScene are used for actual gameplay
                newState = GameStateManager.GameState.GamePlay;
            }
            
            // Force the GameStateManager to update its state, which triggers the GameUiManager
            GameStateManager.Instance.ForceUpdateState(newState);
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

        /// <summary>
        /// Reloads the currently active scene. Used for restarting the game/level.
        /// </summary>
        public void ReloadCurrentScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}