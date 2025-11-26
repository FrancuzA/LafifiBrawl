using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace General.UnityNetwork
{
    public class NetworkSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform playerOneSpawnPoint;
        [SerializeField] private Transform playerTwoSpawnPoint;
        private Dictionary<ulong, List<UnitsStats>> availableUnits = new()
        {
            [0] = new List<UnitsStats>(),
            [1] = new List<UnitsStats>()
        };

        private void Awake()
        {
            if(Dependencies.Instance.GetDependency<NetworkSpawner>())
            {
                Destroy(gameObject);
                return;
            }
            Dependencies.Instance.RegisterDependency(this);
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
                stats.CharacterName, stats.lafifiImg, 
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

        public bool HasUnit(UnitsStats unitStats, ulong clientId)
        {
            return availableUnits[clientId].Contains(unitStats);
        }
        
        public UnitsStats GetUnit(UnitsStats unitStats, ulong clientId)
        {
            availableUnits[clientId].Remove(unitStats);
            Debug.Log($"Deployed unit: {unitStats.CharacterName}");
            return unitStats;
        }
        
        public UnitsStats GetAnyUnit(ulong clientId)
        {
            var unitStats = availableUnits[clientId][0];
            availableUnits[clientId].RemoveAt(0);
            Debug.Log($"Deployed unit: {unitStats.CharacterName}, {GetEquippedUnitCount(clientId)} units left for player {clientId}");
            return unitStats;
        }
        
        public void AddUnit(UnitsStats unitStats, ulong clientId)
        {
            availableUnits[clientId].Add(unitStats);
            Debug.Log($"Added unit: {unitStats.CharacterName}");
        }
        
        public int GetEquippedUnitCount(ulong clientId)
        {
            return availableUnits[clientId].Count;
        }
        
        public void ListEquippedUnits(ulong clientId)
        {
            Debug.Log("Equipped Units:");
            foreach (var unit in availableUnits[clientId])
            {
                Debug.Log($"- {unit.CharacterName}");
            }
        }

        #endregion
    }
}

