using UnityEngine;
using System;
using General;
using UnityEngine.SceneManagement;

/*
 /$$$$$$  /$$                     /$$             /$$                        
 /$$__  $$|__/                    | $$            | $$                        
| $$  \__/ /$$ /$$$$$$$   /$$$$$$ | $$  /$$$$$$  /$$$$$$    /$$$$$$  /$$$$$$$ 
|  $$$$$$ | $$| $$__  $$ /$$__  $$| $$ /$$__  $$|_  $$_/   /$$__  $$| $$__  $$
 \____  $$| $$| $$  \ $$| $$  \ $$| $$| $$$$$$$$  | $$    | $$  \ $$| $$  \ $$
 /$$  \ $$| $$| $$  | $$| $$  | $$| $$| $$_____/  | $$ /$$| $$  | $$| $$  | $$
|  $$$$$$/| $$| $$  | $$|  $$$$$$$| $$|  $$$$$$$  |  $$$$/|  $$$$$$/| $$  | $$
 \______/ |__/|__/  |__/ \____  $$|__/ \_______/   \___/   \______/ |__/  |__/
                         /$$  \ $$                                            
                        |  $$$$$$/                                            
                         \______/                                             
 */

namespace Managers
{
    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager Instance {get; private set;}
        
        public GameState currentState { get; private set;}
        
        public static event Action<GameState> OnGameStateChanged;
        
        // Scenes state machine
        public enum GameState
        {
            Default, // < === The Sample scene is default

            MainMenu,
            GamePlay,
            Paused
        }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                UpdateGameState(GameState.MainMenu);
            }
            else if (currentState != GameState.MainMenu && currentState != GameState.Paused)
            {
                UpdateGameState(GameState.GamePlay);
            }
        }

        void Start()
        {
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                UpdateGameState(GameState.MainMenu);
            }
            else if (currentState != GameState.MainMenu && currentState != GameState.Paused)
            {
                UpdateGameState(GameState.Default);
            }
        }
        
        public void ForceUpdateState(GameState newState)
        {
            UpdateGameState(newState);
        }

        private void UpdateGameState(GameState newState)
        {
            if (newState == currentState) return;
            
            currentState = newState;
            
            OnGameStateChanged?.Invoke(newState);
            Debug.Log("Game State changed to: " + newState);
        }
        
        #region Scene Management

        private void LoadDefaultScene()
        {
            SceneLoader.Instance.LoadSampleScene();
            UpdateGameState(GameState.Default);
        }

        private void LoadMainMenuScene()
        {
            SceneLoader.Instance.LoadMainMenuScene();
            UpdateGameState(GameState.MainMenu);
        }

        private void LoadGamePlayScene()
        {
            SceneLoader.Instance.LoadGameScene();
            UpdateGameState(GameState.GamePlay);
        }
        
        #endregion
        
        
        #region Current Scene Controll

        public void StartGame()
        {
            LoadDefaultScene(); // <=== comment this and uncomment LoadGamePlayScene(); in initial build !!!! //
            //LoadGamePlayScene();
        }

        public void QuitToMainMenu()
        {
            Debug.Log("Button Clicked: Returning to Main Menu.");
            LoadMainMenuScene();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        #endregion
        
        
        #region Game Flow Functions
        
        public void TogglePause(bool pause)
        {
            if (pause && currentState == GameState.GamePlay)
            {
                UpdateGameState(GameState.Paused);
                Time.timeScale = 0f;
            }
            else if (!pause && currentState == GameState.Paused)
            {
                UpdateGameState(GameState.GamePlay);
                Time.timeScale = 1f;
            }
        }
        
        public void RestartLevel()
        {
            Time.timeScale = 1f;
            
            //Main game scene reload
            if (currentState == GameState.GamePlay || currentState == GameState.Paused)
            {
                SceneLoader.Instance.ReloadCurrentScene();
                UpdateGameState(GameState.GamePlay); // Force state back to GamePlay
            }
            
            //Default scene reload  
            else if (currentState == GameState.Default)
            {
                SceneLoader.Instance.ReloadCurrentScene();
                UpdateGameState(GameState.Default);
            }
        }
        
        #endregion
        
    }
}