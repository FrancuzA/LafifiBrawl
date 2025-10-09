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
    
    // Metody pomocnicze do tworzenia popularnych kształtów
    public static ItemShape CreateRectangle(int width, int height)
    {
        var shape = new ItemShape();
        shape.occupiedCells.Clear();
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                shape.occupiedCells.Add(new Vector2Int(x, y));
            }
        }
        
        return shape;
    }
    
    public static ItemShape CreateSingleCell()
    {
        return new ItemShape();
    }
}
