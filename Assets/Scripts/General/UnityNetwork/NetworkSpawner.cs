using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace General.UnityNetwork
{
    public class NetworkSpawner : MonoBehaviour
    {
        public static NetworkSpawner Singleton;
        
        [Tooltip("Belly = 0,\nGrzegorz = 1,\nKon = 2,\nLafifi = 3,\nAngelika = 4,\nRat = 5")]
        [SerializeField] private UnitsStats[] unitsStats;
        
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform playerOneSpawnPoint;
        [SerializeField] private Transform playerTwoSpawnPoint;

        private Dictionary<ulong, List<UnitsStats>> playerStats = new(){
            {0, new List<UnitsStats>()},
            {1, new List<UnitsStats>()}
        };
        

        private void Awake()
        {
            if(Singleton)
            {
                Destroy(gameObject);
                return;
            }
            Singleton = this;
        }
    
        public void SpawnUnitsForPlayer(ulong clientId, ushort statsIndex)
        {
            
            ListEquippedUnits(0);
            ListEquippedUnits(1);
            
            var stats = unitsStats[statsIndex]; 
            
            if (!HasUnit(clientId, statsIndex)) return;
            GetUnitServerRc(clientId, statsIndex);
            
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

        private bool HasUnit(ulong clientId, ushort unitIndex)
        {
            var unitStats = unitsStats[unitIndex];
            
            return playerStats[clientId].Contains(unitStats);
        }

        [ServerRpc (RequireOwnership = false)]
        private void GetUnitServerRc(ulong clientId, ushort unitIndex)
        {
            var unitStats = unitsStats[unitIndex];
            playerStats[clientId].Remove(unitStats);
            //Debug.Log($"Deployed unit: {unitStats.CharacterName}");
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AddUnitServerRpc(ulong clientId, ushort unitIndex)
        {
            var unitStats = unitsStats[unitIndex];
            playerStats[clientId].Add(unitStats);
            //Debug.Log($"Added unit: {unitStats.CharacterName}");
        }
        
        public int GetEquippedUnitCount(ulong clientId)
        {
            var unitStats = playerStats[clientId];
            return unitStats.Count;
        }
        
        public void ListEquippedUnits(ulong clientId)
        {
            string debugMessage = $"Client {clientId} has the following equipped units:";
            foreach (var unit in playerStats[clientId])
            {
                debugMessage += $"\n- {unit.CharacterName}";
            }
            Debug.Log(debugMessage);
        }
        
        public void DeleteUnit(ulong clientId, ushort unitIndex)
        {
            var unitStats = unitsStats[unitIndex];
            if (playerStats[clientId].Contains(unitStats))
            {
                playerStats[clientId].Remove(unitStats);
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



