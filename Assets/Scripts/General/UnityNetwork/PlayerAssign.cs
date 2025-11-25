using Unity.Netcode;
using UnityEngine;

public class PlayerAssign : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerOne;
    [SerializeField] private NetworkObject playerTwo;

    private void Start()
    {
        AssignPlayersServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AssignPlayersServerRpc(ServerRpcParams rpcParams = default)
    {
        if (rpcParams.Receive.SenderClientId == 0)
        {
            playerOne.SpawnAsPlayerObject(rpcParams.Receive.SenderClientId, true);
        }
        else
        {
            playerTwo.SpawnAsPlayerObject(rpcParams.Receive.SenderClientId, true);
        }

    }
}
