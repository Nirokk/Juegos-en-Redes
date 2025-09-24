using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public Action OnConnectedToServer;
    public Action OnJoinedRoomEvent;

    public Action<List<RoomInfo>> OnNewRoomCreated;


    public void Init(Action onJoinRoom, Action<List<RoomInfo>> onRoomCreated)
    {
        OnJoinedRoomEvent += onJoinRoom;
        OnNewRoomCreated += onRoomCreated;
        
    }

    #region servers

    public void SetNickName(string nickname) //establece el nickname del jugador
    {
        PhotonNetwork.NickName = nickname;
    }
    public void ConnectToServer(Action onConnect = null) // callback para cuando se conecta
    {  
        PhotonNetwork.ConnectUsingSettings();
        OnConnectedToServer += onConnect; //suscribe el callback al evento de conecion al server 
    }
    //funcion de Photon para conectar al servidor
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        OnConnectedToServer?.Invoke();//si no es null, invoca
    }


    public void LoadSceneForAllPlayers(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
    #endregion

    #region rooms
    public void CreateRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.EmptyRoomTtl = 100;
        roomOptions.PlayerTtl = 100000;
        roomOptions.BroadcastPropsChangeToAll = true;

        PhotonNetwork.CreateRoom(roomName, roomOptions);
        
        
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        print("Rooms amount> " + roomList.Count);

        OnNewRoomCreated?.Invoke(roomList);
        
    }
    public Room GetCurrentRoom()
    {
        return PhotonNetwork.CurrentRoom;
    }
    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public override void OnJoinedRoom()
    {
        OnJoinedRoomEvent?.Invoke();
        Debug.Log("OnJoinedRoom");
    }
    #endregion
}
