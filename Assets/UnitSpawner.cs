using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitSpawner : NetworkBehaviour
{
    public static UnitSpawner Instance;
    
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private UnitsStats[] unitsStats;
    private bool canSpawn = true;

    private IEnumerator SpawnUnits()
    {
        while(canSpawn){
            SpawnUnitsServerRpc();
            yield return new WaitForSeconds(5);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnUnitsServerRpc()
    {
        SpawnUnitClientRpc();
    }
    
    [ClientRpc]
    private void SpawnUnitClientRpc(ClientRpcParams clientRpcParams = default)
    {
        var unit = Instantiate(unitPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        var playerUnit = unit.GetComponent<PlayerUnit>();
        playerUnit.Stats = unitsStats[Random.Range(0, unitsStats.Length)];
        unit.GetComponent<NetworkObject>().Spawn();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!Instance) {
            Instance = this;
        }
        else {
            Destroy(this);
        }
        StartCoroutine(SpawnUnits());
    }
}
