using System.Collections.Generic;
using General;
using UnityEngine;
using UnityEngine.SceneManagement;
using Managers.GeneralManagers; 

namespace Managers.GlobalManagers 
{
    public class GameUiManager : MonoBehaviour
    {
        private readonly Dictionary<string, CanvasRegistrar> activeCanvasRegistrars = new Dictionary<string, CanvasRegistrar>();

        private void Awake()
        {
            CanvasRegistrar.OnCanvasRegistered += OnCanvasRegistered;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void Start()
        {
            //Debug.Log("[UIManager] START: Subscribing to GameState.OnGameStateChanged.");
            
            if (GameStateManager.instance != null)
            {
                GameStateManager.OnGameStateChanged += OnGameStateChanged;
                // Force an initial update based on the current state
                OnGameStateChanged(GameStateManager.instance.currentState);
            }
        }

        private void OnDestroy()
        {
            CanvasRegistrar.OnCanvasRegistered -= OnCanvasRegistered;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            
            if (GameStateManager.instance != null) // FIX: Changed to Instance (PascalCase)
            {
                GameStateManager.OnGameStateChanged -= OnGameStateChanged;
            }
        }
        
        private void OnSceneUnloaded(Scene scene)
        {
            var sceneName = scene.name;
            if (activeCanvasRegistrars.ContainsKey(sceneName))
            {
                activeCanvasRegistrars.Remove(sceneName);
                //Debug.Log($"[UIManager] CLEANUP: Removed canvas from UNLOADED scene: {sceneName}");
            }
        }

        private void OnCanvasRegistered(CanvasRegistrar registrar)
        {
            var sceneName = registrar.gameObject.scene.name;
            activeCanvasRegistrars[sceneName] = registrar;
            //Debug.Log($"[UIManager] REGISTERED: Canvas for scene '{sceneName}' added to active list.");
            
            if (GameStateManager.instance != null) // FIX: Changed to Instance (PascalCase)
            {
                ApplyStateToUI(GameStateManager.instance.currentState, registrar);
            }
            else
            {
                //Debug.LogWarning("[UIManager] WARNING: Canvas registered before GameStateManager was created.");
            }
        }

        private void OnGameStateChanged(GameStateManager.GameState newState)
        {
            //Debug.Log($"[UIManager] EVENT: Received GameState change to {newState}. Updating ALL active canvases.");

            foreach (var registrar in activeCanvasRegistrars.Values)
            {
                ApplyStateToUI(newState, registrar); 
            }
        }
        
        private void ApplyStateToUI(GameStateManager.GameState state, General.CanvasRegistrar registrar)
        {
            if (registrar == null)
            {
                //Debug.LogWarning("[UIManager] WARNING: Attempted to apply state to a destroyed CanvasRegistrar. Skipping.");
                return;
            }

            // SceneLoader.Instance check is still necessary to know which scene is the current game scene.
            if (SceneLoader.Instance == null)
            {
                //Debug.LogError("[UIManager] ERROR: SceneLoader.Instance is NULL. Cannot determine scene names.");
                return;
            }

            var currentSceneName = registrar.gameObject.scene.name;
            var isMainMenuScene = currentSceneName == SceneLoader.Instance.MainMenuScene;
            var isGameplayScene = currentSceneName == SceneLoader.Instance.GameplaySceneName || currentSceneName == SceneLoader.Instance.SampleScene;
            
            //Debug.Log($"[UIManager] APPLYING STATE: {state} to Canvas in scene: {currentSceneName}. (IsMenu: {isMainMenuScene}, IsGame: {isGameplayScene})");

            foreach (var entry in registrar.panelMap) 
            {
                var panelId = entry.Key;
                var panelObject = entry.Value;
                var shouldBeActive = false;

                switch (state)
                {
                    case GameStateManager.GameState.MainMenu:
                        // Only show MainMenu panel if player in the Main Menu scene.
                        if (isMainMenuScene && panelId == UIPanelID.MainMenu)
                        {
                            shouldBeActive = true;
                        }
                        break;

                    case GameStateManager.GameState.GamePlay:
                        // Only show HUD if player in a Gameplay scene, and hide the pause panel.
                        if (isGameplayScene && panelId == UIPanelID.GameplayHUD)
                        {
                            shouldBeActive = true;
                        }
                        break;

                    case GameStateManager.GameState.Paused:
                        // Show both HUD and Pause Menu when paused.
                        if (isGameplayScene && (panelId == UIPanelID.GameplayHUD || panelId == UIPanelID.GameplayPause))
                        {
                            shouldBeActive = true;
                        }
                        break;
                    
                    case GameStateManager.GameState.Default:
                        // No panels should be active in the default/loading state.
                        shouldBeActive = false;
                        break;
                }
                
                if (panelObject != null && panelObject.activeSelf != shouldBeActive)
                {
                     panelObject.SetActive(shouldBeActive);
                     // Debug.Log($"[UIManager] -> PANEL VISIBILITY: Panel {panelId} set to: {shouldBeActive} in scene {currentSceneName}.");
                }
            }
        }
    }
}