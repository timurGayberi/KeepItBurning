using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using PlayerScripts;
using GamePlay.Interactables;
using UnityEngine.SceneManagement;

namespace General
{
    public enum GameState
    {
        InGame,
        OnPause,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action<GameState> OnGameStateChange;
        public GameState CurrentGameState { get; private set; } = GameState.InGame;

        private IInputService _inputService;
        private PlayerMovement _playerMovement;
        private Tent _tent;
        private IEnumerator _stateChangeCoroutine;
        private IEnumerator _setupCoroutine;

        private bool _isStateTransitioning = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            SceneManager.sceneLoaded += OnSceneLoaded;

            FireplaceInteraction.OnFireplaceOut += HandleFireplaceOut;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;

            FireplaceInteraction.OnFireplaceOut -= HandleFireplaceOut;

            if (_inputService != null)
            {
                _inputService.OnPauseEvent -= TogglePause;
            }
        }

        #region GameObjects

        #region Fireplace

        private void HandleFireplaceOut()
        {
            Debug.Log("[GM] Fireplace ran out of fuel! Initiating Game Over.");
            SetGameState(GameState.GameOver);
        }

        #endregion

        #region Tents
        public void SetTentState(TentState newState)
        {
            if (_tent != null)
            {
                _tent.SetState(newState);
                Debug.Log($"GameManager set Tent state to: {newState}");
            }
            else
            {
                Debug.LogError("Attempted to set Tent state, but Tent object reference is null. Ensure Tent is in the scene.");
            }
        }
        #endregion

        private void InitializeSceneObjects()
        {
            _tent = null;
            _tent = FindObjectOfType<Tent>();

            if (_tent != null)
            {
                SetTentState(TentState.Normal);
                Debug.Log($"Tent found and initial state set: {_tent.gameObject.name}.");
            }
            else
            {
                Debug.LogWarning("Tent object not found in the scene. Tent control is disabled until found.");
            }
        }

        #endregion

        private void Start()
        {
            _setupCoroutine = SceneSetupRoutine();
            StartCoroutine(_setupCoroutine);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_setupCoroutine != null)
                StopCoroutine(_setupCoroutine);

            _setupCoroutine = SceneSetupRoutine();
            StartCoroutine(_setupCoroutine);
            StartCoroutine(ReconnectUIRoutine());
        }

        private IEnumerator ReconnectUIRoutine()
        {
            yield return null;

            var uiManager = FindObjectOfType<UI.UIManager>();
            if (uiManager != null)
            {
                uiManager.LinkButtonsToGameManager();
                Debug.Log("GameManager: Re-linking UI buttons after scene load...");
                var linkMethod = uiManager.GetType().GetMethod("LinkButtonsToGameManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                linkMethod?.Invoke(uiManager, null);
            }
            else
            {
                Debug.LogWarning("GameManager: No UIManager found in new scene after reload.");
            }
        }

        private IEnumerator SceneSetupRoutine()
        {
            yield return null;
            GetServices();
            InitializeSceneObjects();
            SetGameState(GameState.InGame);
        }

        private void GetServices()
        {
            if (_inputService != null)
            {
                _inputService.OnPauseEvent -= TogglePause;
            }

            try
            {
                _inputService = ServiceLocator.GetService<IInputService>();
                _inputService.OnPauseEvent += TogglePause;
                Debug.Log("IInputService and Pause Event subscribed successfully.");
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError("IInputService not found AFTER scene load. Is InputReader registered in Awake? Error: " + e.Message);
            }

            _playerMovement = FindObjectOfType<PlayerMovement>();
        }



        public void TogglePause()
        {
            if (CurrentGameState == GameState.OnPause)
            {
                SetGameState(GameState.InGame); // Unpause
            }
            else if (CurrentGameState == GameState.InGame)
            {
                SetGameState(GameState.OnPause); // Pause
            }
        }

        private void SetGameState(GameState newState)
        {
            if (CurrentGameState == newState) return;
            CurrentGameState = newState;

            if (_stateChangeCoroutine != null)
            {
                StopCoroutine(_stateChangeCoroutine);
            }

            _stateChangeCoroutine = ProcessStateChange(newState);
            StartCoroutine(_stateChangeCoroutine);

            OnGameStateChange?.Invoke(CurrentGameState);
        }

        private IEnumerator ProcessStateChange(GameState newState)
        {
            if (_isStateTransitioning) yield break;
            _isStateTransitioning = true;

            switch (newState)
            {
                case GameState.InGame:
                    Time.timeScale = 1f;
                    if (_inputService != null)
                    {
                        _inputService.EnablePlayerInput();
                    }
                    Debug.Log("Game State: InGame. Time Scale set to 1.");
                    break;

                case GameState.OnPause:
                    Time.timeScale = 0f;
                    if (_inputService != null)
                    {
                        _inputService.DisablePlayerInput();
                    }
                    Debug.Log("Game State: OnPause. Time Scale set to 0.");
                    break;

                case GameState.GameOver:
                    Time.timeScale = 0f;
                    if (_inputService != null)
                    {
                        _inputService.DisablePlayerInput();
                    }

                    yield return new WaitForSecondsRealtime(2f);
                    RestartLevel();
                    break;
            }

            _isStateTransitioning = false;
        }

        public void RestartLevel()
        {
            Debug.Log("--- Level Restart Initiated ---");

            //=======Change It Lataer!========//

            FireplaceInteraction fireplace = FindObjectOfType<FireplaceInteraction>();
            if (fireplace != null)
            {
                fireplace.enabled = true;
            }

            //===============================//

            if (_tent != null)
            {
                _tent.SetState(TentState.Normal);
            }

            StopAllCoroutines();

            _playerMovement = null;
            _tent = null;

            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log($"Level '{SceneManager.GetActiveScene().name}' reloaded.");
        }

        public void OpenOptions()
        {
            Debug.Log("Options menu requested. (Requires UI_Manager to handle panel swap)");
        }

        public void QuitGame()
        {
            Time.timeScale = 1f;
            Debug.Log("Quit Game initiated.");
            SceneManager.LoadScene("MainMenuAlpha", LoadSceneMode.Single);
            Application.Quit();

        }
    }
}