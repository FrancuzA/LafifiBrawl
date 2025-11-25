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

        private void Awake()
        {
            if(Dependencies.Instance.GetDependency<NetworkSpawner>())
            {
                Destroy(gameObject);
                return;
            }
            Dependencies.Instance.RegisterDependency(this);
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
    }
}

