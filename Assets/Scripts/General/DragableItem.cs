using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image image;
    [HideInInspector] public Transform parentAfterDrag;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Canvas canvas;
    
    private void Awake()
    {
        image = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (image == null)
            image = GetComponent<Image>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
            
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        
        parentAfterDrag = transform.parent;
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        
        if (image != null)
            image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out position);
            transform.localPosition = position;
    
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Sprawdź czy parentAfterDrag został zmieniony przez OnDrop
        if (parentAfterDrag != originalParent)
        {
            // Drop się udał - umieść obiekt w nowym rodzicu
            transform.SetParent(parentAfterDrag);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
        }
        else
        {
            // Drop się nie udał - wróć do oryginalnej pozycji
            ReturnToOriginalPosition();
        }
        
        if (image != null)
            image.raycastTarget = true;
    }
    
    public void ReturnToOriginalPosition()
    {
        if (originalParent != null)
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalPosition;
        }
        
        if (image != null)
            image.raycastTarget = true;
    }
}
