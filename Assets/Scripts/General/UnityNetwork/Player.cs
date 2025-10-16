using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            transform.position = new Vector3(0, -3.8f, 0);
        }
        else
        {
            transform.position = new Vector3(0, 3.8f, 0);
        }
    }
}
