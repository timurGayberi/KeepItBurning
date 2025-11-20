using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonTextHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Assign TMP Text ")]
    [SerializeField] private TMP_Text targetText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetText != null)
            targetText.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetText != null)
            targetText.color = normalColor;
    }

    private void Reset()
    {
        // Auto-assign text if script is placed on the Button
        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>();
    }
}