using System;
using System.Collections.Generic;
using UnityEngine;

namespace General.Managers
{
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private List<UnitsStats> equippedUnits = new();

        private void Start()
        {
            Dependencies.Instance.RegisterDependency(this);
            DontDestroyOnLoad(this);
        }

        public bool HasUnit(UnitsStats unitStats)
        {
            return equippedUnits.Contains(unitStats);
        }
        
        public UnitsStats GetUnit(UnitsStats unitStats)
        {
            equippedUnits.Remove(unitStats);
            Debug.Log($"Deployed unit: {unitStats.CharacterName}");
            return unitStats;
        }
        
        public UnitsStats GetAnyUnit()
        {
            if (equippedUnits.Count == 0)
            {
                Debug.Log("No units to deploy.");
                return null;
            }
            var unitStats = equippedUnits[0];
            equippedUnits.RemoveAt(0);
            Debug.Log($"Deployed unit: {unitStats.CharacterName}");
            return unitStats;
        }
        
        public void AddUnit(UnitsStats unitStats)
        {
            equippedUnits.Add(unitStats);
            Debug.Log($"Added unit: {unitStats.CharacterName}");
        }
        
        public int GetEquippedUnitCount()
        {
            return equippedUnits.Count;
        }
        
        public void ListEquippedUnits()
        {
            Debug.Log("Equipped Units:");
            foreach (var unit in equippedUnits)
            {
                Debug.Log($"- {unit.CharacterName}");
            }
        }
    }
}
