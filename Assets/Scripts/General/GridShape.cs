using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridShape
{
    public Vector2Int size;
    public List<Dictionary<Vector2Int, bool>> gridCells = new List<Dictionary<Vector2Int, bool>>();
    
    public GridShape()
    {
        if (gridCells.Count == 0)
        {
            ClearCells();
            gridCells.Add(new Dictionary<Vector2Int, bool> { { new Vector2Int(0, 0), false } });
        }
    }
    
    public GridShape CreateGrid() {
        var grid = new GridShape();
        grid.ClearCells();
        grid.size = size;
        for (var i = 0; i < size.y; i++)
        {
            for (var j = 0; j < size.x; j++)
            {
                grid.gridCells.Add(new Dictionary<Vector2Int, bool> { { new Vector2Int(j, i), false } });
            }
        }
        return grid;
    }

    public void ClearCells()
    {
        gridCells.Clear();
    }
}
