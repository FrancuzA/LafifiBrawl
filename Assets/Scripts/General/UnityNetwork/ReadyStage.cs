using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General.UnityNetwork
{
    public class ReadyStage : NetworkBehaviour
    {
        public NetworkVariable<bool> player1Ready = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> player2Ready = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        
        [SerializeField] private NetworkObject playerOne;
        [SerializeField] private NetworkObject playerTwo;
        
        [SerializeField] private GameObject playerPrefab;


        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            player1Ready.OnValueChanged += OnPlayerReadyChanged;
            player2Ready.OnValueChanged += OnPlayerReadyChanged;
        }

        private void OnPlayerReadyChanged(bool previousValue, bool newValue)
        {
            if(player1Ready.Value && player2Ready.Value)
            {
                Debug.Log("Both players are ready!");
                if (!IsServer) return;
                NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadComplete;
                StartGame();
            }
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
        }

        private void StartGame()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("FightStage", LoadSceneMode.Single);
            
            player1Ready.OnValueChanged -= OnPlayerReadyChanged;
            player2Ready.OnValueChanged -= OnPlayerReadyChanged;
        }

        public void SetPlayerReady()
        {
            SetPlayerReadyServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;
            if(clientId == 0)
            {
                player1Ready.Value = true;
                Debug.Log("Player 1 is ready");
            }
            else if(clientId == 1)
            {
                player2Ready.Value = true;
                Debug.Log("Player 2 is ready");
            }
        }


    }
}
