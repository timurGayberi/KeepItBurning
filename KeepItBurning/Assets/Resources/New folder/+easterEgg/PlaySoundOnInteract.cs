using UnityEngine;
using Interfaces;
using General;

public class PlaySoundOnInteract : MonoBehaviour
{
    public AudioSource audioSource;
    private bool playerInside = false;
    private IInputService inputService;

    private void OnEnable()
    {
        try
        {
            inputService = ServiceLocator.GetService<IInputService>();
            if (inputService != null)
            {
                inputService.OnInteractEvent += OnInteractPressed;
            }
        }
        catch (System.InvalidOperationException e)
        {
            Debug.LogWarning("[PlaySoundOnInteract] Could not get IInputService. Error: " + e.Message);
        }
    }

    private void OnDisable()
    {
        if (inputService != null)
        {
            inputService.OnInteractEvent -= OnInteractPressed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    private void OnInteractPressed()
    {
        Debug.Log($"[PlaySoundOnInteract] OnInteractPressed called. playerInside: {playerInside}, audioSource: {audioSource != null}");
        if (playerInside && audioSource != null)
        {
            Debug.Log("[PlaySoundOnInteract] Playing sound!");
            audioSource.Play();
        }
    }
}