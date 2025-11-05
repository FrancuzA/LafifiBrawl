using General;
using System.Collections.Generic;
using UnityEngine;

public class Shopmanager : MonoBehaviour
{
    public List<GameObject> ItemsToBuy = new List<GameObject>();
    public List<Transform> ItemPlacements = new List<Transform>();
    public List<int> ChosenItems = new List<int>();
    public int Money = 20;
    public bool killSwitch;

    private void Start()
    {
        InitializeShop();
    }

    private void InitializeShop()
    {
        Dependencies.Instance.RegisterDependency<Shopmanager>(this);
        ChosenItems.Clear();
        foreach (Transform placement in ItemPlacements)
        {
            int newItemIndex = UnityEngine.Random.Range(0, ItemsToBuy.Count);
            Debug.Log("First Index" + newItemIndex);
            if (ChosenItems.Count != 0)
            {
                newItemIndex = CheckForDuplicate(newItemIndex);
            }
            Debug.Log("chosen " + newItemIndex);
            ChosenItems.Add(newItemIndex);
            GameObject spawnedItem = Instantiate(ItemsToBuy[newItemIndex], placement.position, Quaternion.identity, placement);
            spawnedItem.GetComponent<DragUIElement>().isBought = false;
        }
    }

    private int CheckForDuplicate(int newIndex)
    {
        if (ItemsToBuy == null || ItemsToBuy.Count == 0) return newIndex;

        int attempts = 0;
        int maxAttempts = ItemsToBuy.Count * 2;
        while (ChosenItems.Contains(newIndex) && attempts < maxAttempts)
        {
            newIndex = UnityEngine.Random.Range(0, ItemsToBuy.Count);
            Debug.Log("Rechosen index " + newIndex);
            attempts++;
        }
        return newIndex;
    }
}
