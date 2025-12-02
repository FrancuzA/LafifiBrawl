using System;
using UnityEngine;
using UnityEngine.UI;

public class LayoutScaler : MonoBehaviour
{
    [SerializeField] private bool isPlacement;
    [SerializeField] private GridLayoutGroup InventoryUI;
    [SerializeField] private RectTransform RTransform;
    [SerializeField] private int HeightMultip;
    [SerializeField] private int WidthMultip;
    private DragUIElement _dragUI;
    private const float Rows = 8f;

    void OnEnable()
    {
        Scale();
        TryGetComponent(out _dragUI);
    }

    void OnValidate()
    {
        /*TryGetComponent(out _dragUI);
        Scale();*/
    }
    
    private void Scale()
    {
        var cellSize = new Vector2(Screen.width / Rows, Screen.width / Rows);
        if (RTransform) 
        {
            RTransform.sizeDelta = new Vector2(cellSize.x * WidthMultip, cellSize.y * HeightMultip);
        }

        if (_dragUI)
        {
            _dragUI.centerOffsetValue = new Vector2(-_dragUI.cellCountOffset.x * cellSize.x, -_dragUI.cellCountOffset.y * cellSize.y);
        }
        
        if (isPlacement) cellSize = new Vector2(Screen.width * 0.7f / Rows, Screen.width * 0.7f / Rows);
        
        if (InventoryUI)
        {
            InventoryUI.cellSize = cellSize;
            if(isPlacement)
            {
                InventoryUI.spacing = new Vector2(Screen.width * 0.3f / Rows, Screen.width * 0.3f / Rows);
            }
        }
    }
}
