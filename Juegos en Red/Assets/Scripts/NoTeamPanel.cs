using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoTeamPanel : MonoBehaviour
{
    [SerializeField] private PlayerIcon playerPrefab;
    [SerializeField] private Transform noTeamPanel;
    [SerializeField] private Transform aTeamPanel;
    [SerializeField] private Transform bTeamPanel;
    

    private List<PlayerIcon> playersUI = new List<PlayerIcon>();
    private const string TEAM_KEY = "team";

    void Start()
    {
      
        NetworkManager.Instance.OnJoinedRoom += UpdatePlayers;
        NetworkManager.Instance.OnPlayerLeftRoom += UpdatePlayers;
        NetworkManager.Instance.OnPlayerEnteredRoom += UpdatePlayers;

        UpdatePlayers();
    }

    private void Update()
    {
        UpdatePlayers();
    }
    private void UpdatePlayers()
    {

        ClearPlayers();

        Dictionary<int, Player> players = NetworkManager.Instance.GetPlayersInRoom();
        Debug.Log("Players count: " + players.Count);

        foreach (KeyValuePair<int, Player> kv in players)
        {
            Player p = kv.Value;
            string team = null;

            if (p.CustomProperties.TryGetValue(TEAM_KEY, out object teamObj))
            {
                team = teamObj?.ToString();
                Debug.Log($"Player {p.NickName} is in team {team}");

            }


            PlayerIcon icon;
            if (team == "A")
            {
                icon = Instantiate(playerPrefab, aTeamPanel);
                icon.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            }
            else if (team == "B")
            {
                icon = Instantiate(playerPrefab, bTeamPanel);
                icon.GetComponent<UnityEngine.UI.Image>().color = Color.blue;
            }
            else
            {
                icon = Instantiate(playerPrefab, noTeamPanel);
            }

            icon.SetUp(p);
            playersUI.Add(icon);
        }


    }

    private void ClearPlayers()
    {
        //foreach (PlayerIcon playerItemUI in playersUI)
        //{
        //    Destroy(playerItemUI.gameObject);
        //}
        //playersUI.Clear();

        foreach (PlayerIcon playerItemUI in playersUI)
        {
            if (playerItemUI != null)
                Destroy(playerItemUI.gameObject);
        }
        playersUI.Clear();
    }
}
