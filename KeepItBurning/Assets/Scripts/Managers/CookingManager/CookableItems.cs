using General;
using Player;
using UnityEngine;

public class CookableItems : MonoBehaviour
{
    [Header("References")]
    public CollectibleBase collectibleBase;
    public PlayerInventory playerInventory;

    [Header("Cooking Settings")]
    public float cookTime = 5f;
    public float burnTime = 9f;

    private float timer = 0f;
    private bool isOnHeat = false;

    void Awake()
    {
        if (collectibleBase != null)
            collectibleBase.currentState = CollectibleBase.CookState.Raw;

        playerInventory.chocolateVisual.SetActive(false);
        playerInventory.hotChocolateVisual.SetActive(false);
        playerInventory.burnedHotChocolateVisual.SetActive(false);
    }

    void Update()
    {
        if (!isOnHeat) return;

        timer += Time.deltaTime;

        if (timer >= burnTime)
        {
            SetState(CollectibleBase.CookState.Burnt);
            SoundManager.Play(SoundAction.BurnedFood);
        }
            
        else if (timer >= cookTime)
        {
            SoundManager.Play(SoundAction.FoodCoockedGood);
            SetState(CollectibleBase.CookState.Cooked);
        }
            
        else
        {
            SetState(CollectibleBase.CookState.Raw);
        }
            
    }

    void SetState(CollectibleBase.CookState newState)
    {
        if (collectibleBase.currentState == newState) return;

        collectibleBase.currentState = newState;

        playerInventory.chocolateVisual.SetActive(newState == CollectibleBase.CookState.Raw);
        playerInventory.hotChocolateVisual.SetActive(newState == CollectibleBase.CookState.Cooked);
        playerInventory.burnedHotChocolateVisual.SetActive(newState == CollectibleBase.CookState.Burnt);
    }

    public void StartCooking()
    {
        SoundManager.Play(SoundAction.PutingFoodInCampFire);
        isOnHeat = true;
        timer = 0f;
    }

    public void StopCooking()
    {
        isOnHeat = false;
    }

    public float GetCookingProgress() => Mathf.Clamp01(timer / burnTime);
}