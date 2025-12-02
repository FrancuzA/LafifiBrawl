using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace General.UnityNetwork
{
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private List<ushort> equippedItems = new ();
        [SerializeField] private List<PlayerUnit> _deployedUnits = new ();
        
        #region Equipment Managment

        public bool HasUnit(ushort unitIndex)
        {
            return equippedItems.Contains(unitIndex);
        }

        public void GetUnit(ushort unitIndex)
        {
            equippedItems.Remove(unitIndex);
        }
        
        public void AddUnit(ushort unitIndex)
        {
            equippedItems.Add(unitIndex);
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

        #region Deployment

        public void DeployUnit(PlayerUnit unit)
        {
            _deployedUnits.Add(unit);
        }
        
        public void UndeployUnit(PlayerUnit unit)
        {
            _deployedUnits.Remove(unit);
        }
        
        public List<PlayerUnit> GetDeployedUnits()
        {
            return _deployedUnits;
        }

        #endregion
    }
}
