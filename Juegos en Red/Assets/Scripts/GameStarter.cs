using Photon.Pun;
using Photon.Realtime;
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

    [Header("Spawn Points - Team A")]
    public Transform[] teamASpawnPoints;

    [Header("Spawn Points - Team B")]
    public Transform[] teamBSpawnPoints;

    private void Awake()
    {
        

    }
    private void Start()
    {
        AssignTeam(PhotonNetwork.LocalPlayer);
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
    void AssignTeam(Player player)
    {
        int actorNumber = player.ActorNumber; // número único que Photon asigna

        string team = (actorNumber % 2 == 0) ? "TeamA" : "TeamB";

        // Guardar el team en las CustomProperties del jugador
        var props = new ExitGames.Client.Photon.Hashtable();
        props["team"] = team;
        player.SetCustomProperties(props);

        Debug.Log($"Jugador {player.NickName} (ActorNumber {actorNumber}) asignado a {team}");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("team"))
        {
            Debug.Log($"Jugador {targetPlayer.NickName} está en {changedProps["team"]}");
        }

        if (changedProps.ContainsKey("kills"))
        {
            Debug.Log($"[SCORE] {targetPlayer.NickName} ahora tiene {changedProps["kills"]} kills");
            // Aquí actualizas la UI del scoreboard

        }
    }



    //public void NewSpawnPoint()
    //{
    //    if (spawnPoints.Length == 0) return;

    //    // Implement spawn point logic here
    //}

}
