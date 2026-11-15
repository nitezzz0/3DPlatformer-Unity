using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class HoverEnlarger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float sizeDifference = 1.05f;

    private Vector2 originalScale;
    private Transform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<Transform>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        rectTransform.localScale = originalScale * sizeDifference;
    }

    public void OnPointerExit(PointerEventData eventData)
    {

        rectTransform.localScale = originalScale;
    }
}
