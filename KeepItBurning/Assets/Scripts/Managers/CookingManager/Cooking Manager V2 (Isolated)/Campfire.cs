using UnityEngine;

public class Campfire : MonoBehaviour
{
    [Header("Cooking Settings")]
    public float cookRange = 2f;
    public Transform cookPoint;
    private CookableItem currentItem;

    private bool isCooking = false;

    public void Interact()
    {
        if (!isCooking)
            StartCooking();
        else
            StopCooking();
    }

    void StartCooking()
    {
        Collider[] hits = Physics.OverlapSphere(cookPoint.position, cookRange);
        foreach (var hit in hits)
        {
            var cookable = hit.GetComponent<CookableItem>();
            if (cookable != null)
            {
                currentItem = cookable;
                currentItem.transform.position = cookPoint.position;
                currentItem.StartCooking();
                isCooking = true;
                Debug.Log($"Started cooking: {cookable.itemName}");
                return;
            }
        }

        Debug.LogWarning("No Cookable item found near campfire!");
    }

    void StopCooking()
    {
        if (currentItem != null)
        {
            currentItem.StopCooking();
            Debug.Log($"Stopped cooking: {currentItem.itemName}");
        }

        isCooking = false;
        currentItem = null;
    }

    private void OnDrawGizmos()
    {
        if (cookPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(cookPoint.position, cookRange);
    }
}