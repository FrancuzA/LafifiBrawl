using System.Collections;
using General;
using General.UnityNetwork;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private UnitsStats[] stats;
    [SerializeField] private GameObject bloodManager;
    private SpriteRenderer spriteRenderer;
    private bool isSpawning = true;
    private NetworkSpawner networkSpawner;
    
    [Header("Stats")]
    public NetworkVariable<ushort> playerHealth = new NetworkVariable<ushort>(100);
    public NetworkVariable<int> blood = new NetworkVariable<int>(10);

    public override void OnNetworkSpawn()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        networkSpawner = Dependencies.Instance.GetDependency<NetworkSpawner>();
        if (IsOwner)
        {
            //transform.position = new Vector3(0, -3.8f, 0);
            spriteRenderer.color = Color.white;
            bloodManager.SetActive(true);
            StartCoroutine(SpawnUnitsRoutine());
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
        var wait = new WaitForSeconds(1f);
        for (var i = 0; i < 50; i++)
        {
            SpawnUnitServerRpc();
            yield return wait;
        }
        
    }
    
    [ServerRpc]
    private void SpawnUnitServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        var client = NetworkManager.Singleton.ConnectedClients[clientId];
        if (!networkSpawner) networkSpawner = Dependencies.Instance.GetDependency<NetworkSpawner>();
        networkSpawner.SpawnUnitsForPlayer(client, stats[Random.Range(0, stats.Length)]);

    }

}
