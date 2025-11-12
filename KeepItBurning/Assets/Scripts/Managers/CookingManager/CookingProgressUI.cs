using UnityEngine;
using UnityEngine.UI;

public class CookingProgressUI : MonoBehaviour
{
    [Header("References")]
    public CookableItems cookableItems;
    public Image progressCircle;

    void Update()
    {
        if (cookableItems == null || progressCircle == null) return;

        float progress = cookableItems.GetCookingProgress();
        progressCircle.fillAmount = progress;

        if (cookableItems.currentState == CookableItems.CookState.Raw)
            progressCircle.color = Color.green;
        else if (cookableItems.currentState == CookableItems.CookState.Cooked)
            progressCircle.color = Color.yellow;
        else if (cookableItems.currentState == CookableItems.CookState.Burnt)
            progressCircle.color = Color.red;
    }
}