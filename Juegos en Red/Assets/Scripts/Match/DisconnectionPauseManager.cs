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

    private static DisconnectionPauseManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("[DPM] Otro DisconnectionPauseManager encontrado → destruyendo duplicado");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DisableAllPanels();
    }

    // NOTA: quité DontDestroyOnLoad: este manager debe existir en la GameScene como objeto de escena único.

    private void DisableAllPanels()
    {
        panelWaitingPlayer?.SetActive(false);
        panelForceMatchEnded?.SetActive(false);
        panelPlayerReconnect?.SetActive(false);
        panelContinueMatch?.SetActive(false);
    }

    private void PauseGameLocal()
    {
        gamePaused = true;
        Time.timeScale = 0f;
    }

    private void ResumeGameLocal()
    {
        gamePaused = false;
        Time.timeScale = 1f;
        DisableAllPanels();
    }

    // Alguien se desconecta
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[DPM] OnPlayerLeftRoom llamado en cliente {PhotonNetwork.LocalPlayer.NickName} (IsMaster={PhotonNetwork.IsMasterClient}). other: {otherPlayer.NickName} ({otherPlayer.ActorNumber}) InRoom={PhotonNetwork.InRoom}");

        // Guardar conteo y timers solo si estamos en la sala
        CountTeams();
        waitingForReconnect = true;
        reconnectionHappened = false;
        currentTimer = timeToWaitReconnect;

        // >>> TEMPORAL: permite que cualquier cliente pida la pausa (para test)
        // Esto nos va a decir si el problema era "master only" o "RPCs no llegan".
        if (photonView == null)
        {
            Debug.LogError("[DPM] photonView == null, no puedo llamar RPCs.");
            return;
        }

        // Llamamos RPC desde *quien detectó* la desconexión (temporal)
        photonView.RPC(nameof(RPC_ShowWaitingPanel), RpcTarget.AllBuffered);
        photonView.RPC(nameof(RPC_PauseGameAll), RpcTarget.AllBuffered);
    }

    // Alguien vuelve
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[DPM] OnPlayerEnteredRoom llamado en cliente {PhotonNetwork.LocalPlayer.NickName}. newPlayer: {newPlayer.NickName}");
        CountTeams();

        reconnectionHappened = true;
        waitingForReconnect = false;
        currentTimer = timeToResumeMatch;

        if (photonView == null)
        {
            Debug.LogError("[DPM] photonView == null en OnPlayerEnteredRoom.");
            return;
        }

        photonView.RPC(nameof(RPC_ShowReconnectPanel), RpcTarget.AllBuffered);

        if (DisconnectionHandler.playerInstances != null &&
            DisconnectionHandler.playerInstances.TryGetValue(newPlayer.ActorNumber, out GameObject playerObj))
        {
            playerObj.SetActive(true);
        }
        else
        {
            Debug.Log($"[DPM] No encontré playerObj para actor {newPlayer.ActorNumber}");
        }
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
        Debug.Log($"[DPM] CountTeams => A: {disconnectedTeamA} | B: {disconnectedTeamB}");
    }

    private void Update()
    {
        // Sólo el Master debería procesar la cuenta atrás, pero para test mantenemos esto:
        if (!PhotonNetwork.IsMasterClient) return;
        if (!gamePaused) return;

        currentTimer -= Time.unscaledDeltaTime;

        if (waitingForReconnect)
        {
            if (photonView != null) photonView.RPC(nameof(RPC_UpdateWaitingTimer), RpcTarget.All, currentTimer);

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
            if (photonView != null) photonView.RPC(nameof(RPC_UpdateReconnectTimer), RpcTarget.All, currentTimer);

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
        Debug.Log("[DPM_RPC] ShowWaitingPanel recibido.");
        DisableAllPanels();
        panelWaitingPlayer?.SetActive(true);
    }

    [PunRPC]
    private void RPC_ShowReconnectPanel()
    {
        Debug.Log("[DPM_RPC] ShowReconnectPanel recibido.");
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
    private void RPC_PauseGameAll()
    {
        Debug.Log("[DPM_RPC] RPC_PauseGameAll recibido -> pausando localmente.");
        gamePaused = true;
        Time.timeScale = 0f;
    }

    [PunRPC]
    private void RPC_ResumeGameAll()
    {
        gamePaused = false;
        Time.timeScale = 1f;
    }

    [PunRPC]
    private void RPC_ResumeMatch()
    {
        ResumeGameLocal();
    }
}
