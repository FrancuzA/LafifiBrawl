using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShapeVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject cellHighlightPrefab;
    [SerializeField] private Color validPlacementColor = Color.green;
    [SerializeField] private Color invalidPlacementColor = Color.red;
    
    private List<GameObject> highlightCells = new List<GameObject>();
    private InventoryGrid inventoryGrid;
    private Lafifi lafifi;
    private Canvas canvas;
    
    private void Awake()
    {
        lafifi = GetComponent<Lafifi>();
        canvas = GetComponentInParent<Canvas>();
        
        // Znajdź InventoryGrid w scenie
        inventoryGrid = FindObjectOfType<InventoryGrid>();
        
        // Utwórz prefab podświetlenia komórki jeśli nie został przypisany
        if (cellHighlightPrefab == null)
        {
            CreateDefaultHighlightPrefab();
        }
    }
    
    private void CreateDefaultHighlightPrefab()
    {
        // Utwórz prosty prefab do podświetlania komórek
        GameObject prefab = new GameObject("CellHighlight");
        Image image = prefab.AddComponent<Image>();
        image.color = new Color(1, 1, 1, 0.5f);
        
        RectTransform rect = prefab.GetComponent<RectTransform>();
        rect.sizeDelta = Vector2.one * 50f; // Dostosuj rozmiar do swoich slotów
        
        cellHighlightPrefab = prefab;
        
        // Ukryj prefab
        prefab.SetActive(false);
    }
    
    public void ShowShape(int startSlotIndex, bool isValidPlacement = true)
    {
        HideShape();
        
        if (inventoryGrid == null || lafifi == null) return;
        
        var shape = lafifi.GetItemShape();
        if (shape == null) return;
        
        Color highlightColor = isValidPlacement ? validPlacementColor : invalidPlacementColor;
        
        foreach (var cell in shape.occupiedCells)
        {
            Vector2Int gridPos = inventoryGrid.IndexToGridPosition(startSlotIndex) + cell;
            int cellIndex = inventoryGrid.GridPositionToIndex(gridPos);
            
            // Sprawdź czy komórka mieści się w siatce
            if (cellIndex >= 0 && cellIndex < inventoryGrid.GetSlotsCount())
            {
                var slot = inventoryGrid.GetSlot(cellIndex);
                if (slot != null)
                {
                    GameObject highlight = Instantiate(cellHighlightPrefab, slot.transform);
                    highlight.SetActive(true);
                    
                    Image highlightImage = highlight.GetComponent<Image>();
                    if (highlightImage != null)
                    {
                        highlightImage.color = highlightColor;
                    }
                    
                    highlight.transform.SetAsFirstSibling(); // Umieść za obiektem
                    highlightCells.Add(highlight);
                }
            }
        }
    }
    
    public void HideShape()
    {
        foreach (var highlight in highlightCells)
        {
            if (highlight != null)
                DestroyImmediate(highlight);
        }
        highlightCells.Clear();
    }
    
    public void ShowShapeAtPosition(Vector3 worldPosition)
    {
        if (inventoryGrid == null) return;
        
        int closestSlotIndex = inventoryGrid.GetClosestSlotIndex(worldPosition);
        bool canPlace = inventoryGrid.CanPlaceItem(closestSlotIndex, lafifi.GetItemShape());
        
        ShowShape(closestSlotIndex, canPlace);
    }
    
    private void OnDestroy()
    {
        HideShape();
    }
}
