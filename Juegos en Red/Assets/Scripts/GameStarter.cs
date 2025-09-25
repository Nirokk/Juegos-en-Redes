using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviourPunCallbacks
{
    private PhotonView _photonView;
    public Transform[] spawnPoints;
    //public LayerMask localPlayerLayer;
    //public LayerMask onlinePlayerLayer;
    private GameObject _player;

    private void Awake()
    {
        

    }
    private void Start()
    {
        _player = PhotonNetwork.Instantiate("NewPlayer", new Vector3(0, 0), Quaternion.identity);
        _photonView = _player.GetComponentInChildren<PhotonView>();
        Debug.Log(_player);

        if (_photonView.IsMine)
        {
            _player.layer = 3;
        }
        else
        {
            _player.layer = 7;
        }

    }


    
    //public void NewSpawnPoint()
    //{
    //    if (spawnPoints.Length == 0) return;
        
    //    // Implement spawn point logic here
    //}
    
}
