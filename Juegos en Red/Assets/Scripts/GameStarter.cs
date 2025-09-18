using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public PhotonView playerPrefab;
    public Transform[] spawnPoints;

    private void Start()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate("NewPlayer", new Vector3 (0,0), Quaternion.identity);
        
    }
    
    public void NewSpawnPoint()
    {
        if (spawnPoints.Length == 0) return;
        
        // Implement spawn point logic here
    }
}
