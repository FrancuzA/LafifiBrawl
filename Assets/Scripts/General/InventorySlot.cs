using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int inventoryIndex;
    private bool isOccupied = false;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;
        DragableItem dragableItem = dropped.GetComponent<DragableItem>();
        
        if (dragableItem != null && !IsOccupied())
        {
            dragableItem.parentAfterDrag = transform;
        }
    }
    
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
    }
    
    public bool IsOccupied()
    {
        return isOccupied || transform.childCount > 0;
    }
}
