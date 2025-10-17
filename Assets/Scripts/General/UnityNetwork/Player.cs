using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private UnitsStats[] stats;
    private SpriteRenderer spriteRenderer;
    private bool isSpawning = true;
    
    [Header("Stats")]
    public NetworkVariable<ushort> playerHealth = new NetworkVariable<ushort>(100);
    public NetworkVariable<ushort> mana = new NetworkVariable<ushort>(100);

    public override void OnNetworkSpawn()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (IsOwner)
        {
            transform.position = new Vector3(0, -3.8f, 0);
            spriteRenderer.color = Color.white;
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
            spriteRenderer.color = Color.black;
        }
    }

    private IEnumerator SpawnUnitsRoutine()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count > 1);
        for (int i = 0; i < 50; i++)
        {
            SpawnUnitServerRpc();
            yield return new WaitForSeconds(0.05f);
        }
        
    }
    
    /*[ClientRpc]
    public void AddUnitToListClientRpc(ulong ownerClientId, GameObject unitNetworkObject)
    {
        if (NetworkSpawner.Instance == null) return;
        if (ownerClientId == NetworkManager.Singleton.LocalClientId)
        {
            NetworkSpawner.Instance.myUnits.Add(unitNetworkObject.gameObject);
        }
        else
        {
            NetworkSpawner.Instance.enemyUnits.Add(unitNetworkObject.gameObject);
        }
    }*/
    
    [ServerRpc]
    private void SpawnUnitServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        var client = NetworkManager.Singleton.ConnectedClients[clientId];
        if (NetworkSpawner.Instance != null)
        {
            NetworkSpawner.Instance.SpawnUnitsForPlayer(client, stats[Random.Range(0, stats.Length)]);
        }
        
    }

}
