using System;
using General;
using Interfaces;
using UnityEngine;
using Managers.GamePlayManagers;
using UnityEngine.UI;
using Managers.GeneralManagers;

namespace Managers.GeneralManagers
{

    public class GameStateManager : MonoBehaviour
    {
        public static GameStateManager instance {get; private set;}
        
        public GameState currentState { get; private set;}
        
        public static event Action<GameState> OnGameStateChanged;
        
        // Scenes state machine
        public enum GameState
        {
            Default,
            
            MainMenu,
            GamePlay,
            Paused,
            GameOver 
        }

        private IInputService inputService;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            try
            {
                inputService = ServiceLocator.GetService<IInputService>();
                inputService.OnPauseEvent += HandlePauseInput; 
            }
            catch (InvalidOperationException _)
            {
                //Debug.LogError("[GameStateManager] FAILED: Could not get IInputService. Check if InputReader (Execution Order -200) ran successfully. " + e.Message);
            }
        }

        void OnDestroy()
        {
            if (inputService != null)
            {
                inputService.OnPauseEvent -= HandlePauseInput;
            }
        }
        
        public void UpdateState(GameState newState)
        {
            if (currentState == newState) return;
            
            currentState = newState;
            
            OnGameStateChanged?.Invoke(newState);
            ApplyInputSettings(newState);
        }
        
        public void ForceUpdateState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged?.Invoke(currentState);
            ApplyInputSettings(newState);
        }

        private void HandlePauseInput()
        {
            // If on settings panel, just go back to main menu (stay paused)
            if (ClipboardMenuController.Instance != null && ClipboardMenuController.Instance.IsOnSettingsMenu())
            {
                ClipboardMenuController.Instance.ShowMainMenu();
                return; // Don't unpause, just return to main menu
            }

            if (currentState == GameState.GamePlay)
            {
                SoundManager.Play(SoundAction.PauseMenu);
                UpdateState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                UpdateState(GameState.GamePlay);
            }
        }

        private void ApplyInputSettings(GameState state)
        {
            if (inputService == null)
            {
                return;
            }

            switch (state)
            {
                case GameState.GamePlay:
                    Time.timeScale = 1.0f;
                    inputService.EnablePlayerInput();
                    inputService.EnableUIInput();
                    break;
                case GameState.Paused:
                    Time.timeScale = 0.0f;
                    inputService.DisablePlayerInput();
                    inputService.EnableUIInput();
                    break;
                case GameState.MainMenu:
                    Time.timeScale = 1.0f; // Unfreeze for main menu
                    inputService.DisablePlayerInput();
                    inputService.EnableUIInput();
                    break;
                case GameState.Default:
                case GameState.GameOver:
                    Time.timeScale = 0.0f;
                    inputService.DisablePlayerInput();
                    inputService.EnableUIInput();
                    break;
            }
        }
        
        public void TriggerGameOver()
        {
            if (currentState != GameState.GameOver)
            {
                Debug.Log("[GAME STATE] Game Over triggered! Restarting level.");
                

                UpdateState(GameState.GameOver);
                
            }
        }
        
        #region Flow Control (Scene and Game Management)

        public void StartGame()
        {
            SceneLoader.Instance.LoadGameScene();
        }

        public void QuitToMainMenu()
        {
            if (PlayGameManager.Instance != null)
            {
                PlayGameManager.Instance.ResetAllStats();
            }

            SceneLoader.Instance.LoadMainMenuScene();
        }
        
        public void ResumeGameFromUI()
        {
            if (currentState == GameState.Paused )
            {
                UpdateState(GameState.GamePlay);
                Debug.Log("Resume game pressed");
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        
        public void OnPauseAction(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            if (currentState == GameState.GamePlay)
            {
                UpdateState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                UpdateState(GameState.GamePlay);
            }
        }
        
        public void RestartLevel()
        {
            if (Managers.GamePlayManagers.PlayGameManager.Instance != null)
            {
                Managers.GamePlayManagers.PlayGameManager.Instance.ResetAllStats();
            }

            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.ReloadCurrentScene();
            }
        }

        #endregion
    }
}