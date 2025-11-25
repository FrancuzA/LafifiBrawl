using System.Collections;
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
            StartCoroutine(WaitForPlayersToConnect());
        }

        private IEnumerator WaitForPlayersToConnect()
        {
            yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClients.Count >= 2);
            AssignPlayers();
        }

        private void AssignPlayers()
        {
            var player1 = Instantiate(playerOne, transform);
            player1.SpawnAsPlayerObject(0, true);
            var player2 = Instantiate(playerTwo, transform);
            player2.SpawnAsPlayerObject(1, true);
        }
    }
}
