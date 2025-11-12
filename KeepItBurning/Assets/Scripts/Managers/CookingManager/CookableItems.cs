using General;
using Player;
using UnityEngine;

public class CookableItems : MonoBehaviour
{
    [Header("References")]
    public CollectibleBase collectibleBase;
    public CollectiblesLogic collectiblesLogic;

    [Header("Cooking Settings")]
    public float cookTime = 5f;
    public float burnTime = 9f;

    private float timer = 0f;
    private bool isOnHeat = false;

    void Awake()
    {
        if (collectibleBase != null)
            collectibleBase.currentState = CollectibleBase.CookState.Raw;

        collectiblesLogic.chocolateVisual.SetActive(false);
        collectiblesLogic.hotChocolateVisual.SetActive(false);
        collectiblesLogic.burnedHotChocolateVisual.SetActive(false);
    }

    void Update()
    {
        if (!isOnHeat) return;

        timer += Time.deltaTime;

        if (timer >= burnTime)
            SetState(CollectibleBase.CookState.Burnt);
        else if (timer >= cookTime)
            SetState(CollectibleBase.CookState.Cooked);
        else
            SetState(CollectibleBase.CookState.Raw);
    }

    void SetState(CollectibleBase.CookState newState)
    {
        if (collectibleBase.currentState == newState) return;

        collectibleBase.currentState = newState;

        collectiblesLogic.chocolateVisual.SetActive(newState == CollectibleBase.CookState.Raw);
        collectiblesLogic.hotChocolateVisual.SetActive(newState == CollectibleBase.CookState.Cooked);
        collectiblesLogic.burnedHotChocolateVisual.SetActive(newState == CollectibleBase.CookState.Burnt);
    }

    public void StartCooking()
    {
        isOnHeat = true;
    }

    public void StopCooking()
    {
        isOnHeat = false;
    }

    public float GetCookingProgress() => Mathf.Clamp01(timer / burnTime);
}