using System.Collections;
using System.Collections.Generic;
using General;
using General.Managers;
using General.UnityNetwork;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField] private UnitsStats[] stats;
    [SerializeField] private List<UnitsStats> equippedUnits;
    [SerializeField] private GameObject bloodManager;
    [SerializeField] private Image playerImage;
    private NetworkSpawner _networkSpawner;
    
    [Header("Stats")]
    public NetworkVariable<ushort> playerHealth = new NetworkVariable<ushort>(100);
    public NetworkVariable<int> blood = new NetworkVariable<int>(10);

    public override void OnNetworkSpawn()
    {
        _networkSpawner = NetworkSpawner.Singleton;
        if (IsOwner)
        {
            playerImage.color = Color.white;
            bloodManager.SetActive(true);
            StartCoroutine(SpawnUnitsRoutine());
        }
        else
        {
            playerImage.color = Color.black;
        }
    }

    private IEnumerator SpawnUnitsRoutine()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count > 1);
        for (var i = 0; i < _networkSpawner.GetEquippedUnitCount(OwnerClientId); i++)
        {
            RequestSpawnUnitServerRpc();
            yield return new WaitForSeconds(5f);
        }
    }
    
    [ServerRpc]
    private void RequestSpawnUnitServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        
        if (!_networkSpawner) _networkSpawner = NetworkSpawner.Singleton;
        _networkSpawner.GetAnyUnitServerRpc(clientId, out var unitStat);
        _networkSpawner.SpawnUnitsForPlayer(clientId, unitStat);
    }

}
