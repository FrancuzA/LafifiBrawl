using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace General.UnityNetwork
{
    public class NetworkSpawner : NetworkBehaviour
    {
        public static NetworkSpawner Singleton;
        
        [Tooltip("Belly = 0,\nGrzegorz = 1,\nKon = 2,\nLafifi = 3,\nAngelika = 4,\nRat = 5")]
        [SerializeField] private UnitsStats[] unitsStats;
        
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform playerOneSpawnPoint;
        [SerializeField] private Transform playerTwoSpawnPoint;

        public List<PlayerEquipment> player;

        private void Awake()
        {
            if(Singleton)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(this);
            Singleton = this;
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        public void SpawnUnitsForPlayerServerRpc(ulong clientId, ushort unitIndex, NetworkBehaviourReference netRef)
        {
            var stats = unitsStats[unitIndex]; 
            if (!player[(int)clientId].HasUnit(unitIndex)) return;
            player[(int)clientId].GetUnit(unitIndex);
            if(netRef.TryGet(out Player playerScript))
            {
                playerScript.RemoveBloodClientRpc(stats.UltCost);
            }
            
            bool playerZero = clientId == 0;
        
            Transform spawnPoint = playerZero ? playerOneSpawnPoint : playerTwoSpawnPoint;
            Vector3 spawnPosition = GetRandomSpawnPosition(spawnPoint);
        
            var unit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        
            var playerUnit = unit.GetComponent<PlayerUnit>();
            playerUnit.spawner = this;
            player[(int)clientId].DeployUnit(playerUnit);
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
            var xOffset = Random.Range(-5f, 5f);
            var yOffset = Random.Range(-25f, 25f);
            return spawnPoint.position + new Vector3(xOffset, yOffset, 0);
        }

        /*[ServerRpc(RequireOwnership = false)]
        public void AngelikaUltServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            foreach (var unit in player[(int)clientId].GetDeployedUnits())
            {
                unit.HealUnitServerRpc(5);
            }
        }*/
        
        [ServerRpc(RequireOwnership = false)]
        public void AddUnitServerRpc(ulong clientId, ushort unitIndex)
        {
            player[(int)clientId].AddUnit(unitIndex);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void DeleteUnitServerRpc(ulong clientId, ushort unitIndex)
        {
            player[(int)clientId].DeleteUnit(unitIndex);
        }

        /*public void ClearDeployedUnitServerRpc(NetworkBehaviourReference netRef, ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            netRef.TryGet(out PlayerUnit unit);
            player[(int)clientId].UndeployUnit(unit);
        }*/
    }
}



