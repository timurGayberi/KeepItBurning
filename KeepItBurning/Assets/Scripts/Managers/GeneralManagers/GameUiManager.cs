using System;
using System.Collections.Generic;
using General;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers.GeneralManagers 
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
            if (GameStateManager.instance != null)
            {
                GameStateManager.OnGameStateChanged += OnGameStateChanged;
                OnGameStateChanged(GameStateManager.instance.currentState);
            }
        }

        private void OnDestroy()
        {
            CanvasRegistrar.OnCanvasRegistered -= OnCanvasRegistered;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            
            if (GameStateManager.instance != null) 
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
            }
        }

        private void OnCanvasRegistered(CanvasRegistrar registrar)
        {
            var sceneName = registrar.gameObject.scene.name;
            activeCanvasRegistrars[sceneName] = registrar;
            
            if (GameStateManager.instance != null) 
            {
                ApplyStateToUI(GameStateManager.instance.currentState, registrar);
            }
        }

        private void OnGameStateChanged(GameStateManager.GameState newState)
        {

            foreach (var registrar in activeCanvasRegistrars.Values)
            {
                ApplyStateToUI(newState, registrar); 
            }
        }
        
        private void ApplyStateToUI(GameStateManager.GameState state, General.CanvasRegistrar registrar)
        {
            if (registrar == null)
            {
                return;
            }
            
            if (SceneLoader.Instance == null)
            {
                return;
            }

            var currentSceneName = registrar.gameObject.scene.name;
            var isMainMenuScene = currentSceneName == SceneLoader.Instance.MainMenuScene;
            var isGameplayScene = currentSceneName == SceneLoader.Instance.GameplaySceneName || currentSceneName == SceneLoader.Instance.SampleScene;

            foreach (var entry in registrar.panelMap) 
            {
                var panelId = entry.Key;
                var panelObject = entry.Value;
                var shouldBeActive = false;

                switch (state)
                {
                    case GameStateManager.GameState.MainMenu:
                        if (isMainMenuScene && panelId == UIPanelID.MainMenu)
                        {
                            shouldBeActive = true;
                        }
                        break;

                    case GameStateManager.GameState.GamePlay:
                        if (isGameplayScene && panelId == UIPanelID.GameplayHUD)
                        {
                            shouldBeActive = true;
                        }
                        break;

                    case GameStateManager.GameState.Paused:
                        // FIX: Only show the pause panel, ensure HUD is inactive
                        if (isGameplayScene && panelId == UIPanelID.GameplayPause)
                        {
                            shouldBeActive = true;
                        }
                        break;
                    
                    case GameStateManager.GameState.Default:
                        shouldBeActive = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
                
                if (panelObject != null && panelObject.activeSelf != shouldBeActive)
                {
                    // Check if panel has a PanelAnimator component
                    var panelAnimator = panelObject.GetComponent<General.PanelAnimator>();

                    if (panelAnimator != null)
                    {
                        // Use animated transition
                        if (shouldBeActive)
                        {
                            panelAnimator.Show();
                        }
                        else
                        {
                            panelAnimator.Hide();
                        }
                    }
                    else
                    {
                        // Fallback to instant show/hide
                        panelObject.SetActive(shouldBeActive);
                    }
                }
            }
        }
    }
}