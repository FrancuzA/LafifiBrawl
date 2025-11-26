using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace General.UnityNetwork
{
    public class NetworkSpawner : NetworkBehaviour
    {
        public static NetworkSpawner Singleton;
        
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform playerOneSpawnPoint;
        [SerializeField] private Transform playerTwoSpawnPoint;

        private List<UnitsStats> playerOneStats = new();
        private List<UnitsStats> playerTwoStats = new();
        

        private void Awake()
        {
            if(Singleton)
            {
                Destroy(gameObject);
                return;
            }
            Singleton = this;
            DontDestroyOnLoad(this);
        }
    
        public void SpawnUnitsForPlayer(ulong clientId, UnitsStats stats)
        {
            bool playerZero = clientId == 0;
        
            Transform spawnPoint = playerZero ? playerOneSpawnPoint : playerTwoSpawnPoint;
            Vector3 spawnPosition = GetRandomSpawnPosition(spawnPoint);
        
            var unit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        
            var playerUnit = unit.GetComponent<PlayerUnit>();
            var unitNetworkObject = unit.GetComponent<NetworkObject>();
            
            unitNetworkObject.SpawnWithOwnership(clientId);
        
            // Ustaw kolor po spawnie na wszystkich klientach
            playerUnit.SetColorClientRpc(clientId);

            // Ustaw statystyki jednostki
            playerUnit.SetStatsClientRpc(
                stats.CharacterName,
                stats.lafifiImg, 
                stats.MaxHealthPoints,
                stats.AttackDMG,
                stats.AttackSpd,
                stats.Ult,
                stats.UltCD,
                stats.UltCost);

            playerUnit.SetStartHealthPointsClientRpc();
        }
    
        private Vector3 GetRandomSpawnPosition(Transform spawnPoint)
        {
            var xOffset = Random.Range(-20f, 10f);
            var yOffset = Random.Range(-25f, 25f);
            return spawnPoint.position + new Vector3(xOffset, yOffset, 0);
        }

        #region Equipment Managment

        public bool HasUnit(ulong clientId, UnitsStats unitStats)
        {
            var availableUnits = clientId == 0 ? playerOneStats : playerTwoStats;
            return availableUnits.Contains(unitStats);
        }
        
        public void GetUnit(ulong clientId, UnitsStats unitStats, out UnitsStats unitStat)
        {
            var availableUnits = clientId == 0 ? playerOneStats : playerTwoStats;
            unitStat = unitStats;
            availableUnits.Remove(unitStats);
            Debug.Log($"Deployed unit: {unitStats.CharacterName}");
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void GetAnyUnitServerRpc(ulong clientId, out UnitsStats unitStats)
        {
            var availableUnits = clientId == 0 ? playerOneStats : playerTwoStats;
            unitStats = availableUnits[0];
            availableUnits.RemoveAt(0);
            Debug.Log($"Deployed unit: {unitStats.CharacterName}, {GetEquippedUnitCount(clientId)} units left for player {clientId}");
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AddUnitServerRpc(ulong clientId, UnitsStats unitStats)
        {
            var availableUnits = clientId == 0 ? playerOneStats : playerTwoStats;
            availableUnits.Add(unitStats);
            Debug.Log($"Added unit: {unitStats.CharacterName}");
        }
        
        public int GetEquippedUnitCount(ulong clientId)
        {
            var availableUnits = clientId == 0 ? playerOneStats : playerTwoStats;
            return availableUnits.Count;
        }
        
        public void ListEquippedUnits(ulong clientId)
        {
            var availableUnits = clientId == 0 ? playerOneStats : playerTwoStats;
            Debug.Log("Equipped Units:");
            foreach (var unit in availableUnits)
            {
                Debug.Log($"- {unit.CharacterName}");
            }
        }
        
        public void DeleteUnit(ulong clientId, UnitsStats unitStats)
        {
            var availableUnits = clientId == 0 ? playerOneStats : playerTwoStats;
            if (availableUnits.Contains(unitStats))
            {
                availableUnits.Remove(unitStats);
                Debug.Log($"Deleted unit: {unitStats.CharacterName}");
            }
            else
            {
                Debug.LogWarning($"Unit not found: {unitStats.CharacterName}");
            }
        }

        #endregion

    }
}



