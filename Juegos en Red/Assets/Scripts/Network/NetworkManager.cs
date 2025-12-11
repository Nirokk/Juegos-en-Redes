using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager :  MonoBehaviourPun
{
    private int _alivePlayersTeamA;
    private int _alivePlayersTeamB;


    public PhotonManager photonManager;

    private Action OnConnectedToServer;
    public Action OnJoinedRoom;

    public Action OnPlayerEnteredRoom;
    public Action OnPlayerLeftRoom;

    private List<RoomInfo> rooms = new List<RoomInfo>();

    private static NetworkManager _instance;
    public static NetworkManager Instance { get => _instance; set => _instance = value; }

    public static bool wasInMatchBefore = false;
    public static string lastGameScene = "";
    public static string lastRoomName = "";
    public static int lastRoomID = 0;
    public string currentRoomName;

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
    private void Start()
    {
        photonManager.Init(CheckIfJoinedRoom, CheckRoomCreated, HandleNewPlayerInRoom, HandlePlayerLeftRoom);
    }
    #region Connection
    public void SetNickname(string nickname)
    {
        photonManager.SetNickName(nickname);
    }
    public void ConnectToServer(Action connectionCallback = null) // callback para cuando se conecta
    {
        photonManager.ConnectToServer(CheckConnectionToServer);
        OnConnectedToServer += connectionCallback;
    }
    private void CheckConnectionToServer()
    {
        OnConnectedToServer?.Invoke(); //si no es null, invoca
    }
    #endregion
    #region Players

    public Dictionary<int, Player> GetPlayersInRoom()
    {
        return photonManager.GetPlayersInRoom();
    }


    public void HandleNewPlayerInRoom()
    {
        OnPlayerEnteredRoom?.Invoke();
    }

    public void HandlePlayerLeftRoom()
    {
        OnPlayerLeftRoom?.Invoke();
    }

    #endregion
    #region Scenes
    public void LoadSceneForEveryone(string sceneName)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var hash = new ExitGames.Client.Photon.Hashtable();
        hash["currentScene"] = sceneName;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

        PhotonNetwork.LoadLevel(sceneName);
    }


    #endregion
    #region Rooms

    public string GetCurrentRoomName()
    {
        return photonManager.GetCurrentRoom().Name;
        //string name = photonManager.GetCurrentRoom().Name;
        //return name;
    }
    private void CheckIfJoinedRoom()
    {
        wasInMatchBefore = true;
        OnJoinedRoom?.Invoke();
    }

    public void CleanSession()
    {
        wasInMatchBefore = false;
    }

    public void CreateRoom(string roomName)
    {
        photonManager.CreateRoom(roomName);
        //photonManager.CreateRoom(roomName, CheckRoomCreated);
        lastRoomName = roomName;
        currentRoomName = roomName;
        lastRoomID = PhotonNetwork.LocalPlayer.ActorNumber;
        //return photonManager.GetCurrentRoom().Name;
        //photonManager.CreateRoom(roomName);
        //lastRoomID = photonManager.photonView.ControllerActorNr;

    }
    private void CheckRoomCreated(List<RoomInfo> rooms)
    {
        this.rooms = rooms;
        
    }
    public List<RoomInfo> GetAllRooms()
    {
        return rooms;
    }

    public void JoinLobby()
    {
        photonManager.JoinLobby();
    }
    public void JoinSelectedRoom(string roomName)
    {
        photonManager.JoinRoom(roomName);
        lastRoomName = roomName ;
        currentRoomName = roomName;
        //photonManager.JoinRoom(roomName);
        lastRoomID = PhotonNetwork.LocalPlayer.ActorNumber;
        //lastRoomID = photonManager.photonView.ControllerActorNr;
    }

    public void SaveSession()
    {
        wasInMatchBefore = true;
        currentRoomName = PhotonNetwork.CurrentRoom.Name;
        lastGameScene = SceneManager.GetActiveScene().name;
    }





    #endregion
    #region Teams Controller
    public void RegisterTeams(int teamAPlayers, int teamBPlayers)
    {
        _alivePlayersTeamA = teamAPlayers;
        _alivePlayersTeamB = teamBPlayers;
    }

    [PunRPC]
    public void PlayerDied(int actorNumber, int team)
    {
        if (team == 0) // Team A
            _alivePlayersTeamA--;
        else if (team == 1) // Team B
            _alivePlayersTeamB--;

        if (_alivePlayersTeamA == 0 || _alivePlayersTeamB == 0)
        {
            string winner = _alivePlayersTeamA > 0 ? "A" : "B";
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
        _alivePlayersTeamA = 2; // reemplazar con la cantidad real de jugadores en A
        _alivePlayersTeamB = 2; // reemplazar con la cantidad real de jugadores en B
    }
    #endregion
}
