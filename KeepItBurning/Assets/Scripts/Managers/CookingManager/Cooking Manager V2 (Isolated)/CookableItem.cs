using UnityEngine;

public class CookableItem : MonoBehaviour
{
    public string itemName;
    public CookState currentState = CookState.Raw;

    [Header("Cooking Times")]
    public float cookTime = 5f;
    public float burnTime = 9f;

    [Header("Prefabs")]
    public GameObject rawPrefab;
    public GameObject cookedPrefab;
    public GameObject burnedPrefab;

    private float timer;
    private bool isCooking = false;

    public void StartCooking()
    {
        if (isCooking) return;
        isCooking = true;
        timer = 0f;
    }

    public void StopCooking()
    {
        isCooking = false;
    }

    private void Update()
    {
        if (!isCooking) return;

        timer += Time.deltaTime;

        if (timer >= burnTime)
            SetState(CookState.Burned);
        else if (timer >= cookTime)
            SetState(CookState.Cooked);
        else
            SetState(CookState.Raw);
    }

    private void SetState(CookState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        if (rawPrefab) rawPrefab.SetActive(newState == CookState.Raw);
        if (cookedPrefab) cookedPrefab.SetActive(newState == CookState.Cooked);
        if (burnedPrefab) burnedPrefab.SetActive(newState == CookState.Burned);
    }
}
