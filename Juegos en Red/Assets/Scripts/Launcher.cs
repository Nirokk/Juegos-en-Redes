using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.UI;
using Unity.VisualScripting;
using Photon.Realtime;


public class Launcher : MonoBehaviourPunCallbacks
{
    private const string nameEveryFriendKnows = "NewRoom";

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("ME CONECTE");
        
    }

   //public void CreateRoom()
   // {
   //     RoomOptions roomOptions = new RoomOptions();
   //     roomOptions.IsVisible = false;
   //     roomOptions.MaxPlayers = 4;
   //     PhotonNetwork.JoinOrCreateRoom(nameEveryFriendKnows, roomOptions, TypedLobby.Default);
   //     Debug.Log("my room name is " + nameEveryFriendKnows);
   // }
    
}
