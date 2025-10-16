using System;
using UnityEngine;
using Unity.Netcode;

public class NetworkMenu : MonoBehaviour
{
    public void JoinAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    
    public void JoinAsClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    
    public void JoinAsServer()
    {
        NetworkManager.Singleton.StartServer();
    }
}
