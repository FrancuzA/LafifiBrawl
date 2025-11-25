using System.Collections;
using General;
using General.Managers;
using General.UnityNetwork;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField] private UnitsStats[] stats;
    [SerializeField] private GameObject bloodManager;
    [SerializeField] private Image playerImage;
    private NetworkSpawner _networkSpawner;
    [SerializeField] private PlayerEquipment playerEquipment;
    
    [Header("Stats")]
    public NetworkVariable<ushort> playerHealth = new NetworkVariable<ushort>(100);
    public NetworkVariable<int> blood = new NetworkVariable<int>(10);

    public override void OnNetworkSpawn()
    {
        _networkSpawner = Dependencies.Instance.GetDependency<NetworkSpawner>();
        playerEquipment = Dependencies.Instance.GetDependency<PlayerEquipment>();
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
        for (var i = 0; i < playerEquipment.GetEquippedUnitCount(OwnerClientId); i++)
        {
            Debug.Log($"[LOCAL] Player: {OwnerClientId} Requesting unit spawn from server...");
            RequestSpawnUnitServerRpc();
            yield return new WaitForSeconds(5f);
        }
    }
    
    [ServerRpc]
    private void RequestSpawnUnitServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        
        if (!_networkSpawner) _networkSpawner = Dependencies.Instance.GetDependency<NetworkSpawner>();
        var unitStats = playerEquipment.GetAnyUnit(clientId);
        _networkSpawner.SpawnUnitsForPlayer(clientId, unitStats);
        Debug.Log($"[SERVER] Spawned unit: {unitStats.CharacterName} for Player: {clientId}.");
    }

}
