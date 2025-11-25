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
    private PlayerEquipment _playerEquipment;
    
    [Header("Stats")]
    public NetworkVariable<ushort> playerHealth = new NetworkVariable<ushort>(100);
    public NetworkVariable<int> blood = new NetworkVariable<int>(10);

    public override void OnNetworkSpawn()
    {
        _networkSpawner = Dependencies.Instance.GetDependency<NetworkSpawner>();
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

    public void SetPlayerEq(PlayerEquipment playerEquipment)
    {
        _playerEquipment = playerEquipment;
    }

    private IEnumerator SpawnUnitsRoutine()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count > 1);
        for (var i = 0; i < _playerEquipment.GetEquippedUnitCount(OwnerClientId); i++)
        {
            Debug.Log(i);
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
        var unitStats = _playerEquipment.GetAnyUnit(clientId);
        _networkSpawner.SpawnUnitsForPlayer(clientId, unitStats);
        Debug.Log($"[SERVER] Spawned unit: {unitStats.CharacterName} for Player: {clientId}.");
    }

}
