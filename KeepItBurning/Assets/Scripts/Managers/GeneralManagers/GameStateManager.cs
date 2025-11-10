using System;
using General;
using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

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
            Paused
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
            
            //Debug.Log("[GameStateManager] STATE: Changed to: " + newState); 
            OnGameStateChanged?.Invoke(newState);
            ApplyInputSettings(newState);
        }
        
        public void ForceUpdateState(GameState newState)
        {
            currentState = newState;
            //Debug.Log("[GameStateManager] STATE: Forced update to: " + newState);
            OnGameStateChanged?.Invoke(currentState);
            ApplyInputSettings(newState);
        }

        private void HandlePauseInput()
        {
            if (currentState == GameState.GamePlay)
            {
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
                //Debug.LogWarning($"[GameStateManager] WARNING: Input settings for state {state} not applied, _inputService is NULL.");
                return;
            }

            switch (state)
            {
                case GameState.GamePlay:
                    Time.timeScale = 1.0f;
                    inputService.EnablePlayerInput();
                    inputService.DisableUIInput();
                    break;
                case GameState.Paused:
                    Time.timeScale = 0.0f; 
                    inputService.DisablePlayerInput();
                    inputService.EnableUIInput();
                    break;
                case GameState.MainMenu:
                case GameState.Default:
                    Time.timeScale = 1.0f;
                    inputService.DisablePlayerInput();
                    inputService.EnableUIInput();
                    break;
            }
        }
        
        
        #region Flow Control (Scene and Game Management)

        public void StartGame()
        {
            SceneLoader.Instance.LoadSampleScene(); 
        }

        public void QuitToMainMenu()
        {
            SceneLoader.Instance.LoadMainMenuScene();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
        
        /*public void TogglePause(bool pause)
        {
            if (pause && currentState == GameState.GamePlay)
            {
                UpdateState(GameState.Paused);
            }
            else if (!pause && currentState == GameState.Paused)
            {
                UpdateState(GameState.GamePlay);
            }
        }*/
        
        public void OnPauseAction(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            if (currentState == GameState.GamePlay)
            {
                UpdateState(GameState.Paused);
                Debug.Log("Game Pause");
            }
            else if (currentState == GameState.Paused)
            {
                UpdateState(GameState.GamePlay);
                Debug.Log("Game UnPause");
            }
        }
        
        public void RestartLevel()
        {
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.ReloadCurrentScene();
            }
        }
        
        #endregion
    }
}