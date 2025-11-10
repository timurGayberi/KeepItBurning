using UnityEngine;
using General;
using System;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {

        [Tooltip("The root GameObject for the Pause Menu UI.")]
        [SerializeField] private GameObject pauseMenuPanel;


        [Tooltip("Pause Menu Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;

        private GameManager _gameManager;

        private IInputService _inputService;
        private ControlDevice _currentDevice = ControlDevice.Unknown;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            SetupGameManagerLinks();
        }

        private void Start()
        {
            SetupInputService();
        }

        private void OnEnable()
        {
            // Always ensure a clean state on UI creation
            LinkButtonsToGameManager();
        }

        private void OnDisable()
        {
            if (resumeButton != null)
                resumeButton.onClick.RemoveAllListeners();
            if (restartButton != null)
                restartButton.onClick.RemoveAllListeners();
        }

        private void SetupGameManagerLinks()
        {
            try
            {
                _gameManager = GameManager.Instance;

                if (_gameManager == null)
                {
                    Debug.LogError("UIManager failed to find GameManager Instance. Cannot subscribe.");
                    return;
                }

                LinkButtonsToGameManager();

                _gameManager.OnGameStateChange += HandleGameStateChange;

                HandleGameStateChange(_gameManager.CurrentGameState);

            }
            catch (InvalidOperationException e)
            {
                Debug.LogError($"UIManager Setup failed: {e.Message}");
            }
        }

        private void SetupInputService()
        {
            try
            {
                _inputService = ServiceLocator.GetService<IInputService>();
                _inputService.OnControlSchemeChange += HandleControlSchemeChange;

                HandleControlSchemeChange(_inputService.CurrentControlDevice);

            }
            catch (InvalidOperationException e)
            {
                Debug.LogError($"UIManager InputService lookup failed: {e.Message}. Control scheme visualization disabled.");
            }
        }


        public void LinkButtonsToGameManager()
        {
            // 1. Ensure GameManager is available (Good initial check)
            if (GameManager.Instance == null)
            {
                return;
            }

            // 2. 🚨 CRITICAL FIX: Ensure the UI button references are NOT null.
            // This prevents the NRE if the button GameObjects were destroyed on scene load.
            if (resumeButton == null || restartButton == null)
            {
                // If the buttons are null, the UI hasn't loaded yet or the references were lost.
                // Log a warning and exit cleanly.
                Debug.LogWarning("UIManager: Button references (resumeButton/restartButton) are missing/null. Cannot link listeners.");
                return;
            }

            GameManager gm = _gameManager != null ? _gameManager : GameManager.Instance;

            resumeButton.onClick.RemoveAllListeners();
            restartButton.onClick.RemoveAllListeners();

            resumeButton.onClick.AddListener(gm.TogglePause);
            restartButton.onClick.AddListener(gm.RestartLevel);

            Debug.Log("UIManager: Buttons successfully linked to GameManager.");
        }

        private void OnDestroy()
        {
            if (_gameManager != null)
            {
                _gameManager.OnGameStateChange -= HandleGameStateChange;
            }
            if (_inputService != null)
            {
                _inputService.OnControlSchemeChange -= HandleControlSchemeChange;
            }
        }



        private void HandleControlSchemeChange(ControlDevice newDevice)
        {
            _currentDevice = newDevice;
            Debug.Log($"UI Manager: Current input device set to {_currentDevice}");
            // TODO: Add logic here to trigger UI elements (like button icons)
        }

        private void HandleGameStateChange(GameState newState)
        {
            if (pauseMenuPanel == null)
            {
                Debug.LogWarning("Pause Menu Panel reference is missing in UIManager.");
                return;
            }

            if (newState == GameState.OnPause)
            {
                pauseMenuPanel.SetActive(true);
                Debug.Log("UI Manager: Pause Menu Opened.");

                if (resumeButton != null)
                {
                    resumeButton.interactable = true;
                    Debug.Log("Resume Button forced interactable.");
                }
            }
            else
            {
                pauseMenuPanel.SetActive(false);
                Debug.Log("UI Manager: Pause Menu Closed.");
            }
        }
    }
}
