using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject[] unitPrefabs;
    [SerializeField] private Transform[] spawnPoints;
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
        var unit = Instantiate(unitPrefabs[Random.Range(0, unitPrefabs.Length - 1)], spawnPoints[Random.Range(0, spawnPoints.Length - 1)].position, Quaternion.identity);
        unit.GetComponent<NetworkObject>().Spawn();
    }

    public override void OnNetworkSpawn()
    {
        if(IsOwner) StartCoroutine(SpawnUnits());
    }
}
