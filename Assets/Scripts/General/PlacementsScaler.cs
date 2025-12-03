using System;
using UnityEngine;
using UnityEngine.UI;

public class PlacementsScaler : MonoBehaviour
{
    [SerializeField] private RectTransform placementsTransform;
    [SerializeField] private GridLayoutGroup placementsGrid;
    [SerializeField] private int heightMultiplier;
    [SerializeField] private int widthMultiplier;
    
    
    private void OnEnable()
    {
        if (placementsGrid)
        {
            placementsGrid.cellSize = new Vector2(placementsTransform.sizeDelta.x / 3f, placementsTransform.sizeDelta.y);
        }
    }
}
