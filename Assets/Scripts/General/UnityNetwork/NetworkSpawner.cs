using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace General.UnityNetwork
{
    public class NetworkSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject unitPrefab;
        [SerializeField] private Transform playerOneSpawnPoint;
        [SerializeField] private Transform PlayerTwoSpawnpoint;

        private void Awake()
        {
            if(Dependencies.Instance.GetDependency<NetworkSpawner>() != null)
            {
                Destroy(gameObject);
                return;
            }
            Dependencies.Instance.RegisterDependency(this);
        }
    
        public void SpawnUnitsForPlayer(NetworkClient playerClient, UnitsStats stats)
        {
            // Określ pozycję spawnu na podstawie clientId
            // Niższe clientId (host, zazwyczaj 0) spawn'uje z lewej (mySpawnPoint)
            // Wyższe clientId (client, zazwyczaj 1) spawn'uje z prawej (enemySpawnPoint)
            bool playerZero = playerClient.ClientId == 0;
        
            Transform spawnPoint = playerZero ? playerOneSpawnPoint : PlayerTwoSpawnpoint;
            Vector3 spawnPosition = GetRandomSpawnPosition(spawnPoint);
        
            var unit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
            var playerUnit = unit.GetComponent<PlayerUnit>();
        
            var unitNetworkObject = unit.GetComponent<NetworkObject>();
            unitNetworkObject.SpawnWithOwnership(playerClient.ClientId);
        
            // Ustaw kolor po spawnie na wszystkich klientach
            playerUnit.SetColorClientRpc(playerClient.ClientId);

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
            var xOffset = Random.Range(-1f, 1f);
            var yOffset = Random.Range(-2.5f, 2.5f);
            return spawnPoint.position + new Vector3(xOffset, yOffset, 0);
        }
    }
}

