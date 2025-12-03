using System.Collections.Generic;
using General.UnityNetwork;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyStage : NetworkBehaviour
{
    public NetworkVariable<bool> player1Ready = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> player2Ready = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private NetworkObject playerPrefab;

    [SerializeField] private GameObject[] killZones;

    [ItemCanBeNull] private List<Player> _playerInstances = new();

    private void Start()
    {
        player1Ready.OnValueChanged += OnPlayerReadyChanged;
        player2Ready.OnValueChanged += OnPlayerReadyChanged;
        DontDestroyOnLoad(this);
    }
    
    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        switch (clientId)
        {
            case 0:
                player1Ready.Value = true;
                break;
            case 1:
                player2Ready.Value = true;
                break;
        }
    }
    
    private void OnPlayerReadyChanged(bool previousValue, bool newValue)
    {
        if (!player1Ready.Value || !player2Ready.Value || !IsServer) return;
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadComplete;
        StartGame();
    }
    
    private void StartGame()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("FightStage", LoadSceneMode.Single);
    }

    private void SceneManager_OnLoadComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Debug.Log("Scene Loaded for all clients");
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var clientId = client.ClientId;
            var player = Instantiate(playerPrefab);
            _playerInstances.Add(player.GetComponent<Player>());
            player.SpawnWithOwnership(clientId);
        }
        SpawnZonesClientRpc();
        
        player1Ready.OnValueChanged -= OnPlayerReadyChanged;
        player2Ready.OnValueChanged -= OnPlayerReadyChanged;
        //NetworkObject.Despawn();
    }

    [ClientRpc]
    private void SpawnZonesClientRpc()
    {
        Instantiate(killZones[0], transform).GetComponent<KillZone>().readyStage = this;
        Instantiate(killZones[1], transform).GetComponent<KillZone>().readyStage = this;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void DespawnUnitAndDealDamageServerRpc(NetworkBehaviourReference unitRef, ushort unitIndex, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log(clientId);
        _playerInstances[(int)clientId]?.TakeDamageClientRpc(10f);
        if (!IsServer) return;
        if (!unitRef.TryGet(out var unit)) return;
        
        NetworkSpawner.Singleton.AddUnitServerRpc(unit.OwnerClientId, unitIndex);
        unit.NetworkObject.Despawn();
    }
}
