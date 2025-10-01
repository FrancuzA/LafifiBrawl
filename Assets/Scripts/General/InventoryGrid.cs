using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{
    [SerializeField] private GameObject holderPrefab;
    [SerializeField] private int rows = 4;
    [SerializeField] private int columns = 4;
    
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
    private GridLayoutGroup gridLayoutGroup;
    
    private void Awake()
    {
        // Sprawdź czy parent ma GridLayoutGroup
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        
        // Jeśli ma GridLayoutGroup, zsynchronizuj ustawienia
        if (gridLayoutGroup != null)
        {
            SyncWithGridLayoutGroup();
        }
    }
    
    private void Start()
    {
        CreateSlots();
    }
    
    private void SyncWithGridLayoutGroup()
    {
        // Ustaw constraint na Fixed Column Count jeśli nie jest ustawiony
        if (gridLayoutGroup.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
        {
            gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        }
        
        // Zsynchronizuj liczbę kolumn
        gridLayoutGroup.constraintCount = columns;
    }
    
    private void CreateSlots()
    {
        // Usuń istniejące sloty jeśli jakieś są
        ClearExistingSlots();
        
        // Utwórz sloty
        for (int i = 0; i < rows * columns; i++)
        {
            var newObj = Instantiate(holderPrefab, transform);
            var inventorySlot = newObj.GetComponent<InventorySlot>();
            
            if (inventorySlot == null)
            {
                inventorySlot = newObj.AddComponent<InventorySlot>();
            }
            
            inventorySlot.inventoryIndex = i;
            slots.Add(inventorySlot);
            
            // Nazwa dla łatwiejszego debugowania
            newObj.name = $"Slot_{i} ({i % columns}, {i / columns})";
        }
        
        // Przebuduj layout jeśli używamy GridLayoutGroup
        if (gridLayoutGroup != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }
    }
    
    private void ClearExistingSlots()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                DestroyImmediate(slot.gameObject);
            }
        }
        slots.Clear();
    }
    
    // Metoda sprawdzająca czy obiekt może być umieszczony w danej pozycji
    public bool CanPlaceItem(int startIndex, ItemShape shape)
    {
        if (shape == null || startIndex < 0 || startIndex >= slots.Count)
            return false;
            
        Vector2Int startPos = IndexToGridPosition(startIndex);
        
        foreach (var cell in shape.occupiedCells)
        {
            Vector2Int cellPos = startPos + cell;
            
            // Sprawdź czy pozycja mieści się w siatce
            if (cellPos.x < 0 || cellPos.x >= columns || cellPos.y < 0 || cellPos.y >= rows)
                return false;
                
            // Sprawdź czy slot jest wolny
            int slotIndex = GridPositionToIndex(cellPos);
            if (slotIndex >= slots.Count || slots[slotIndex].IsOccupied())
                return false;
        }
        
        return true;
    }
    
    // Metoda umieszczająca obiekt na siatce
    public bool PlaceItem(GameObject item, int startIndex, ItemShape shape)
    {
        if (!CanPlaceItem(startIndex, shape))
            return false;
            
        Vector2Int startPos = IndexToGridPosition(startIndex);
        
        // Umieść główny obiekt w pierwszym slocie
        item.transform.SetParent(slots[startIndex].transform);
        item.transform.localPosition = Vector3.zero;
        
        // Zaznacz wszystkie zajęte sloty
        var lafifi = item.GetComponent<Lafifi>();
        if (lafifi != null)
        {
            lafifi.occupiedSlots = new List<int>();
            
            foreach (var cell in shape.occupiedCells)
            {
                Vector2Int cellPos = startPos + cell;
                int slotIndex = GridPositionToIndex(cellPos);
                lafifi.occupiedSlots.Add(slotIndex);
                
                // Jeśli to nie główny slot, oznacz jako zajęty
                if (slotIndex != startIndex)
                {
                    slots[slotIndex].SetOccupied(true);
                }
            }
        }
        
        return true;
    }
    
    // Metoda usuwająca obiekt z siatki
    public void RemoveItem(GameObject item)
    {
        var lafifi = item.GetComponent<Lafifi>();
        if (lafifi != null && lafifi.occupiedSlots != null)
        {
            foreach (int slotIndex in lafifi.occupiedSlots)
            {
                if (slotIndex < slots.Count)
                {
                    slots[slotIndex].SetOccupied(false);
                }
            }
            lafifi.occupiedSlots.Clear();
        }
    }
    
    // Konwersja indeksu na pozycję w siatce
    public Vector2Int IndexToGridPosition(int index)
    {
        return new Vector2Int(index % columns, index / columns);
    }
    
    // Konwersja pozycji siatki na indeks
    public int GridPositionToIndex(Vector2Int gridPos)
    {
        return gridPos.y * columns + gridPos.x;
    }
    
    // Metoda znajdująca najbliższy slot do pozycji świata
    public int GetClosestSlotIndex(Vector3 worldPosition)
    {
        float closestDistance = float.MaxValue;
        int closestIndex = 0;
        
        for (int i = 0; i < slots.Count; i++)
        {
            float distance = Vector3.Distance(worldPosition, slots[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        
        return closestIndex;
    }
    
    // Publiczne metody pomocnicze dla ItemShapeVisualizer
    public int GetSlotsCount()
    {
        return slots.Count;
    }
    
    public InventorySlot GetSlot(int index)
    {
        if (index >= 0 && index < slots.Count)
            return slots[index];
        return null;
    }
    
    // Metoda do zmiany rozmiaru siatki w runtime
    [ContextMenu("Rebuild Grid")]
    public void RebuildGrid()
    {
        CreateSlots();
    }
    
    public void SetGridSize(int newRows, int newColumns)
    {
        rows = newRows;
        columns = newColumns;
        
        if (gridLayoutGroup != null)
        {
            gridLayoutGroup.constraintCount = columns;
        }
        
        RebuildGrid();
    }
    
    // Metoda pomocnicza do debugowania
    public void ValidateGridLayout()
    {
        if (gridLayoutGroup != null)
        {
            Debug.Log($"GridLayoutGroup - Constraint: {gridLayoutGroup.constraint}, Count: {gridLayoutGroup.constraintCount}");
            Debug.Log($"InventoryGrid - Rows: {rows}, Columns: {columns}, Total Slots: {slots.Count}");
        }
        else
        {
            Debug.LogWarning("No GridLayoutGroup found on this GameObject!");
        }
    }
}
