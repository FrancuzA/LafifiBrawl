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
    private ItemShapeVisualizer shapeVisualizer;
    
    private void Awake()
    {
        image = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
        shapeVisualizer = GetComponent<ItemShapeVisualizer>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Upewnij się, że komponenty są zainicjalizowane
        if (image == null)
            image = GetComponent<Image>();
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        if (shapeVisualizer == null)
            shapeVisualizer = GetComponent<ItemShapeVisualizer>();
            
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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                canvas.worldCamera,
                out position);
            transform.localPosition = position;
            
            // Pokazuj kształt obiektu podczas przeciągania
            if (shapeVisualizer != null)
            {
                shapeVisualizer.ShowShapeAtPosition(transform.position);
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Ukryj wizualizację kształtu
        if (shapeVisualizer != null)
        {
            shapeVisualizer.HideShape();
        }
        
        // Sprawdź czy parentAfterDrag został zmieniony przez OnDrop
        if (parentAfterDrag != originalParent)
        {
            // Drop się udał - umieść obiekt w nowym rodzicu
            transform.SetParent(parentAfterDrag);
            transform.localPosition = Vector3.zero;
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
