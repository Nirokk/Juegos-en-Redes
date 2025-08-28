using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public PhotonView playerPrefab;

    private void Start()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3 (0,0), Quaternion.identity);

    }
    
    
}
