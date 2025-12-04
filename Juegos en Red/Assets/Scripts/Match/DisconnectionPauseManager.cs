using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class DisconnectionPauseManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    public GameObject panelWaitingPlayer;
    public GameObject panelForceMatchEnded;
    public GameObject panelPlayerReconnect;
    public GameObject panelContinueMatch;

    [Header("UI Timer References")]
    public TextMeshProUGUI waitingTimerTMP;
    public TextMeshProUGUI continueTimerTMP_A;
    public TextMeshProUGUI continueTimerTMP_B;

    [Header("Timers")]
    public float timeToWaitReconnect = 20f;
    public float timeToResumeMatch = 5f;

    private float currentTimer;

    private bool waitingForReconnect = false;
    private bool reconnectionHappened = false;

    private int disconnectedTeamA = 0;
    private int disconnectedTeamB = 0;

    public static bool gamePaused = false;

    private void Awake()
    {
        DisableAllPanels();
    }

    private void DisableAllPanels()
    {
        panelWaitingPlayer?.SetActive(false);
        panelForceMatchEnded?.SetActive(false);
        panelPlayerReconnect?.SetActive(false);
        panelContinueMatch?.SetActive(false);
    }

    private void PauseGame()
    {
        gamePaused = true;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        gamePaused = false;
        Time.timeScale = 1f;
        DisableAllPanels();
    }

    // Alguien se desconecta
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        CountTeams();

        waitingForReconnect = true;
        reconnectionHappened = false;
        currentTimer = timeToWaitReconnect;

        photonView.RPC(nameof(RPC_ShowWaitingPanel), RpcTarget.AllBuffered);
        PauseGame();
    }

    // Alguien vuelve
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        CountTeams();

        reconnectionHappened = true;
        waitingForReconnect = false;
        currentTimer = timeToResumeMatch;

        photonView.RPC(nameof(RPC_ShowReconnectPanel), RpcTarget.AllBuffered);
    }

    private void CountTeams()
    {
        disconnectedTeamA = 0;
        disconnectedTeamB = 0;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("team", out object team))
            {
                if ((string)team == "A") disconnectedTeamA++;
                if ((string)team == "B") disconnectedTeamB++;
            }
        }
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!gamePaused) return;

        currentTimer -= Time.unscaledDeltaTime;

        if (waitingForReconnect)
        {
            photonView.RPC(nameof(RPC_UpdateWaitingTimer), RpcTarget.All, currentTimer);

            if (currentTimer <= 0f)
            {
                bool canContinue = (disconnectedTeamA > 0 && disconnectedTeamB > 0);

                if (canContinue)
                    photonView.RPC(nameof(RPC_ShowContinuePanel), RpcTarget.AllBuffered);
                else
                    photonView.RPC(nameof(RPC_ShowForceEndPanel), RpcTarget.AllBuffered);

                waitingForReconnect = false;
            }
        }
        else if (reconnectionHappened)
        {
            photonView.RPC(nameof(RPC_UpdateReconnectTimer), RpcTarget.All, currentTimer);

            if (currentTimer <= 0f)
            {
                reconnectionHappened = false;
                photonView.RPC(nameof(RPC_ResumeMatch), RpcTarget.AllBuffered);
            }
        }
    }

    // RPC UI Handling

    [PunRPC]
    private void RPC_ShowWaitingPanel()
    {
        DisableAllPanels();
        panelWaitingPlayer?.SetActive(true);
    }

    [PunRPC]
    private void RPC_ShowReconnectPanel()
    {
        DisableAllPanels();
        panelPlayerReconnect?.SetActive(true);
    }

    [PunRPC]
    private void RPC_ShowContinuePanel()
    {
        DisableAllPanels();
        panelContinueMatch?.SetActive(true);
    }

    [PunRPC]
    private void RPC_ShowForceEndPanel()
    {
        DisableAllPanels();
        panelForceMatchEnded?.SetActive(true);
    }

    [PunRPC]
    private void RPC_UpdateWaitingTimer(float time)
    {
        waitingTimerTMP.text = Mathf.Ceil(time).ToString();
    }

    [PunRPC]
    private void RPC_UpdateReconnectTimer(float time)
    {
        continueTimerTMP_A.text = Mathf.Ceil(time).ToString();
        continueTimerTMP_B.text = Mathf.Ceil(time).ToString();
    }

    [PunRPC]
    private void RPC_ResumeMatch()
    {
        ResumeGame();
    }
}
