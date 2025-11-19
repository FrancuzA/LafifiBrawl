using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General.UnityNetwork
{
    public class NetworkMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject waitingForPlayers;

        public void JoinAsHost()
        {
            NetworkManager.Singleton.StartHost();
            waitingForPlayers.SetActive(true);
            StartCoroutine(StartGame());
        }
    
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
