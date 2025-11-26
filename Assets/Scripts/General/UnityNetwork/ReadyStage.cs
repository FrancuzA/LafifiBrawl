using System;
using System.Collections.Generic;
using General.Managers;
using General.UnityNetwork;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyStage : NetworkBehaviour
{
    public NetworkVariable<bool> player1Ready = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> player2Ready = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    [SerializeField] private NetworkObject playerOne;
    [SerializeField] private NetworkObject playerTwo;

    [SerializeField] private NetworkSpawner networkSpawner;
    
    [SerializeField] private UnitsStats unitsStats;

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

    public void AddUnitButton()
    {
        AddUnitServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void AddUnitServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        for(var i = 0; i < 7; i++)
            networkSpawner.AddUnit(unitsStats, clientId);
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
            var player = Instantiate(clientId == 0 ? playerOne : playerTwo, transform);
            player.SpawnAsPlayerObject(clientId, true);
        }
        
        player1Ready.OnValueChanged -= OnPlayerReadyChanged;
        player2Ready.OnValueChanged -= OnPlayerReadyChanged;
        //NetworkObject.Despawn();
    }
}
