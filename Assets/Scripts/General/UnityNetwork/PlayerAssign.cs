using Unity.Netcode;
using UnityEngine;

namespace General.UnityNetwork
{
    public class PlayerAssign : NetworkBehaviour
    {
        [SerializeField] private NetworkObject playerOne;
        [SerializeField] private NetworkObject playerTwo;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer) return;
            AssignPlayersServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void AssignPlayersServerRpc()
        {
            playerOne.RemoveOwnership();
            playerTwo.RemoveOwnership();
            playerOne.ChangeOwnership(0);
            playerTwo.ChangeOwnership(1);
        }
    }
}
