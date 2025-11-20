using Photon.Realtime;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoTeamPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerIcon playerPrefab;
    [SerializeField] private Transform noTeamPanel;
    [SerializeField] private Transform aTeamPanel;
    [SerializeField] private Transform bTeamPanel;

    private List<PlayerIcon> playersUI = new List<PlayerIcon>();
    private const string TEAM_KEY = "team";

    void Start()
    {
        UpdatePlayers();
    }

    // Cuando un jugador cambia sus custom properties (como team)
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(TEAM_KEY))
        {
            UpdatePlayers();
        }
    }

    // Cuando un jugador entra a la room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayers();
    }

    // Cuando un jugador sale de la room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayers();
    }


    private void UpdatePlayers()
    {
        ClearPlayers();

        Player[] players = PhotonNetwork.PlayerList;
        Debug.Log("PhotonNetwork.PlayerList count = " + players.Length);

        foreach (Player p in players)
        {
            if (p == null) continue;
            if (string.IsNullOrEmpty(p.NickName)) continue;   // evita íconos vacíos

            string team = null;

            if (p.CustomProperties.TryGetValue(TEAM_KEY, out object teamObj))
                team = teamObj?.ToString();

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
        foreach (PlayerIcon playerItemUI in playersUI)
        {
            if (playerItemUI != null)
                Destroy(playerItemUI.gameObject);
        }

        playersUI.Clear();
    }
}
