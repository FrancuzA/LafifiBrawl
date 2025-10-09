using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{
    private GridLayoutGroup _gridLayout;
    public GameObject invSlot;
    public GridShape myGrid;

    private void Start()
    {
        TryGetComponent(out _gridLayout);
        myGrid = myGrid.CreateGrid();
        
        foreach (var slot in myGrid.gridCells)
        {
            Instantiate(invSlot, gameObject.transform);
        }

        if (_gridLayout)
        {
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            _gridLayout.constraintCount = myGrid.size.x;
        }
    }
    
}
