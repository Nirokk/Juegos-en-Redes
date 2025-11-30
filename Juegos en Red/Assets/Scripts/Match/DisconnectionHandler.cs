using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DisconnectionHandler : MonoBehaviourPunCallbacks
{
    [Header("Tiempos (segundos)")]
    public float waitForReconnectTime = 20f;
    public float continueMatchCountdown = 5f;

    [Header("Paneles UI")]
    public GameObject reconnectPanel;
    public TextMeshProUGUI reconnectTimerText;

    public GameObject continuePanel;
    public TextMeshProUGUI continueTimerText;

    public GameObject matchEndedPanel;
    public GameObject matchEndedButton;

    public static double pausedElapsed = -1;
    public static bool matchPaused = false;

    public static Dictionary<int, GameObject> playerInstances = new Dictionary<int, GameObject>();

    public static void RegisterPlayerInstance(int actorNumber, GameObject playerObj)
    {
        if (!playerInstances.ContainsKey(actorNumber))
            playerInstances.Add(actorNumber, playerObj);
    }

    public static void UnregisterPlayerInstance(int actorNumber)
    {
        if (playerInstances.ContainsKey(actorNumber))
            playerInstances.Remove(actorNumber);
    }

    // DESCONEXIÓN
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Jugador desconectado: {otherPlayer.NickName} ({otherPlayer.ActorNumber})");

        if (playerInstances.TryGetValue(otherPlayer.ActorNumber, out GameObject obj))
        {
            PhotonNetwork.Destroy(obj);
            playerInstances.Remove(otherPlayer.ActorNumber);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(HandleDisconnectRoutine(otherPlayer));
        }
    }

    // RUTINA PRINCIPAL
    private IEnumerator HandleDisconnectRoutine(Player disconnectedPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            matchPaused = true;

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MatchStartTime"))
            {
                double startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["MatchStartTime"];
                pausedElapsed = PhotonNetwork.Time - startTime;
            }
        }
        MatchTimer.isPausedByDisconnection = true;
        Time.timeScale = 0f;

        reconnectPanel.SetActive(true);

        float timer = waitForReconnectTime;

        while (timer > 0f)
        {
            reconnectTimerText.text = Mathf.Ceil(timer).ToString("0");
            timer -= Time.unscaledDeltaTime;

            // Volvió un jugador
            if (PhotonNetwork.PlayerList.Length > CountPlayers())
                break;

            yield return null;
        }

        reconnectPanel.SetActive(false);

        // SI VOLVIÓ
        if (PhotonNetwork.PlayerList.Length > CountPlayers())
        {
            ResumeMatch();
            yield break;
        }

        // SI NO VUELVE el jugador desconectado entonces evaluar equipos
        if (TeamAHasPlayers() && TeamBHasPlayers())
        {
            yield return StartCoroutine(ContinueMatchRoutine());
        }
        else
        {
            EndMatchForEveryone();
        }
    }

    // CONTINUAR PARTIDA
    private IEnumerator ContinueMatchRoutine()
    {
        continuePanel.SetActive(true);

        float t = continueMatchCountdown;

        while (t > 0f)
        {
            continueTimerText.text = Mathf.Ceil(t).ToString("0");
            t -= Time.unscaledDeltaTime;
            yield return null;
        }

        continuePanel.SetActive(false);
        ResumeMatch();
    }

    private void ResumeMatch()
    {
        if (PhotonNetwork.IsMasterClient && matchPaused && pausedElapsed >= 0)
        {
            double newStartTime = PhotonNetwork.Time - pausedElapsed;

            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            props["MatchStartTime"] = newStartTime;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }

        matchPaused = false;
        MatchTimer.isPausedByDisconnection = false;
        pausedElapsed = -1;

        Time.timeScale = 1f;
    }

    // FIN DE PARTIDA
    private void EndMatchForEveryone()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        matchEndedPanel.SetActive(true);

        matchEndedButton.SetActive(true);
        matchEndedButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        matchEndedButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            PhotonNetwork.LoadLevel("LobbyScene");
        });
    }

    // HELPERS
    private int CountPlayers()
    {
        return PhotonNetwork.PlayerList.Length;
    }

    private bool TeamAHasPlayers()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("team", out object t) && (string)t == "A")
                return true;
        }
        return false;
    }

    private bool TeamBHasPlayers()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.CustomProperties.TryGetValue("team", out object t) && (string)t == "B")
                return true;
        }
        return false;
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("PendingReconnect", 1); //Esto declara que había una partida en curso
        PlayerPrefs.SetString("LastRoomName", PhotonNetwork.CurrentRoom.Name);
        PlayerPrefs.Save();
        //PhotonNetwork.LeaveRoom();
        //PhotonNetwork.Disconnect();
    }
}
