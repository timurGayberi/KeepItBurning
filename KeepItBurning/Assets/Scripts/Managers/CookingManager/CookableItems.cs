using UnityEngine;

public class CookableItems : MonoBehaviour
{
    public enum CookState { Raw, Cooked, Burnt }
    public CookState currentState = CookState.Raw;

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
        currentState = CookState.Raw;
    }

    void Update()
    {
        if (!isOnHeat) return;

        timer += Time.deltaTime;

        if (timer >= burnTime)
            SetState(CookState.Burnt);
        else if (timer >= cookTime)
            SetState(CookState.Cooked);
        else
            SetState(CookState.Raw);
    }

    void SetState(CookState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (newState)
        {
            case CookState.Raw:
                SetActiveVisual(rawVisual);
                break;
            case CookState.Cooked:
                SetActiveVisual(cookedVisual);
                break;
            case CookState.Burnt:
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