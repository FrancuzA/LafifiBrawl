using System;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEditor.Tilemaps;
using System.Runtime.CompilerServices;

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
    
    public void JoinAsServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    private IEnumerator StartGame()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count > 1);
        NetworkManager.Singleton.SceneManager.LoadScene("InventoryStage", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
