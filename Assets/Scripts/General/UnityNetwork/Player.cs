using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        var id = NetworkObjectId;
        if (IsOwner)
        {
            transform.position = new Vector3(0, -3.8f, 0);
            Debug.Log($"Player {id} object spawned for the local player.");
        }
        else
        {
            transform.position = new Vector3(0, 3.8f, 0);
            Debug.Log($"Player {id} object spawned for a remote player.");
        }
    }
}
