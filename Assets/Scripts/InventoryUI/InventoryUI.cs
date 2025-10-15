using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryGrid inventoryGrid;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private Color normalCellColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    [SerializeField] private Color highlightCellColor = new Color(0.5f, 0.8f, 0.5f, 0.5f);
    [SerializeField] private Color invalidCellColor = new Color(0.8f, 0.3f, 0.3f, 0.5f);
    
    private GameObject[,] cellObjects;
    private Dictionary<GridInventoryItem, GameObject> itemVisuals = new Dictionary<GridInventoryItem, GameObject>();
    
    private void Start()
    {
        if (inventoryGrid == null)
        {
            inventoryGrid = GetComponent<InventoryGrid>();
        }
        
        CreateGridVisuals();
    }
    
    private void CreateGridVisuals()
    {
        int width = inventoryGrid.GetWidth();
        int height = inventoryGrid.GetHeight();
        float cellSize = inventoryGrid.GetCellSize();
        
        cellObjects = new GameObject[width, height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject cell = Instantiate(cellPrefab, gridContainer);
                RectTransform rectTransform = cell.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(x * cellSize, y * cellSize);
                rectTransform.sizeDelta = new Vector2(cellSize, cellSize);
                
                Image image = cell.GetComponent<Image>();
                if (image != null)
                {
                    image.color = normalCellColor;
                }
                
                cellObjects[x, y] = cell;
            }
        }
    }
    
    public void ShowItemVisual(GridInventoryItem gridItem)
    {
        if (itemVisuals.ContainsKey(gridItem))
        {
            return;
        }
        
        GameObject itemVisual = Instantiate(itemPrefab, gridContainer);
        RectTransform rectTransform = itemVisual.GetComponent<RectTransform>();
        
        float cellSize = inventoryGrid.GetCellSize();
        rectTransform.anchoredPosition = new Vector2(gridItem.posX * cellSize, gridItem.posY * cellSize);
        rectTransform.sizeDelta = new Vector2(gridItem.itemData.width * cellSize, gridItem.itemData.height * cellSize);
        
        Image image = itemVisual.GetComponent<Image>();
        if (image != null && gridItem.itemData.icon != null)
        {
            image.sprite = gridItem.itemData.icon;
        }
        
        itemVisuals[gridItem] = itemVisual;
    }
    
    public void RemoveItemVisual(GridInventoryItem gridItem)
    {
        if (itemVisuals.ContainsKey(gridItem))
        {
            Destroy(itemVisuals[gridItem]);
            itemVisuals.Remove(gridItem);
        }
    }
    
    public void HighlightCells(int startX, int startY, int width, int height, bool isValid)
    {
        ResetCellColors();
        
        Color highlightColor = isValid ? highlightCellColor : invalidCellColor;
        
        for (int x = startX; x < startX + width && x < inventoryGrid.GetWidth(); x++)
        {
            for (int y = startY; y < startY + height && y < inventoryGrid.GetHeight(); y++)
            {
                if (x >= 0 && y >= 0)
                {
                    Image image = cellObjects[x, y].GetComponent<Image>();
                    if (image != null)
                    {
                        image.color = highlightColor;
                    }
                }
            }
        }
    }
    
    public void ResetCellColors()
    {
        for (int x = 0; x < inventoryGrid.GetWidth(); x++)
        {
            for (int y = 0; y < inventoryGrid.GetHeight(); y++)
            {
                Image image = cellObjects[x, y].GetComponent<Image>();
                if (image != null)
                {
                    image.color = normalCellColor;
                }
            }
        }
    }
    
    public void RefreshAllItems()
    {
        // Usuń wszystkie wizualizacje
        foreach (var visual in itemVisuals.Values)
        {
            Destroy(visual);
        }
        itemVisuals.Clear();
        
        // Dodaj wizualizacje dla wszystkich itemów
        foreach (var item in inventoryGrid.GetAllItems())
        {
            ShowItemVisual(item);
        }
    }
}

