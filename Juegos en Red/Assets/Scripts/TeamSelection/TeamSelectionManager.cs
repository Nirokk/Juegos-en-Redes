using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;


public class TeamSelectionManager : MonoBehaviourPunCallbacks
{
    public Button startMatch;
    public Button joinTeamA;
    public Button joinTeamB;
    public Button returnButton;
    
    private ExitGames.Client.Photon.Hashtable _myTeamIs = new ExitGames.Client.Photon.Hashtable();




    void Start()
    {
        startMatch.onClick.AddListener(StartGame);
        joinTeamA.onClick.AddListener(AddToTeamA);
        joinTeamB.onClick.AddListener(AddToTeamB);
        returnButton.onClick.AddListener(LeaveRoom);
        

        startMatch.interactable = PhotonNetwork.IsMasterClient;

        CheckTeamsAndUpdateStartButton();
    }


    private void StartGame()
    {
        NetworkManager.Instance.LoadSceneForEveryone("GameScene");
        
    }


    public void AddToTeamA()
    {
        _myTeamIs["team"] = "A";
        PhotonNetwork.LocalPlayer.SetCustomProperties(_myTeamIs);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " moved to Team A");
        CheckTeamsAndUpdateStartButton();
    }

    
    public void AddToTeamB()
    {
        _myTeamIs["team"] = "B";
        PhotonNetwork.LocalPlayer.SetCustomProperties(_myTeamIs);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " moved to Team B");
        CheckTeamsAndUpdateStartButton();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }
    // Se llama cuando cambian propiedades (como team)
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        CheckTeamsAndUpdateStartButton();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //otherPlayer.CustomProperties["team"] = null;
        CheckTeamsAndUpdateStartButton();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startMatch.interactable = PhotonNetwork.IsMasterClient;
        CheckTeamsAndUpdateStartButton();
    }

    private void CheckTeamsAndUpdateStartButton()
    {
        int teamAcount = 0;
        int teamBcount = 0;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("team", out object teamObj))
            {
                string t = (string)teamObj;
                if (t == "A") teamAcount++;
                if (t == "B") teamBcount++;
            }
        }

        bool bothTeamsHavePlayers = teamAcount > 0 && teamBcount > 0;

        
        startMatch.interactable = PhotonNetwork.IsMasterClient && bothTeamsHavePlayers;
    }
}

