using UnityEngine;

public class GridInventoryItem
{
    public InventoryItem itemData;
    public int posX;
    public int posY;
    
    public GridInventoryItem(InventoryItem item, int x, int y)
    {
        itemData = item;
        posX = x;
        posY = y;
    }
    
    public bool OccupiesCell(int x, int y)
    {
        return x >= posX && x < posX + itemData.width &&
               y >= posY && y < posY + itemData.height;
    }
}

