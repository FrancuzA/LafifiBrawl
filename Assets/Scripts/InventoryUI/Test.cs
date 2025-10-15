using System;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private InventoryGrid inventoryGrid;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private InventoryItem[] testItems; // Przypisz różne itemy w inspektorze
    private int currentItemIndex = 0;
    
    private void Start()
    {
        if (inventoryGrid == null)
        {
            inventoryGrid = GetComponent<InventoryGrid>();
        }
        
        if (inventoryUI == null)
        {
            inventoryUI = GetComponent<InventoryUI>();
        }
    }

    private void Update()
    {
        // Lewy przycisk myszy - dodaj item
        if (Input.GetMouseButtonDown(0) && testItems != null && testItems.Length > 0) 
        {
            InventoryItem itemToPlace = testItems[currentItemIndex % testItems.Length];
            bool placed = inventoryGrid.PlaceItemFromScreen(itemToPlace, Input.mousePosition);
            
            if (placed && inventoryUI != null)
            {
                inventoryUI.RefreshAllItems();
            }
        }
        
        // Prawy przycisk myszy - usuń item
        if (Input.GetMouseButtonDown(1)) 
        {
            bool removed = inventoryGrid.RemoveItemAt(Input.mousePosition);
            
            if (removed && inventoryUI != null)
            {
                inventoryUI.RefreshAllItems();
            }
        }
        
        // Spacja - zmień aktualny item do umieszczenia
        if (Input.GetKeyDown(KeyCode.Space) && testItems != null && testItems.Length > 0)
        {
            currentItemIndex++;
            Debug.Log($"Current item: {testItems[currentItemIndex % testItems.Length].itemName}");
        }
    }
}
