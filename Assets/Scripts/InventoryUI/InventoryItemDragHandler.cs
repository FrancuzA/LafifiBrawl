using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private InventoryItem itemData;
    [SerializeField] private InventoryGrid inventoryGrid;
    [SerializeField] private InventoryUI inventoryUI;
    
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private GridInventoryItem currentGridItem;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        canvas = GetComponentInParent<Canvas>();
    }
    
    public void Initialize(InventoryItem item, InventoryGrid grid, InventoryUI ui)
    {
        itemData = item;
        inventoryGrid = grid;
        inventoryUI = ui;
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        
        // Znajdź i usuń item z gridu jeśli tam był
        currentGridItem = inventoryGrid.GetItemAtScreen(eventData.position);
        
        if (currentGridItem != null)
        {
            inventoryGrid.RemoveItem(currentGridItem);
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        
        // Pobierz pozycję grid z pozycji ekranu
        Vector2 localPoint;
        RectTransform gridRect = inventoryGrid.GetComponent<RectTransform>();
        if (gridRect == null)
        {
            // Jeśli InventoryGrid nie ma RectTransform, użyj origin position
            gridRect = inventoryGrid.transform.GetComponent<RectTransform>();
        }
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridRect,
            eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );
        
        int x = Mathf.FloorToInt(localPoint.x / inventoryGrid.GetCellSize());
        int y = Mathf.FloorToInt(localPoint.y / inventoryGrid.GetCellSize());
        
        bool canPlace = inventoryGrid.CanPlaceItem(itemData, x, y);
        inventoryUI.HighlightCells(x, y, itemData.width, itemData.height, canPlace);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        inventoryUI.ResetCellColors();
        
        bool placed = inventoryGrid.PlaceItemFromScreen(itemData, eventData.position);
        
        if (placed)
        {
            inventoryUI.RefreshAllItems();
            Destroy(gameObject); // Usuń tymczasowy obiekt - UI stworzy nowy
        }
        else
        {
            // Przywróć do oryginalnej pozycji jeśli nie można umieścić
            rectTransform.anchoredPosition = originalPosition;
            
            // Jeśli był wcześniej w gridzie, przywróć go tam
            if (currentGridItem != null)
            {
                inventoryGrid.PlaceItem(itemData, currentGridItem.posX, currentGridItem.posY);
                inventoryUI.RefreshAllItems();
            }
        }
    }
}
