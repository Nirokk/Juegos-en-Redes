using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;
    public Player _player { get; private set; }

    [Header("Spawn Points - Team A")]
    public List<Transform> teamAspawnPointsList;

    [Header("Spawn Points - Team B")]
    public List<Transform> teamBspawnPointsList;



    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["team"] == "A")
        {
            GameObject playerObject = PhotonNetwork.Instantiate("NewPlayer", teamAspawnPointsList[Random.Range(0, teamAspawnPointsList.Count)].position, Quaternion.identity);
            _player = PhotonNetwork.LocalPlayer;
            
            // Assign the local player reference
        }
        else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["team"] == "B")
        {
            GameObject playerObject = PhotonNetwork.Instantiate("NewPlayer", teamBspawnPointsList[Random.Range(0, teamBspawnPointsList.Count)].position, Quaternion.identity);
            _player = PhotonNetwork.LocalPlayer; // Assign the local player reference
        }
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

}
