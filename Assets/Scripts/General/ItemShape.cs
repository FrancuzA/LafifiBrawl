using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemShape
{
    public List<Vector2Int> occupiedCells = new List<Vector2Int>();
    
    public ItemShape()
    {
        // Domyślnie obiekt zajmuje jedną komórkę (0,0)
        if (occupiedCells.Count == 0)
        {
            occupiedCells.Add(Vector2Int.zero);
        }
    }
    
    public ItemShape(List<Vector2Int> cells)
    {
        occupiedCells = new List<Vector2Int>(cells);
    }
    
    public ItemShape Rotate90CW()
    {
        var rotated = new ItemShape();
        rotated.occupiedCells.Clear();
        
        foreach (var cell in occupiedCells)
        {
            rotated.occupiedCells.Add(new Vector2Int(-cell.y, cell.x));
        }
        
        return rotated;
    }
    
    public ItemShape Rotate90CCW()
    {
        var rotated = new ItemShape();
        rotated.occupiedCells.Clear();
        
        foreach (var cell in occupiedCells)
        {
            rotated.occupiedCells.Add(new Vector2Int(cell.y, -cell.x));
        }
        
        return rotated;
    }
}
