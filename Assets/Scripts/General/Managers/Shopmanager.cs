using General;
using System.Collections.Generic;
using General.UnityNetwork;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Shopmanager : MonoBehaviour
{
    public List<GameObject> ItemsToBuy = new List<GameObject>();
    public List<Transform> ItemPlacements = new List<Transform>();
    public List<int> ChosenItems = new List<int>();
    public TextMeshProUGUI currentMoneyText;
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
            if (ChosenItems.Count != 0)
            {
                newItemIndex = CheckForDuplicate(newItemIndex);
            }
            ChosenItems.Add(newItemIndex);
            GameObject spawnedItem = Instantiate(ItemsToBuy[newItemIndex], placement.position, Quaternion.identity, placement);
            spawnedItem.GetComponent<DragUIElement>().isBought = false;
        }
        UpdateMoneyCount();
    }

    private int CheckForDuplicate(int newIndex)
    {
        if (ItemsToBuy == null || ItemsToBuy.Count == 0) return newIndex;

        int attempts = 0;
        int maxAttempts = ItemsToBuy.Count * 2;
        while (ChosenItems.Contains(newIndex) && attempts < maxAttempts)
        {
            newIndex = UnityEngine.Random.Range(0, ItemsToBuy.Count);
            attempts++;
        }
        return newIndex;
    }

    public void UpdateMoneyCount()
    {
        currentMoneyText.text = $"Gold: {Money.ToString()}";
    }

    public void Add(LafifiIndex unitIndex)
    {
        NetworkSpawner.Singleton.AddUnitServerRpc(NetworkManager.Singleton.LocalClientId, (ushort)unitIndex);
    }
    
    public void Remove(LafifiIndex unitIndex)
    {
        NetworkSpawner.Singleton.DeleteUnitServerRpc(NetworkManager.Singleton.LocalClientId, (ushort)unitIndex);
    }
}
