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
            var clientId = rpcParams.Receive.SenderClientId;
            if (clientId == 0)
            {
                playerOne.RemoveOwnership();
                playerOne.ChangeOwnership(rpcParams.Receive.SenderClientId);
            }
            else
            {
                playerTwo.RemoveOwnership();
                playerTwo.ChangeOwnership(rpcParams.Receive.SenderClientId);
            }

        }
    }
}
