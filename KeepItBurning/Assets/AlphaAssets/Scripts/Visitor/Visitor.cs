using UnityEngine;
using UnityEngine.UI;

public class Visitor : MonoBehaviour
{
    [Header("References")]
    public Image thoughtBubbleImage;
    public Sprite defaultBubbleSprite;

    [Header("Settings")]
    public float waitTime = 10f;

    private float timer;
    private IVisitorState currentState;

    public void SetState(IVisitorState newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    private void Update()
    {
        currentState?.Update(this);
    }

    public void ResetTimer()
    {
        timer = waitTime;
    }

    public bool TickTimer()
    {
        timer -= Time.deltaTime;
        return timer <= 0;
    }

    public void ShowThoughtBubble(Sprite itemSprite)
    {
        if (thoughtBubbleImage != null)
        {
            thoughtBubbleImage.sprite = itemSprite;
            thoughtBubbleImage.enabled = true;
        }
    }

    public void HideThoughtBubble()
    {
        if (thoughtBubbleImage != null)
        {
            thoughtBubbleImage.sprite = defaultBubbleSprite;
            thoughtBubbleImage.enabled = false;
        }
    }
}