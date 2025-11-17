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
                                PlayerPlatformScore =   "Game Score",
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
        
        #region Public Service Methods
        
        public void SaveNickname(string nickname)
        {
            if (!_loginSuccess)
            {
                Debug.LogWarning("Nickname save blocked: Not logged into PlayFab.");
                return;
            }

            var request = new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string> { {PlayerNickName , nickname} },
                Permission = UserDataPermission.Public
            };
            
            PlayFabClientAPI.UpdateUserData(request, OnNicknameSaveSuccess, OnError);
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
                MaxResultsCount = 10
            };
            
            PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardGet, OnError);
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
        
        private static void OnLeaderboardGet(GetLeaderboardResult result)
        {
            Debug.Log($"Retrieved {result.Leaderboard.Count} leaderboard entries.");
        }
    }
}