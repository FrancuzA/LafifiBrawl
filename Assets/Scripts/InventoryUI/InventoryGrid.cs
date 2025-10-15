using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 5;
    [SerializeField] private float cellSize = 64f;
    [SerializeField] private RectTransform originPosition;
    [SerializeField] private Canvas canvas;
    
    private Grid<GridInventoryItem> grid;
    private List<GridInventoryItem> itemsInInventory = new List<GridInventoryItem>();
    
    private void Start()
    {
        grid = new Grid<GridInventoryItem>(width, height, cellSize, originPosition.position, () => null);
        
        if (canvas == null)
        {
            canvas = originPosition.GetComponentInParent<Canvas>();
        }
    }
    
    public bool CanPlaceItem(InventoryItem item, int posX, int posY)
    {
        // Sprawdź czy item mieści się w gridzie
        if (posX < 0 || posY < 0 || posX + item.width > width || posY + item.height > height)
        {
            return false;
        }
        
        // Sprawdź czy wszystkie komórki są wolne
        for (int x = posX; x < posX + item.width; x++)
        {
            for (int y = posY; y < posY + item.height; y++)
            {
                if (grid.GetGridObject(x, y) != null)
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    public bool PlaceItem(InventoryItem item, int posX, int posY)
    {
        if (!CanPlaceItem(item, posX, posY))
        {
            Debug.Log($"Cannot place {item.itemName} at ({posX}, {posY})");
            return false;
        }
        
        GridInventoryItem gridItem = new GridInventoryItem(item, posX, posY);
        itemsInInventory.Add(gridItem);
        
        // Zajmij wszystkie komórki które item zajmuje
        for (int x = posX; x < posX + item.width; x++)
        {
            for (int y = posY; y < posY + item.height; y++)
            {
                grid.SetGridObject(x, y, gridItem);
            }
        }
        
        Debug.Log($"Placed {item.itemName} ({item.width}x{item.height}) at ({posX}, {posY})");
        return true;
    }
    
    public bool PlaceItem(InventoryItem item, Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int y);
        return PlaceItem(item, x, y);
    }
    
    public bool PlaceItemFromScreen(InventoryItem item, Vector3 screenPosition)
    {
        GetXYFromScreenPosition(screenPosition, out int x, out int y);
        return PlaceItem(item, x, y);
    }
    
    public GridInventoryItem GetItemAt(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }
    
    public GridInventoryItem GetItemAtScreen(Vector3 screenPosition)
    {
        GetXYFromScreenPosition(screenPosition, out int x, out int y);
        return GetItemAt(x, y);
    }
    
    public bool RemoveItem(GridInventoryItem item)
    {
        if (!itemsInInventory.Contains(item))
        {
            return false;
        }
        
        // Zwolnij wszystkie komórki
        for (int x = item.posX; x < item.posX + item.itemData.width; x++)
        {
            for (int y = item.posY; y < item.posY + item.itemData.height; y++)
            {
                grid.SetGridObject(x, y, null);
            }
        }
        
        itemsInInventory.Remove(item);
        Debug.Log($"Removed {item.itemData.itemName} from ({item.posX}, {item.posY})");
        return true;
    }
    
    public bool RemoveItemAt(int x, int y)
    {
        GridInventoryItem item = GetItemAt(x, y);
        if (item != null)
        {
            return RemoveItem(item);
        }
        return false;
    }
    
    /*public bool RemoveItemAt(Vector3 worldPosition)
    {
        GetXY(worldPosition, out int x, out int y);
        return RemoveItemAt(x, y);
    }*/
    
    public bool RemoveItemAt(Vector3 screenPosition)
    {
        GetXYFromScreenPosition(screenPosition, out int x, out int y);
        return RemoveItemAt(x, y);
    }
    
    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Vector3 gridPosition = worldPosition - originPosition.position;
        x = Mathf.FloorToInt(gridPosition.x / cellSize);
        y = Mathf.FloorToInt(gridPosition.y / cellSize);
    }
    
    private void GetXYFromScreenPosition(Vector3 screenPosition, out int x, out int y)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            originPosition, 
            screenPosition, 
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, 
            out localPoint
        );
        
        x = Mathf.FloorToInt(localPoint.x / cellSize);
        y = Mathf.FloorToInt(localPoint.y / cellSize);
        
        //Debug.Log($"Screen: {screenPosition}, Local: {localPoint}, Grid: ({x}, {y})");
    }
    
    public List<GridInventoryItem> GetAllItems()
    {
        return new List<GridInventoryItem>(itemsInInventory);
    }
    
    public int GetWidth() => width;
    public int GetHeight() => height;
    public float GetCellSize() => cellSize;
    public Vector3 GetOriginPosition() => originPosition.position;
}
