using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;


public class TeamSelectionManager : MonoBehaviour
{
    public Button startMatch;
    public Button joinTeamA;
    public Button joinTeamB;
    private const string TEAM_KEY = "team";
    

    void Start()
    {
        startMatch.onClick.AddListener(StartGame);
        joinTeamA.onClick.AddListener(AddToTeamA);
        joinTeamB.onClick.AddListener(AddToTeamB);


    }
    private void Update()
    {
        CheckTeams();
    }

    private void StartGame()
    { 
        NetworkManager.Instance.LoadSceneForEveryone("GameScene");
    }
    public void CheckTeams()
    {
        int countA = 0;
        int countB = 0;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue(TEAM_KEY, out object teamObj))
            {
                string team = teamObj.ToString();
                if (team == "A") countA++;
                else if (team == "B") countB++;
            }
            
        }
        startMatch.interactable = (countA > 0 && countB > 0 && PhotonNetwork.IsMasterClient);
        
    }
    public void AddToTeamA()
    {
        
        var props = new Hashtable { { TEAM_KEY, "A" } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " moved to Team A");
    }
    public void AddToTeamB()
    {
       
        var props = new Hashtable { { TEAM_KEY, "B" } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " moved to Team B");
    }

}
