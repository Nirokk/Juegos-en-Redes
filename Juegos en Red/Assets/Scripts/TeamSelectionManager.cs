using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;


public class TeamSelectionManager : MonoBehaviour
{
    public Button startMatch;
    public Button joinTeamA;
    public Button joinTeamB;
    
    private ExitGames.Client.Photon.Hashtable _myTeamIs = new ExitGames.Client.Photon.Hashtable();




    void Start()
    {
        startMatch.onClick.AddListener(StartGame);
        joinTeamA.onClick.AddListener(AddToTeamA);
        joinTeamB.onClick.AddListener(AddToTeamB);
        //pv = this.gameObject.GetComponent<PhotonView>();
        
    }


    private void StartGame()
    {
        NetworkManager.Instance.LoadSceneForEveryone("GameScene");
        //pv.RPC("GoToGame", RpcTarget.All);
    }


    public void AddToTeamA()
    {
        _myTeamIs["team"] = "A";
        PhotonNetwork.LocalPlayer.SetCustomProperties(_myTeamIs);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " moved to Team A");
    }

    
    public void AddToTeamB()
    {
        _myTeamIs["team"] = "B";
        PhotonNetwork.LocalPlayer.SetCustomProperties(_myTeamIs);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " moved to Team B");
    }


}
