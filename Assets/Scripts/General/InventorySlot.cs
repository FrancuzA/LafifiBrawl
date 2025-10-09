using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public int inventoryIndex;
    private bool isOccupied = false;
    
    public void OnDrop(PointerEventData eventData)
    {
        var dropped = eventData.pointerDrag;
        var lafifi = dropped.GetComponent<Lafifi>();
        var dragableItem = dropped.GetComponent<DragableItem>();
        
        if (lafifi == null || dragableItem == null) return;
        
        // Znajdź InventoryGrid
        var inventoryGrid = GetComponentInParent<InventoryGrid>();
        if (inventoryGrid == null) return;
        
        // Sprawdź czy obiekt może być umieszczony w tym miejscu
        var shape = lafifi.GetItemShape();
        if (shape != null && inventoryGrid.CanPlaceItem(inventoryIndex, shape))
        {
            // Usuń obiekt z poprzedniego miejsca
            inventoryGrid.RemoveItem(dropped);
            
            // Umieść obiekt w nowym miejscu
            if (inventoryGrid.PlaceItem(dropped, inventoryIndex, shape))
            {
                // Informuj DragableItem, że drop się udał
                dragableItem.parentAfterDrag = transform;
            }
        }
        // Jeśli nie można umieścić, parentAfterDrag pozostaje niezmieniony
        // i OnEndDrag automatycznie przywróci obiekt do oryginalnej pozycji
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
