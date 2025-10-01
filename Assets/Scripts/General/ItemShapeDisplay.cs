using UnityEngine;
using UnityEngine.EventSystems;

public class ItemShapeDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private ItemShapeVisualizer shapeVisualizer;
    private InventorySlot inventorySlot;
    
    private void Awake()
    {
        shapeVisualizer = GetComponent<ItemShapeVisualizer>();
        inventorySlot = GetComponentInParent<InventorySlot>();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Pokaż kształt obiektu gdy najedziesz myszą
        if (shapeVisualizer != null && inventorySlot != null)
        {
            shapeVisualizer.ShowShape(inventorySlot.inventoryIndex, true);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // Ukryj kształt gdy zjedziesz myszą
        if (shapeVisualizer != null)
        {
            shapeVisualizer.HideShape();
        }
    }
}
