using General;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class CookingProgressUI : MonoBehaviour
{
    [Header("References")]
    public CollectibleBase collectibleBase;
    public CookableItems cookableItems;
    public Image progressCircle;

    void Update()
    {
        if (cookableItems == null || progressCircle == null) return;

        float progress = cookableItems.GetCookingProgress();
        progressCircle.fillAmount = progress;

        if (collectibleBase.currentState == CollectibleBase.CookState.Raw)
            progressCircle.color = Color.green;
        else if (collectibleBase.currentState == CollectibleBase.CookState.Cooked)
            progressCircle.color = Color.yellow;
        else if (collectibleBase.currentState == CollectibleBase.CookState.Burnt)
            progressCircle.color = Color.red;
    }
}