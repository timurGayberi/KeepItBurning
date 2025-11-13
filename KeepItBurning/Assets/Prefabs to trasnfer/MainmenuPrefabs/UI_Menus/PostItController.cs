using UnityEngine;
using UnityEngine.EventSystems;

public class PostItController : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Animator animator;

    private bool isExpanded = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (animator != null)
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isExpanded)
        {
            animator.SetBool("isHovered", true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isExpanded)
        {
            animator.SetBool("isHovered", false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isExpanded)
        {
            animator.SetTrigger("postItDismissed");
            isExpanded = false;
        }
        else
        {
            animator.SetTrigger("postItClicked");
            animator.SetBool("isHovered", false);
            isExpanded = true;
        }
    }

    public void DismissPostIt()
    {
        if (isExpanded)
        {
            animator.SetTrigger("postItDismissed");
            isExpanded = false;
        }
    }
}
