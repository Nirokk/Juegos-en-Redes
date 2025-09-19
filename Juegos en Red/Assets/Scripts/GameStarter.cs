using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviourPunCallbacks
{
    private PhotonView playerPrefab;
    public Transform[] spawnPoints;
    public LayerMask localPlayerLayer;
    public LayerMask onlinePlayerLayer;

    private void Start()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();

    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate("NewPlayer", new Vector3 (0,0), Quaternion.identity);

        playerPrefab = player.GetComponent<PhotonView>();

        if (playerPrefab.IsMine)
        {
            player.layer = 3;
        }
        else
        {
            player.layer = 7;
        }
    }
    
    public void NewSpawnPoint()
    {
        if (spawnPoints.Length == 0) return;
        
        // Implement spawn point logic here
    }
    
}
