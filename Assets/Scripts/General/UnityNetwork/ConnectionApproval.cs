using UnityEngine;

public class ConnectionApproval : MonoBehaviour
{
    [SerializeField] private ushort maxPlayers = 2;
    
    private void Start()
    {
        var networkManager = Unity.Netcode.NetworkManager.Singleton;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
    }
    
    private void ApprovalCheck(Unity.Netcode.NetworkManager.ConnectionApprovalRequest request, Unity.Netcode.NetworkManager.ConnectionApprovalResponse response)
    {
        
        response.Approved = true;
        
        if (Unity.Netcode.NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers)
        {
            response.Approved = false;
            response.Reason = "Server full";
        }
        
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;
        
        response.Pending = false;
    }
}