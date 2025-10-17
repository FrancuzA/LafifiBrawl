using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private UnitsStats[] stats;
    private bool isSpawning = true;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            transform.position = new Vector3(0, -3.8f, 0);
            StartCoroutine(SpawnUnitsRoutine());
            // TYLKO HOST SPAWN'UJE JEDNOSTKI - to zapobiega duplikowaniu
            /*if (IsServer && NetworkSpawner.Instance != null)
            {
                StartCoroutine(SpawnUnitsRoutine());
            }*/
        }
        else
        {
            transform.position = new Vector3(0, 3.8f, 0);
        }
    }

    private IEnumerator SpawnUnitsRoutine()
    {
        //yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count > 1);
        for (int i = 0; i < 50; i++)
        {
            SpawnUnitServerRpc();
            yield return new WaitForSeconds(0.05f);
        }
        
    }
    
    [ServerRpc]
    private void SpawnUnitServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        if (NetworkSpawner.Instance != null)
        {
            NetworkSpawner.Instance.SpawnUnitsForPlayer(clientId, stats[0]);
        }
        
    }
}
