using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MatchTimer : MonoBehaviourPunCallbacks
{
    [Header("Timer Settings")]
    public double matchDuration = 60; // en segundos (5 minutos)
    private double startTime;

    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public GameObject endMatchPanel;
    public TextMeshProUGUI winnerText;
    public Button returnToLobbyButton;

    public static bool isPausedByDisconnection = false;
    private double pausedRemainingTime = -1;

    private bool matchEnded = false;

    public static bool damagePhase = false;

    //public static Action OnMatchEnded;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Si no hay un tiempo de inicio aún, lo creamos
            if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MatchStartTime"))
            {
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props["MatchStartTime"] = PhotonNetwork.Time;
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }

        // Ocultamos panel final al inicio
        if (endMatchPanel != null)
            endMatchPanel.SetActive(false);
    }

    private void Update()
    {
        if (matchEnded) return;

        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MatchStartTime"))
            return;

        startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["MatchStartTime"];

        // Si el juego está pausado por desconexión, NO avanza el timer
        if (isPausedByDisconnection)
        {
            // Guardar el último tiempo restante una sola vez
            if (pausedRemainingTime < 0)
            {
                double elapsedWhenPaused = PhotonNetwork.Time - startTime;
                pausedRemainingTime = matchDuration - elapsedWhenPaused;
            }

            // Mostrar el tiempo congelado mientras está la pausa
            UpdateTimerUI(pausedRemainingTime);
            return;
        }

        // Si NO está pausado, seguimos normalmente
        double elapsed = PhotonNetwork.Time - startTime;
        double remaining = matchDuration - elapsed;

        // Si salimos de la pausa, borramos el tiempo congelado
        pausedRemainingTime = -1;

        if (remaining > 0)
            UpdateTimerUI(remaining);

        if (remaining <= 30)
            damagePhase = true;

        if (remaining <= 0)
            EndMatch();
    }


    private void UpdateTimerUI(double remainingTime)
    {
        int minutes = Mathf.FloorToInt((float)remainingTime / 60f);
        int seconds = Mathf.FloorToInt((float)remainingTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void EndMatch()
    {
        matchEnded = true;

        //OnMatchEnded?.Invoke();
        //Debug.Log("OnMatchEnded INVOKE — Suscriptores: " +
        //  (OnMatchEnded?.GetInvocationList()?.Length ?? 0));

        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
        h["MatchEnded"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(h);


        // Mostramos el panel de fin
        if (endMatchPanel != null)
        {
            endMatchPanel.SetActive(true);
        }

        // Determinar ganador
        string winner = GetWinningTeam();
        winnerText.text = winner;

        // Parar movimiento y lógica del juego (opcional)
        Time.timeScale = 0f;

        // Configurar botón
        if (returnToLobbyButton != null)
        {
            returnToLobbyButton.onClick.RemoveAllListeners();
            returnToLobbyButton.onClick.AddListener(ReturnToLobby);
        }
    }

    private string GetWinningTeam()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogWarning("ScoreManager no encontrado. No se puede determinar el ganador.");
            return "Desconocido";
        }

        int scoreA = ScoreManager.Instance.GetScore(0);
        int scoreB = ScoreManager.Instance.GetScore(1);

        Debug.Log($"[MatchTimer] Puntos finales → A: {scoreA} | B: {scoreB}");

        if (scoreA > scoreB)
            return "A";
        else if (scoreB > scoreA)
            return "B";
        else
            return "Empate";
    }

    private void ReturnToLobby()
    {
        Time.timeScale = 1f;
        PhotonNetwork.AutomaticallySyncScene = true;
        SceneManager.LoadScene("LobbyScene");
    }

    
}
