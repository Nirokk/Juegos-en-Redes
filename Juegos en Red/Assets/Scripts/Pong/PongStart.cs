using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PongStart : MonoBehaviourPunCallbacks
{
    private List<Player> _players = new List<Player>();

    public Transform[] teamASpawnPoints;
    public Transform[] teamBSpawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        //AssingTeams();
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
        PhotonNetwork.Instantiate("Pong-Player", spawnPos, Quaternion.identity);
        Debug.Log("player instantiated");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _players.Add(newPlayer);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AssingTeams();
        }
    }


    private void AssingTeams()
    {
        Player[] allPlayers = PhotonNetwork.PlayerList;

        for (int i = 0; i < allPlayers.Length; i++)
        {
            ExitGames.Client.Photon.Hashtable teamProp = new ExitGames.Client.Photon.Hashtable();

            if (i % 2 == 0)
            {
                teamProp.Add("team", "TeamA");
            }
            else
            {
                teamProp.Add("team", "TeamB");
            }

            allPlayers[i].SetCustomProperties(teamProp);
        }
    }


    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer && changedProps.ContainsKey("team"))
        {
            string myTeam = changedProps["team"].ToString();
            SpawnPlayer(myTeam);
        }
    }

    private void SpawnPlayer(string team)
    {
        Vector3 spawnPos = Vector3.zero;

        if (team == "TeamA" && teamASpawnPoints.Length > 0)
        {
            spawnPos = teamASpawnPoints[Random.Range(0, teamASpawnPoints.Length)].position;
        }
        else if (team == "TeamB" && teamBSpawnPoints.Length > 0)
        {
            spawnPos = teamBSpawnPoints[Random.Range(0, teamBSpawnPoints.Length)].position;
        }

        PhotonNetwork.Instantiate("Pong-Player", spawnPos, Quaternion.identity);
        Debug.Log("Player instantiated for team: " + team);
    }



}
