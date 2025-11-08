using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using General;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace Managers
{
    public class GameUiManager : MonoBehaviour
    {
        // Now stores the reliable, inspector-assigned references from the CanvasRegistrar
        private Dictionary<UIPanelID, GameObject> currentPanelMap = new Dictionary<UIPanelID, GameObject>();
        
        private static bool isInitialized = false;
        
        #region Listener
        
        private void OnEnable()
        {
            GameStateManager.OnGameStateChanged += HandleGameStateChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            CanvasRegistrar.OnCanvasRegistered += RegisterCanvas;
            
            if (!isInitialized && GameStateManager.Instance != null)
            {
                StartCoroutine(InitialUISetup());
            }
        }

        private void OnDisable()
        {
            GameStateManager.OnGameStateChanged -= HandleGameStateChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            CanvasRegistrar.OnCanvasRegistered -= RegisterCanvas;
        }
        
        private void RegisterCanvas(CanvasRegistrar registrar)
        {
            currentPanelMap = registrar.PanelMap;

            Debug.Log($"[UI Manager] Canvas Registered. Total Panels: {currentPanelMap.Count}.");
    
            // THIS is the line that should fix it, but let's make sure it's reliable.
            if (GameStateManager.Instance != null) 
            {
                HandleGameStateChanged(GameStateManager.Instance.currentState);
            }
        }
        
        private IEnumerator InitialUISetup()
        {
            yield return null; 
            
            if (GameStateManager.Instance != null)
            {
                HandleGameStateChanged(GameStateManager.Instance.currentState);
            }
            
            isInitialized = true;
        }
        
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            currentPanelMap.Clear(); 
            
            if(GameStateManager.Instance != null)
            {
                HandleGameStateChanged(GameStateManager.Instance.currentState);
            }
        }
        
        #endregion
        

        #region State's handlers
        private void HandleGameStateChanged(GameStateManager.GameState newState)
        {
            if (currentPanelMap.Count == 0)
            {
                return; 
            }
            
            foreach (var panel in currentPanelMap.Values)
            {
                panel.SetActive(false);
            }
            
            GameObject panelToShow = null;
            
            bool activatePausePanel = false;
            UIPanelID targetPanelID = UIPanelID.None;
            
            switch (newState)
            {
                case GameStateManager.GameState.MainMenu:
                    targetPanelID = UIPanelID.MainMenu;
                    break;
                
                case GameStateManager.GameState.GamePlay:
                    targetPanelID = UIPanelID.GameplayHUD;
                    break;
                
                case GameStateManager.GameState.Default:
                    targetPanelID = UIPanelID.GameplayHUD;
                    break;
                
                case GameStateManager.GameState.Paused:
                    targetPanelID = UIPanelID.GameplayPause;
                    activatePausePanel = true;
                    break;
                
                
                default:
                    Debug.LogError($"[UI Manager] Invalid game state: {newState}");
                    break;
            }
            
            if (targetPanelID != UIPanelID.None)
            {
                if (currentPanelMap.TryGetValue(targetPanelID, out panelToShow))
                {
                    panelToShow.SetActive(true);
                }
            }
            
            if (activatePausePanel)
            {
                if (currentPanelMap.TryGetValue(UIPanelID.GameplayPause, out var pausePanel))
                {
                    pausePanel.SetActive(true);
                }
            }
        }
        #endregion
    }
}