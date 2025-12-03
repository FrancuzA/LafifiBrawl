using General.UnityNetwork;
using Unity.Netcode;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    [SerializeField] private ulong owner;
    public ReadyStage readyStage;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Unit")) return; 
        if(!other.TryGetComponent<NetworkBehaviour>(out var networkObject)) return;
        if(!other.TryGetComponent<PlayerUnit>(out var playerUnit)) return;
        
        if (playerUnit.OwnerClientId != owner)
        {
            readyStage.DespawnUnitAndDealDamageServerRpc(networkObject, playerUnit.GetUnitIndex(), owner);
        }
    }
}
