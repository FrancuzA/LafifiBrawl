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
            AssignPlayersServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void AssignPlayersServerRpc(ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId == 0)
            {
                playerOne.ChangeOwnership(rpcParams.Receive.SenderClientId);
            }
            else
            {
                playerTwo.ChangeOwnership(rpcParams.Receive.SenderClientId);
            }

        }
    }
}
