using System;
using Unity.Netcode;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private ulong owner;
    public ReadyStage readyStage;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Unit")) return; 
        if(!other.TryGetComponent<NetworkObject>(out var networkObject)) return;
        if (networkObject.OwnerClientId != owner)
        {
            other.gameObject.SetActive(false);
        }
    }
}
