using Interfaces;
using UnityEngine;

public class PickupPoint : MonoBehaviour, IInteractable
{
    [Header("Spawn Settings")]
    public GameObject chocolatePrefab;
    public GameObject marshmallowPrefab;
    public GameObject sausagePrefab;

    public Transform spawnPoint;

    private string prompt = "Press E to take an item";

    public void Interact()
    {
        GameObject item = Instantiate(chocolatePrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log($"Spawned {item.name} from pickup point.");
    }

    public InteractionData GetInteractionData()
    {
        return new InteractionData
        {
            promptText = prompt,
            actionDuration = 0f
        };
    }

    public void StopInteraction() { }
}