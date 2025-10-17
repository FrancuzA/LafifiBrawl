using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkSpawner : MonoBehaviour
{
    public static NetworkSpawner Instance { get; private set; }
    
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform playerOneSpawnPoint;
    [SerializeField] private Transform PlayerTwoSpawnpoint;
    
    public List<GameObject> myUnits = new();
    public List<GameObject> enemyUnits = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void SpawnUnitsForPlayer(NetworkClient playerClient, UnitsStats stats)
    {
        // Określ pozycję spawnu na podstawie clientId
        // Niższe clientId (host, zazwyczaj 0) spawn'uje na dole (mySpawnPoint)
        // Wyższe clientId (client, zazwyczaj 1) spawn'uje na górze (enemySpawnPoint)
        bool playerZero = playerClient.ClientId == 0;
        
        Transform spawnPoint = playerZero ? playerOneSpawnPoint : PlayerTwoSpawnpoint;
        Vector3 spawnPosition = GetRandomSpawnPosition(spawnPoint);
        
        var unit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        var playerUnit = unit.GetComponent<PlayerUnit>();
        
        var unitNetworkObject = unit.GetComponent<NetworkObject>();
        unitNetworkObject.SpawnWithOwnership(playerClient.ClientId);
        
        // Ustaw kolor po spawnie na wszystkich klientach
        playerUnit.SetColorClientRpc(playerClient.ClientId);
        playerUnit.SetStatsClientRpc(
            stats.CharacterName, stats.lafifiImg, 
            stats.MaxHealthPoints,
            stats.AttackDMG,
            stats.AttackSpd,
            stats.Ult,
            stats.UltCD,
            stats.UltCost);
    }
    
    private Vector3 GetRandomSpawnPosition(Transform spawnPoint)
    {
        var xOffset = Random.Range(-1f, 1f);
        var yOffset = Random.Range(-2.5f, 2.5f);
        return spawnPoint.position + new Vector3(xOffset, yOffset, 0);
    }
}
