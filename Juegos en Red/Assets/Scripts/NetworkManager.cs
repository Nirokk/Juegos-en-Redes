using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager :  MonoBehaviourPun
{
    private int alivePlayersTeamA;
    private int alivePlayersTeamB;

    private static NetworkManager _instance;
    public static NetworkManager Instance { get => _instance; set => _instance = value; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void ConnectToServer(string nickname)
    {
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.ConnectUsingSettings();
        
    }

    public void RegisterTeams(int teamAPlayers, int teamBPlayers)
    {
        alivePlayersTeamA = teamAPlayers;
        alivePlayersTeamB = teamBPlayers;
    }

    [PunRPC]
    public void PlayerDied(int actorNumber, int team)
    {
        if (team == 0) // Team A
            alivePlayersTeamA--;
        else if (team == 1) // Team B
            alivePlayersTeamB--;

        if (alivePlayersTeamA == 0 || alivePlayersTeamB == 0)
        {
            string winner = alivePlayersTeamA > 0 ? "A" : "B";
            photonView.RPC("EndRound", RpcTarget.All, winner);
        }
    }

    [PunRPC]
    public void EndRound(string winningTeam)
    {
        Debug.Log("Ronda terminada, ganó el equipo " + winningTeam);
        StartCoroutine(NewRound());
    }

    private IEnumerator NewRound()
    {
        yield return new WaitForSeconds(5f);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.IsLocal)
            {
                Vector3 spawnPos = Vector3.zero; // TODO: elegir spawn según equipo
                PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
            }
        }

        // Resetear contadores (ejemplo: todos vivos de nuevo)
        alivePlayersTeamA = 2; // reemplazar con la cantidad real de jugadores en A
        alivePlayersTeamB = 2; // reemplazar con la cantidad real de jugadores en B
    }
}
