using UnityEngine;
using Interfaces;
using General;
using TMPro;

public class PlaySoundOnInteract : MonoBehaviour
{
    public AudioSource audioSource;
    [Header("Interaction Prompt")]
    [Tooltip("Optional: UI Text to show when player is near")]
    public TextMeshProUGUI interactionPromptText;
    [Tooltip("The text to display (e.g., 'Press E to interact')")]
    public string promptMessage = "Press E to interact";

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
            ShowPrompt();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.text = promptMessage;
            interactionPromptText.gameObject.SetActive(true);
        }
    }

    private void HidePrompt()
    {
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
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