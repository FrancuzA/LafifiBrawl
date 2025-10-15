using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    [Range(1, 5)]
    public int width = 1;
    [Range(1, 5)]
    public int height = 1;
    public string description;
}

