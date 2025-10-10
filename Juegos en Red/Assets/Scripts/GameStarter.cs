using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviourPunCallbacks
{
    public PhotonView _photonView;
    public Transform[] spawnPoints;
    //public LayerMask localPlayerLayer;
    //public LayerMask onlinePlayerLayer;
    private GameObject _player;
    TeamSelectionManager _teamSelectionManager;

    [Header("Spawn Points - Team A")]
    public List<Transform> teamAspawnPointsList;

    [Header("Spawn Points - Team B")]
    public List<Transform> teamBspawnPointsList;

    private const string TEAM_KEY = "team";

    private void Awake()
    {


    }
    private void Start()
    {
        _photonView = this.gameObject.GetComponent<PhotonView>();
        _teamSelectionManager = FindObjectOfType<TeamSelectionManager>();
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if (_photonView.IsMine)
        {
            if (_teamSelectionManager.teamA.Contains(_photonView.Owner.UserId))
            {
                _player = PhotonNetwork.Instantiate("NewPlayer",teamAspawnPointsList[0].position, Quaternion.identity);
            }
            if (_teamSelectionManager.teamB.Contains(_photonView.Owner.UserId))
            {
                _player = PhotonNetwork.Instantiate("NewPlayer", teamBspawnPointsList[0].position, Quaternion.identity);
            }

        }
        else
        {
            Debug.Log("lol");
        }
    }
































        //AssignTeam(PhotonNetwork.LocalPlayer);
        //// 1. Obtener el equipo del jugador local
        //string myTeam = "TeamA"; // valor por defecto
        //if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("team"))
        //{
        //    myTeam = PhotonNetwork.LocalPlayer.CustomProperties["team"].ToString();
        //}

        //// 2. Elegir un spawn point aleatorio según el equipo
        //Vector3 spawnPos = Vector3.zero;
        //if (myTeam == "TeamA" && teamASpawnPoints.Length > 0)
        //{
        //    spawnPos = teamASpawnPoints[Random.Range(0, teamASpawnPoints.Length)].position;
        //}
        //else if (myTeam == "TeamB" && teamBSpawnPoints.Length > 0)
        //{
        //    spawnPos = teamBSpawnPoints[Random.Range(0, teamBSpawnPoints.Length)].position;
        //}


        //_player = PhotonNetwork.Instantiate("NewPlayer", spawnPos, Quaternion.identity);
        //_photonView = _player.GetComponentInChildren<PhotonView>();
        //Debug.Log(_player);

        //if (_photonView.IsMine)
        //{
        //    _player.layer = 3;
        //}
        //else
        //{
        //    _player.layer = 7;
        //}


   
    //void AssignTeam(Player player)
    //{
    //    int actorNumber = player.ActorNumber; // número único que Photon asigna

    //    string team = (actorNumber % 2 == 0) ? "TeamA" : "TeamB";

    //    // Guardar el team en las CustomProperties del jugador
    //    var props = new ExitGames.Client.Photon.Hashtable();
    //    props["team"] = team;
    //    player.SetCustomProperties(props);

    //    Debug.Log($"Jugador {player.NickName} (ActorNumber {actorNumber}) asignado a {team}");
    //}

    //public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    //{
    //    if (changedProps.ContainsKey("team"))
    //    {
    //        Debug.Log($"Jugador {targetPlayer.NickName} está en {changedProps["team"]}");
    //    }

    //    if (changedProps.ContainsKey("kills"))
    //    {
    //        Debug.Log($"[SCORE] {targetPlayer.NickName} ahora tiene {changedProps["kills"]} kills");
    //        // Aquí actualizas la UI del scoreboard

    //    }
    //}

    public override void OnLeftRoom()
    {
        PhotonNetwork.Destroy(_player);
    }


    //public void NewSpawnPoint()
    //{
    //    if (spawnPoints.Length == 0) return;

    //    // Implement spawn point logic here
    //}

}
