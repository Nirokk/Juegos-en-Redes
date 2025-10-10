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
    public PhotonView pv;
    public List<string> teamA = new List<string>();
    public List<string> teamB = new List<string>();


    //public static TeamSelectionManager Instance;

    //private void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //    else
    //    {
    //        Instance = this;
    //        DontDestroyOnLoad(this.gameObject);
    //    }
    //}
    private void Awake()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            pv = GetComponent<PhotonView>();
        }
    }
    void Start()
    {
        startMatch.onClick.AddListener(StartGame);
        joinTeamA.onClick.AddListener(AddToTeamA);
        joinTeamB.onClick.AddListener(AddToTeamB);
        //pv = this.gameObject.GetComponent<PhotonView>();
        DontDestroyOnLoad(this.gameObject);
    }
    private void Update()
    {
        CheckTeams();
    }

    private void StartGame()
    {
        NetworkManager.Instance.LoadSceneForEveryone("GameScene");
        //pv.RPC("GoToGame", RpcTarget.All);
    }

    [PunRPC]
    private void GoToGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void CheckTeams()
    {
        
        startMatch.interactable = (teamA.Count > 0 && teamB.Count > 0 && PhotonNetwork.IsMasterClient);
        
    }
    
    public void AddToTeamA()
    {
        pv.RPC("PlayerToTeam", RpcTarget.MasterClient, 0, pv.Owner.UserId);
        //var props = new Hashtable { { TEAM_KEY, "A" } };
        //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " moved to Team A");
    }

    
    public void AddToTeamB()
    {
        
        pv.RPC("PlayerToTeam", RpcTarget.MasterClient, 1, pv.Owner.UserId);
        //var props = new Hashtable { { TEAM_KEY, "B" } };
        //PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " moved to Team B");
    }
    [PunRPC]
    public void PlayerToTeam(int team, string id)
    {
        Debug.Log("Se metio un personaje a un team");
        if(team == 0 && !teamA.Contains(id))
        {
            teamA.Add(id);
            if (teamB.Contains(id))
            {
                teamB.Remove(id);
            }

        }
        if (team == 1 && !teamB.Contains(id))
        {
            teamB.Add(id);
            if (teamA.Contains(id))
            {
                teamA.Remove(id);
            }
        }
    }

}
