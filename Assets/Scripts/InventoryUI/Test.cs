using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    
    private void Start()
    {
        var grid = new Grid(width, height, cellSize);
    }

}
