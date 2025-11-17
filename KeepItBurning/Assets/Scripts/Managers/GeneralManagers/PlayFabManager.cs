using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace Managers.GeneralManagers
{
    public class PlayFabManager : MonoBehaviour
    {
        
        #region References

        private const string    PlayFabTitleID =        "1ECE61",
                                PlayerScore =           "Score",
                                PlayerPlatformScore =   "Game Score",
                                PlayerNickName =        "Nickname";

        #endregion
        
        private bool                                    _loginSuccess;

        private void Start()
        {
            PlayFabSettings.staticSettings.TitleId = PlayFabTitleID;
            
            Login();
        }
        
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

    }
}