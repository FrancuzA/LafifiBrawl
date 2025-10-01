using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public void JoinRoom()
    {
        Debug.Log("Trying to join or create");
        PhotonNetwork.JoinRandomOrCreateRoom(null, 2);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joining...");
        PhotonNetwork.LoadLevel("Game");
    }
}
