using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance {get; private set;}
        
        [Header("Scenes to load")]
        [SerializeField] private string sampleScene = "SampleScene"; // <= Default scene
        
        [SerializeField] private string mainMenuScene =  "MainMenu"; 
        [SerializeField] private string gameplaySceneName = "GameScene";
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                // CRITICAL: Subscribe to the scene loaded event
                SceneManager.sceneLoaded += OnSceneLoaded;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void OnDestroy()
        {
            // CRITICAL: Unsubscribe when destroyed
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (GameStateManager.Instance == null) return;
            
            GameStateManager.GameState newState;
            
            if (scene.name == mainMenuScene)
            {
                newState = GameStateManager.GameState.MainMenu;
            }
            else if (scene.name == gameplaySceneName || scene.name == sampleScene)
            {
                newState = GameStateManager.GameState.GamePlay;
            }
            else
            {
                // Fallback for unexpected scenes
                return;
            }
            
            GameStateManager.Instance.ForceUpdateState(newState);
        }
        
        #region Public Functions

        public void LoadMainMenuScene()
        {
            SceneManager.LoadScene(mainMenuScene);
        }

        public void LoadGameScene()
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
        
        public void LoadSampleScene()
        {
            SceneManager.LoadScene(sampleScene);
        }

        public void ReloadCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        
        #endregion
    }
}