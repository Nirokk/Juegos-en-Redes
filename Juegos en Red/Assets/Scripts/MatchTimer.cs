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

    private bool matchEnded = false;

    public static bool damagePhase = false;

    public static Action OnMatchEnded;

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

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MatchStartTime"))
        {
            startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["MatchStartTime"];
            double elapsed = PhotonNetwork.Time - startTime;
            double remaining = matchDuration - elapsed;

            if (remaining > 0)
            {
                UpdateTimerUI(remaining);
            }

            if (remaining <= 30)
            {
                damagePhase = true;
            }

            if (remaining <= 0)
            {
                EndMatch();
            }

            
            //else
            //{
            //    EndMatch();
            //}
        }
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

        OnMatchEnded?.Invoke();

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
