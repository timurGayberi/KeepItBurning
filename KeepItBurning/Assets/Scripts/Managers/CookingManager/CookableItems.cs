using General;
using UnityEngine;

public class CookableItems : MonoBehaviour
{
    public CollectibleBase collectibleBase;

    [Header("Cooking Settings")]
    public string itemName = "Food";
    public float cookTime = 5f;
    public float burnTime = 9f;

    private float timer = 0f;
    private bool isOnHeat = false;

    [Header("State Objects (Child visuals)")]
    [Tooltip("Raw version (default active at start)")]
    [SerializeField] private GameObject rawVisual;
    [Tooltip("Cooked version (hidden until ready)")]
    [SerializeField] private GameObject cookedVisual;
    [Tooltip("Burnt version (hidden until burnt)")]
    [SerializeField] private GameObject burntVisual;

    void Awake()
    {
        SetActiveVisual(rawVisual);
        collectibleBase.currentState = CollectibleBase.CookState.Raw;
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

        switch (newState)
        {
            case CollectibleBase.CookState.Raw:
                SetActiveVisual(rawVisual);
                break;
            case CollectibleBase.CookState.Cooked:
                SetActiveVisual(cookedVisual);
                break;
            case CollectibleBase.CookState.Burnt:
                SetActiveVisual(burntVisual);
                break;
        }

        Debug.Log($"{itemName} state → {newState}");
    }

    void SetActiveVisual(GameObject activeObj)
    {
        if (rawVisual != null) rawVisual.SetActive(activeObj == rawVisual);
        if (cookedVisual != null) cookedVisual.SetActive(activeObj == cookedVisual);
        if (burntVisual != null) burntVisual.SetActive(activeObj == burntVisual);
    }

    public void StartCooking()
    {
        isOnHeat = true;
        Debug.Log($"{itemName} cooking started.");
    }

    public void StopCooking()
    {
        isOnHeat = false;
        Debug.Log($"{itemName} cooking stopped.");
    }

    public float GetCookingProgress() => Mathf.Clamp01(timer / burnTime);
}