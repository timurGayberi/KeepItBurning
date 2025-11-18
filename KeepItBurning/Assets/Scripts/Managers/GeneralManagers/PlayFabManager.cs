using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using General;
using Interfaces;

namespace Managers.GeneralManagers
{
    public class PlayFabManager : MonoBehaviour, IPlayFabService 
    {
        
        #region References

        private const string    PlayFabTitleID =        "1ECE61",
                                PlayerScore =           "Score",
                                PlayerPlatformScore =   "Score",
                                PlayerNickName =        "Nickname";

        #endregion
        
        private bool _loginSuccess;

        private void Awake()
        {
            ServiceLocator.RegisterService<IPlayFabService>(this); 
        }
        
        private void OnDestroy()
        {
            ServiceLocator.UnregisterService<IPlayFabService>(this);
        }

        private void Start()
        {
            PlayFabSettings.staticSettings.TitleId = PlayFabTitleID;
            
            Login();
        }
        
        #region Score setup
        
        public void SaveNickname(string nickname)
        {
            if (!_loginSuccess)
            {
                Debug.LogWarning("Nickname save blocked: Not logged into PlayFab.");
                return;
            }

            var displayRequest = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = nickname 
            };
            
            PlayFabClientAPI.UpdateUserTitleDisplayName(displayRequest, OnDisplayNameUpdateSuccess, OnError);
        }
        
        private static void OnDisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result)
        {
            Debug.Log($"Player Display Name updated to: {result.DisplayName}");
        }
        
        public void SubmitScore(int score)
        {
            if (!_loginSuccess)
            {
                Debug.LogWarning("Score submission blocked: Not logged into PlayFab.");
                return;
            }
            
            var request = new UpdatePlayerStatisticsRequest()
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate { StatisticName = PlayerScore, Value = score }
                }
            };
            
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
        }
        
        public void RetrieveLeaderboard()
        {
            if (!_loginSuccess)
            {
                Debug.LogWarning("Leaderboard retrieval blocked: Not logged into PlayFab.");
                return; 
            }
    
            var request = new GetLeaderboardRequest
            {
                StatisticName = PlayerPlatformScore,
                StartPosition = 0,
                MaxResultsCount = 10,
                
                ProfileConstraints = new PlayerProfileViewConstraints 
                {
                    ShowDisplayName = true 
                }
                
            };
    
            PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
        }

        public void OnLeaderboardGet(GetLeaderboardResult result)
        {
            foreach (var item in result.Leaderboard)
            {
                var nickname = item.Profile.DisplayName; 
                
                if (string.IsNullOrEmpty(nickname))
                {
                    nickname = $"[Player {item.PlayFabId}]";
                }
                
                Debug.Log($"Rank {item.Position + 1}: {nickname} - Score: {item.StatValue}");
            }
        }
        
        #endregion

        #region Login setup

        private void Login()
        {
            var request = new LoginWithCustomIDRequest()
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
        }
        
        private void OnSuccess(LoginResult result)
        {
            Debug.Log("Login Success");
            _loginSuccess = true;
            GetPlayerData();
        }

        private void OnError(PlayFabError error)
        {
            Debug.LogError("PlayFab login Failed!" + error.GenerateErrorReport());
            _loginSuccess = false;
        }
        
        #endregion
        
        private static void OnNicknameSaveSuccess(UpdateUserDataResult result)
        {
            Debug.Log("Nickname saved successfully!");
        }

        private static void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
        {
            Debug.Log("Score submitted successfully!");
        }
        
        #region Data handling
        
        public void GetPlayerData()
        {
            if (!_loginSuccess)
            {
                Debug.LogWarning("Player data retrieval blocked: Not logged into PlayFab.");
                return;
            }
            
            PlayFabClientAPI.GetUserData(new GetUserDataRequest { Keys = null }, OnDataReceive, OnError);
        }

        private static void OnDataReceive(GetUserDataResult result)
        {
            const string NicknameKey = "Nickname"; 

            if (result.Data != null && result.Data.ContainsKey(NicknameKey))
            {
                var savedNickname = result.Data[NicknameKey].Value;
                
                Debug.Log($"Retrieved Player Data Nickname: {savedNickname}");
                
            }
            else
            {
                Debug.LogWarning($"Player Data Key '{NicknameKey}' not found.");
            }
        }
        
        #endregion
    }
}