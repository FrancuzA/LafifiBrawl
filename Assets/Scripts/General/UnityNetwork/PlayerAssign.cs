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
            var player1 = Instantiate(playerOne, transform);
            player1.SpawnAsPlayerObject(0, true);
            var player2 = Instantiate(playerTwo, transform);
            player2.SpawnAsPlayerObject(1, true);
        }
    }
}
