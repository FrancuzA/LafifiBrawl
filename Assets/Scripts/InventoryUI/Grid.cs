using System;
using UnityEngine;

public class Grid <TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridArray;
    private Vector3 originPosition;
    
    public Grid(int width, int height, float cellSize, Vector3 originPosition, Func<TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        
        gridArray = new TGridObject[width, height];
        
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x, y] = createGridObject();
            }
        }
        
        /*for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }
        
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);*/
    }
    
    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }
    
    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }
    
    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            gridArray[x, y] = value;
        }
    }
    
    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        GetXY(worldPosition, out var x, out var y);
        SetGridObject(x, y, value);
    }
    
    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return gridArray[x, y];
        }
        return default;
    }
    
    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        GetXY(worldPosition, out var x, out var y);
        return GetGridObject(x, y);
    }
    
}
