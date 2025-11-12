using UnityEngine;

public class TestInteraction : MonoBehaviour
{
    public PickupPoint pickupPoint;
    public Campfire campfire;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Pressed 1 → Pick up item from table");
            pickupPoint.Interact();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Pressed 2 → Start cooking at campfire");
            campfire.Interact();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("Pressed 3 → Stop cooking at campfire");
            campfire.Interact();
        }
    }
}