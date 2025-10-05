using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoTeamPanel : MonoBehaviour
{
    [SerializeField] private PlayerIcon playerPrefab;
    [SerializeField] private Transform contentTransform;

    private List<PlayerIcon> playersUI = new List<PlayerIcon>();

    void Start()
    {
      
        NetworkManager.Instance.OnJoinedRoom += UpdatePlayers;
        NetworkManager.Instance.OnPlayerLeftRoom += UpdatePlayers;
        NetworkManager.Instance.OnPlayerEnteredRoom += UpdatePlayers;

        UpdatePlayers();
    }

    private void UpdatePlayers()
    {
        ClearPlayers();

        Dictionary<int, Player> players = NetworkManager.Instance.GetPlayersInRoom();
        print("Players count: " + players.Count);

        foreach (KeyValuePair<int, Player> player in players)
        {
            PlayerIcon playerUI = Instantiate(playerPrefab, contentTransform);
            playerUI.SetUp(player.Value);
            playersUI.Add(playerUI);

        }


    }

    private void ClearPlayers()
    {
        foreach (PlayerIcon playerItemUI in playersUI)
        {
            Destroy(playerItemUI.gameObject);
        }
        playersUI.Clear();
    }
}
