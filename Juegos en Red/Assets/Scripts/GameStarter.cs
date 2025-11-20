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

    private List<Player> _playersInAteam;
    private List<Player> _playersInBteam;

    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if ((string)PhotonNetwork.LocalPlayer.CustomProperties["team"] == "A")
        {
            GameObject playerObject = PhotonNetwork.Instantiate("NewPlayer", teamAspawnPointsList[Random.Range(0, teamAspawnPointsList.Count)].position, Quaternion.identity);
            DisconnectionHandler.RegisterPlayerInstance(PhotonNetwork.LocalPlayer.ActorNumber, playerObject);
            _player = PhotonNetwork.LocalPlayer;
            _playersInAteam.Add(_player);

            // Assign the local player reference
        }
        else if ((string)PhotonNetwork.LocalPlayer.CustomProperties["team"] == "B")
        {
            GameObject playerObject = PhotonNetwork.Instantiate("NewPlayer", teamBspawnPointsList[Random.Range(0, teamBspawnPointsList.Count)].position, Quaternion.identity);
            DisconnectionHandler.RegisterPlayerInstance(PhotonNetwork.LocalPlayer.ActorNumber, playerObject);
            _player = PhotonNetwork.LocalPlayer; // Assign the local player reference
            _playersInBteam.Add(_player);
        }
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        
        CleanupPlayerObjects(otherPlayer);
    }

    private void CleanupPlayerObjects(Player player)
    {
        PhotonView[] allViews = FindObjectsOfType<PhotonView>();

        foreach (PhotonView view in allViews)
        {
            if (view.Owner != null && view.Owner.ActorNumber == player.ActorNumber)
            {
                PhotonNetwork.Destroy(view.gameObject);
                Debug.Log("Destroyed leftover object of disconnected player: " + player.NickName);
                
            }
        }
        CheckPlayersCount();
    }

    private void CheckPlayersCount()
    {
        //if(_playersInAteam.Count == 0 || _playersInBteam.Count == 0)
        //{
        //    Debug.Log("One team has no players left. Ending game...");
        //    NetworkManager.Instance.LoadSceneForEveryone("LobbyScene");

        //}
        NetworkManager.Instance.LoadSceneForEveryone("LobbyScene");
    }

}
