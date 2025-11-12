using UnityEngine;
using UnityEngine.EventSystems;

public class ClipboardTextAnimator : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void OnPointerEnter(PointerEventData e)
        => animator.SetBool("isHighlighted", true);

    public void OnPointerExit(PointerEventData e)
        => animator.SetBool("isHighlighted", false);

    public void OnPointerDown(PointerEventData e)
        => animator.SetBool("isPressed", true);

    public void OnPointerUp(PointerEventData e)
        => animator.SetBool("isPressed", false);

    public void OnSelect(BaseEventData e)
        => animator.SetBool("isHighlighted", true);

    public void OnDeselect(BaseEventData e)
        => animator.SetBool("isHighlighted", false);
}
