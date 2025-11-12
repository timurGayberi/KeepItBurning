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

    [Header("Prefabs")]
    public GameObject rawPrefab;
    public GameObject cookedPrefab;
    public GameObject burntPrefab;

    private GameObject currentVisual;

    void Start()
    {
        SpawnVisual(rawPrefab);
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

        if (currentVisual != null)
            Destroy(currentVisual);

        switch (newState)
        {
            case CookState.Raw:
                SpawnVisual(rawPrefab);
                break;
            case CookState.Cooked:
                SpawnVisual(cookedPrefab);
                break;
            case CookState.Burnt:
                SpawnVisual(burntPrefab);
                break;
        }

        Debug.Log($"{itemName} now: {newState}");
    }

    void SpawnVisual(GameObject prefab)
    {
        if (prefab == null) return;
        currentVisual = Instantiate(prefab, transform.position, transform.rotation, transform);
    }

    public float GetCookingProgress()
    {
        return Mathf.Clamp01(timer / burnTime);
    }

    public void StartCooking()
    {
        isOnHeat = true;
    }

    public void StopCooking()
    {
        isOnHeat = false;
        timer = 0;
    }
}