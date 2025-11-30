using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace General.UnityNetwork
{
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private List<ushort> equippedItems = new ();
        
        #region Equipment Managment

        public bool HasUnit(ushort unitIndex)
        {
            Debug.Log($"Checking for unit: {unitIndex}: {equippedItems.Contains(unitIndex)}");
            return equippedItems.Contains(unitIndex);
        }

        public void GetUnit(ushort unitIndex)
        {
            Debug.Log($"Getting unit: {unitIndex}");
            equippedItems.Remove(unitIndex);
            //Debug.Log($"Deployed unit: {unitStats.CharacterName}");
        }
        
        public void AddUnit(ushort unitIndex)
        {
            equippedItems.Add(unitIndex);
            //Debug.Log($"Added unit: {unitStats.CharacterName}");
        }
        
        public void ListEquippedUnits()
        {
            string debugMessage = $"Equipped units (index):";
            foreach (var unit in equippedItems)
            {
                debugMessage += $"\n- {equippedItems}";
            }
            Debug.Log(debugMessage);
        }
        
        public void DeleteUnit(ushort unitIndex)
        {
            if (equippedItems.Contains(unitIndex))
            {
                equippedItems.Remove(unitIndex);
                Debug.Log($"Deleted unit: {equippedItems}");
            }
            else
            {
                Debug.LogWarning($"Unit not found: {equippedItems}");
            }
        }

        #endregion
    }
}
