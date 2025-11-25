using System;
using System.Collections.Generic;
using UnityEngine;

namespace General.Managers
{
    public class PlayerEquipment : MonoBehaviour
    {
        [SerializeField] private List<UnitsStats> startingUnits;
        private Dictionary<ulong, List<UnitsStats>> equippedUnits = new();

        public void Setup()
        {
            DontDestroyOnLoad(this);
            equippedUnits.TryAdd(0, new List<UnitsStats>());
            equippedUnits.TryAdd(1, new List<UnitsStats>());
            foreach (var unit in startingUnits)
            {
                equippedUnits[0].Add(unit);
                equippedUnits[1].Add(unit);
            }

            Debug.Log($"PLayer 0 has {GetEquippedUnitCount(0)} units");
            Debug.Log($"PLayer 1 has {GetEquippedUnitCount(1)} units");
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
            equippedUnits[clientId].RemoveAt(0);
            Debug.Log($"Deployed unit: {unitStats.CharacterName}, {GetEquippedUnitCount(clientId)} units left for player {clientId}");
            return unitStats;
        }
        
        public void AddUnit(UnitsStats unitStats, ulong clientId)
        {
            equippedUnits[clientId].Add(unitStats);
            Debug.Log($"Added unit: {unitStats.CharacterName}");
        }
        
        public int GetEquippedUnitCount(ulong clientId)
        {
            return equippedUnits[clientId].Count;
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
