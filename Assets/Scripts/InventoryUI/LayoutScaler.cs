using UnityEngine;
using UnityEngine.UI;

public class LayoutScaler : MonoBehaviour
{
    public GridLayoutGroup InventoryUI;
    public RectTransform RTransform;
    public int HeightMultip;
    public int WidthMultip;
    
    void OnEnable()
    {
        if (InventoryUI)
        {
            InventoryUI.cellSize = new Vector2(Screen.width, Screen.width) / 8;
        }
        if (RTransform) 
        {
            var CellSize = new Vector2(Screen.width, Screen.width) / 8;
            RTransform.localScale = new Vector2(CellSize.x * WidthMultip, CellSize.y * HeightMultip);
        }

    }

}
