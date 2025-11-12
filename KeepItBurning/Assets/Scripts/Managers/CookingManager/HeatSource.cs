using UnityEngine;

public class HeatSource : MonoBehaviour
{
    [Header("Cooking Setup")]
    public CookableItems cookableItem;
    public Transform cookPoint;

    public bool isCooking = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isCooking)
                StartCooking();
            else
                StopCooking();
        }
    }

    void StartCooking()
    {
        if (cookableItem == null) return;

        cookableItem.transform.position = cookPoint.position;
        cookableItem.StartCooking();
        isCooking = true;
    }

    void StopCooking()
    {
        cookableItem.StopCooking();
        isCooking = false;
    }
}