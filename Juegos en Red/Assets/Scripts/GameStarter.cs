using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviourPunCallbacks
{
    private PhotonView _photonView;

    [Header("Spawn Points - Team A")]
    public Transform[] teamASpawnPoints;

    [Header("Spawn Points - Team B")]
    public Transform[] teamBSpawnPoints;
    //public LayerMask localPlayerLayer;
    //public LayerMask onlinePlayerLayer;
    private GameObject _player;

    private void Awake()
    {
        

    }
    private void Start()
    {
        // 1. Obtener el equipo del jugador local
        string myTeam = "TeamA"; // valor por defecto
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("team"))
        {
            myTeam = PhotonNetwork.LocalPlayer.CustomProperties["team"].ToString();
        }

        // 2. Elegir un spawn point aleatorio según el equipo
        Vector3 spawnPos = Vector3.zero;
        if (myTeam == "TeamA" && teamASpawnPoints.Length > 0)
        {
            spawnPos = teamASpawnPoints[Random.Range(0, teamASpawnPoints.Length)].position;
        }
        else if (myTeam == "TeamB" && teamBSpawnPoints.Length > 0)
        {
            spawnPos = teamBSpawnPoints[Random.Range(0, teamBSpawnPoints.Length)].position;
        }

        _player = PhotonNetwork.Instantiate("NewPlayer", spawnPos, Quaternion.identity);
        _photonView = _player.GetComponentInChildren<PhotonView>();

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("team"))
            myTeam = PhotonNetwork.LocalPlayer.CustomProperties["team"].ToString();
        else
        {
            Debug.LogWarning("El jugador no tiene equipo asignado todavía");
        }

        Debug.Log(_player);

        if (_photonView.IsMine)
        {
            _player.layer = 3; //Local
        }
        else
        {
            _player.layer = 7; //Network
        }

    }


    
    //public void NewSpawnPoint()
    //{
    //    if (spawnPoints.Length == 0) return;
        
    //    // Implement spawn point logic here
    //}
    
}
