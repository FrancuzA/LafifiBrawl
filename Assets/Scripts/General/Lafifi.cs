using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(DragableItem))]
public class Lafifi : MonoBehaviour
{
    [SerializeField] private LafifiSO lafifiData;
    private Image lafifiImage;
    
    [HideInInspector] public List<int> occupiedSlots = new List<int>();
    
    private void Start()
    {
        lafifiImage = GetComponent<Image>();
        if (lafifiData && lafifiImage)
        {
            lafifiImage.sprite = lafifiData.lafifiSprite;
        }
        else
        {
            Debug.LogWarning("Lafifi data or image component is missing.", this);
        }
    }
    
    public ItemShape GetItemShape()
    {
        return lafifiData?.lafifiShape;
    }
    
    public LafifiSO GetLafifiData()
    {
        return lafifiData;
    }
    
    public void SetLafifiData(LafifiSO data)
    {
        lafifiData = data;
        if (lafifiData && lafifiImage)
        {
            lafifiImage.sprite = lafifiData.lafifiSprite;
        }
    }
}
