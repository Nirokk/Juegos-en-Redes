using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DisconnectionPauseManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    public GameObject panelReconnectCountdown;   // Panel del minuto
    public GameObject panelVoteContinue;         // Panel de "seguir sin él"

    [Header("UI References")]
    public Slider countdownSlider;
    public TextMeshProUGUI countdownText;

    public Button voteContinueButton;
    public TextMeshProUGUI votesText;

    public Button voteNoButton;

    [Header("Timers")]
    public float reconnectTime = 60f;

    private float currentTimer;
    private bool waitingReconnect;

    private double pausedElapsed = 0;

    // Sistema de votación
    private Dictionary<int, bool> playerVotes = new Dictionary<int, bool>();

    private void Awake()
    {
        HideAllPanels();
    }

    private void HideAllPanels()
    {
        panelReconnectCountdown?.SetActive(false);
        panelVoteContinue?.SetActive(false);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        waitingReconnect = true;
        currentTimer = reconnectTime;

        photonView.RPC(nameof(RPC_ShowReconnectCountdown), RpcTarget.AllBuffered);
        //PauseGame();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!waitingReconnect) return;

        currentTimer -= Time.unscaledDeltaTime;

        photonView.RPC(nameof(RPC_UpdateCountdownUI), RpcTarget.AllBuffered, currentTimer);

        if (currentTimer <= 0f)
        {
            waitingReconnect = false;
            photonView.RPC(nameof(RPC_StartVoteContinuePanel), RpcTarget.AllBuffered);
        }
    }


    [PunRPC]
    private void RPC_ShowReconnectCountdown()
    {
        PauseGame();
        HideAllPanels();
        panelReconnectCountdown.SetActive(true);
        countdownSlider.maxValue = reconnectTime;
        countdownSlider.value = reconnectTime;
        
    }

    [PunRPC]
    private void RPC_UpdateCountdownUI(float timeLeft)
    {
        countdownSlider.value = timeLeft;
        countdownText.text = Mathf.Ceil(timeLeft).ToString();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC(nameof(RPC_ResumeMatch), RpcTarget.All);
    }


    [PunRPC]
    private void RPC_StartVoteContinuePanel()
    {
        HideAllPanels();
        panelVoteContinue.SetActive(true);

        // Inicializamos votos en NO
        playerVotes.Clear();

        // Solo agregamos jugadores conectados actualmente
        foreach (Player p in PhotonNetwork.PlayerList)
            playerVotes[p.ActorNumber] = false;

        // Ajustamos total de votos excluyendo al desconectado
        int totalPlayersToVote = playerVotes.Count - 1; // el que se desconectó
        votesText.text = $"0 / {totalPlayersToVote}";

        voteContinueButton.onClick.RemoveAllListeners();
        voteContinueButton.onClick.AddListener(OnVoteContinueClicked);
        voteNoButton.onClick.RemoveAllListeners();
        voteNoButton.onClick.AddListener(OnVoteNoClicked);
    }

    private void OnVoteContinueClicked()
    {
        photonView.RPC(nameof(RPC_PlayerVotedContinue), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }
    private void OnVoteNoClicked()
    {
        // Avisamos al MasterClient que este jugador votó "No"
        photonView.RPC(nameof(RPC_PlayerVotedNo), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }


    [PunRPC]
    private void RPC_PlayerVotedContinue(int actor)
    {
        playerVotes[actor] = true;

        int totalVotes = 0;
        foreach (var v in playerVotes.Values)
            if (v) totalVotes++;

        int totalPlayersToVote = playerVotes.Count - 1; // excluye al desconectado
        photonView.RPC(nameof(RPC_UpdateVotesUI), RpcTarget.All, totalVotes, totalPlayersToVote);

        if (totalVotes == playerVotes.Count - 1)
        {
            photonView.RPC(nameof(RPC_ResumeMatch), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_PlayerVotedNo(int actor)
    {
        // Pausar/ocultar paneles en todos
        HideAllPanels();
        MatchTimer.Instance.matchPaude = false;

        // Mandar a todos al Main Menu
        PhotonNetwork.AutomaticallySyncScene = true; // asegura que todos carguen la misma escena
        
        NetworkManager.Instance.LoadSceneForEveryone("Main Menu");

        
    }


    [PunRPC]
    private void RPC_UpdateVotesUI(int votes, int total)
    {
        votesText.text = $"{votes} / {total}";
    }

    [PunRPC]
    private void RPC_ResumeMatch()
    {
        HideAllPanels();
        Time.timeScale = 1f;
        MatchTimer.Instance.matchPaude = false;

        // Ajustar MatchStartTime para que el timer no se "salte"
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["MatchStartTime"] = PhotonNetwork.Time - pausedElapsed;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
    }

    private void PauseGame()
    {
        MatchTimer.Instance.matchPaude = true;
        Time.timeScale = 0f;

        // Guardar el tiempo que pasó hasta ahora
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("MatchStartTime", out object start))
        {
            double startTime = (double)start;
            pausedElapsed = PhotonNetwork.Time - startTime;
        }
    }
}
