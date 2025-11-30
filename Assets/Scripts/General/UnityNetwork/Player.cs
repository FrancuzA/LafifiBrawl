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
    //Belly = 0
    //Grzegorz = 1
    //Kon = 2
    //Lafifi = 3
    //nAngelika = 4
    //Rat = 5
    
    [SerializeField] private List<UnitsStats> equippedUnits;
    [SerializeField] private GameObject bloodManager;
    [SerializeField] private Image playerImage;
    [SerializeField] private GameObject spawnButtons;
    private NetworkSpawner _networkSpawner;
    
    [Header("Stats")]
    public NetworkVariable<ushort> playerHealth = new NetworkVariable<ushort>(100);
    public NetworkVariable<int> blood = new NetworkVariable<int>(10);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            playerImage.color = Color.white;
            bloodManager.SetActive(true);
            spawnButtons.SetActive(true);
        }
        else
        {
            spawnButtons.SetActive(false);
            playerImage.color = Color.black;
        }
    }
    
    public void SpawnUnitBelly()
    {
        RequestSpawnUnitServerRpc(0);
    }
    
    public void SpawnUnitGrzegorz()
    {
        RequestSpawnUnitServerRpc(1);
    }
    
    public void SpawnUnitKon()
    {
        RequestSpawnUnitServerRpc(2);
    }
    
    public void SpawnUnitLafifi()
    {
        RequestSpawnUnitServerRpc(3);
    }
    
    public void SpawnUnitAngelika()
    {
        RequestSpawnUnitServerRpc(4);
    }
    
    public void SpawnUnitRat()
    {
        RequestSpawnUnitServerRpc(5);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnUnitServerRpc(ushort unitStatIndex, ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        
        NetworkSpawner.Singleton.SpawnUnitsForPlayer(clientId, unitStatIndex);
    }

}
