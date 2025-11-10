using UnityEngine;
using UnityEngine.UI;
using Managers.GeneralManagers;

[RequireComponent(typeof(Button))]
public class ResumeButtonHelper : MonoBehaviour
{
    void Start()
    {
        Button btn = GetComponent<Button>();
        
        if (GameStateManager.instance != null)
        {
            btn.onClick.AddListener(GameStateManager.instance.ResumeGameFromUI);
        }
        else
        {
            Debug.LogError("Resume Button couldn't find GameStateManager!");
        }
    }
}