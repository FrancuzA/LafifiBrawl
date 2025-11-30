using System;
using Unity.Netcode;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Unit")) return;
        var networkObject = other.GetComponent<NetworkObject>();
    }
}
