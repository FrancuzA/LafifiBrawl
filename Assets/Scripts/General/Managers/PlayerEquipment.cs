using System;
using System.Collections.Generic;
using UnityEngine;

namespace General.Managers
{
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private List<UnitsStats> startingUnits;
        private Dictionary<ulong, List<UnitsStats>> equippedUnits = new();

        private void Start()
        {
            Dependencies.Instance.RegisterDependency(this);
            DontDestroyOnLoad(this);
            equippedUnits.TryAdd(0, startingUnits);
            equippedUnits.TryAdd(1, startingUnits);
        }

        public bool HasUnit(UnitsStats unitStats, ulong clientId)
        {
            return equippedUnits[clientId].Contains(unitStats);
        }
        
        public UnitsStats GetUnit(UnitsStats unitStats, ulong clientId)
        {
            equippedUnits[clientId].Remove(unitStats);
            Debug.Log($"Deployed unit: {unitStats.CharacterName}");
            return unitStats;
        }
        
        public UnitsStats GetAnyUnit(ulong clientId)
        {
            var unitStats = equippedUnits[clientId][0];
            Debug.Log($"Deployed unit: {unitStats.CharacterName}");
            equippedUnits[clientId].RemoveAt(0);
            return unitStats;
        }
        
        public void AddUnit(UnitsStats unitStats, ulong clientId)
        {
            equippedUnits[clientId].Add(unitStats);
            Debug.Log($"Added unit: {unitStats.CharacterName}");
        }
        
        public int GetEquippedUnitCount(ulong clientId)
        {
            return equippedUnits.Count;
        }
        
        public void ListEquippedUnits(ulong clientId)
        {
            Debug.Log("Equipped Units:");
            foreach (var unit in equippedUnits[clientId])
            {
                Debug.Log($"- {unit.CharacterName}");
            }
        }
    }
}
