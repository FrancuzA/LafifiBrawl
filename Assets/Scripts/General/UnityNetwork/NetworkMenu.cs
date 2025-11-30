using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General.UnityNetwork
{
    public class NetworkMenu : MonoBehaviour
    {
        [SerializeField] private GameObject playerEq;
        [SerializeField] private GameObject spawner;
        [SerializeField] private GameObject waitingForPlayers;
        

        public void JoinAsHost()
        {
            NetworkManager.Singleton.StartHost();
            //NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
            waitingForPlayers.SetActive(true);
            StartCoroutine(StartGame());
        }

        /*private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            foreach (var playerId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                var player = Instantiate(playerEq);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerId);
            }
        }*/

        public void JoinAsClient()
        {
            NetworkManager.Singleton.StartClient();
        }

        private static IEnumerator StartGame()
        {
            yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count > 1);
            NetworkManager.Singleton.SceneManager.LoadScene("InventoryStage", LoadSceneMode.Single);
        }
    }
}
