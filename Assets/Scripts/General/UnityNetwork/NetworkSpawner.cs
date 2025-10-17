using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkSpawner : MonoBehaviour
{
    public static NetworkSpawner Instance { get; private set; }
    
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform mySpawnPoint;
    [SerializeField] private Transform enemySpawnPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void SpawnUnitsForPlayer(ulong playerClientId, UnitsStats stats)
    {
        // Określ pozycję spawnu na podstawie clientId
        // Niższe clientId (host, zazwyczaj 0) spawn'uje na dole (mySpawnPoint)
        // Wyższe clientId (client, zazwyczaj 1) spawn'uje na górze (enemySpawnPoint)
        bool spawnAtLeft = playerClientId == 0;
        
        Transform spawnPoint = spawnAtLeft ? mySpawnPoint : enemySpawnPoint;
        Vector3 spawnPosition = GetRandomSpawnPosition(spawnPoint);
        
        var unit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
        var playerUnit = unit.GetComponent<PlayerUnit>();
        playerUnit.Stats = stats;
        
        var networkObject = unit.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(playerClientId);
        
        // Ustaw kolor po spawnie na wszystkich klientach
        playerUnit.SetColorClientRpc(playerClientId);
    }
    
    private Vector3 GetRandomSpawnPosition(Transform spawnPoint)
    {
        var xOffset = Random.Range(-1f, 1f);
        var yOffset = Random.Range(-2.5f, 2.5f);
        return spawnPoint.position + new Vector3(xOffset, yOffset, 0);
    }
}
