using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;

public class ReconnectButtons : MonoBehaviourPunCallbacks
{
    [Header("Panels")]
    public GameObject panelReconnect;
    public GameObject panelErrorReconnect;

    private void Start()
    {
        CheckIfCanReconnect();
    }

    private void CheckIfCanReconnect()
    {
        bool canTryReconnect =
            NetworkManager.wasInMatchBefore &&
            !PhotonNetwork.InRoom &&
            PhotonNetwork.NetworkingClient != null &&
            PhotonNetwork.NetworkingClient.LoadBalancingPeer != null;

        panelReconnect.SetActive(canTryReconnect);
        panelErrorReconnect.SetActive(false);
    }


    public void ButtonReconectar()
    {
        // Si ya estamos conectados a Photon, probamos Rejoin
        if (PhotonNetwork.IsConnected)
        {
            if (NetworkManager.Instance != null &&
                NetworkManager.wasInMatchBefore &&
                !string.IsNullOrEmpty(NetworkManager.Instance.GetCurrentRoomName()))
            {
                PhotonNetwork.RejoinRoom(NetworkManager.Instance.GetCurrentRoomName());
                SceneManager.LoadScene(NetworkManager.lastGameScene);
            }
            else
            {
                ShowErrorPanel();
            }
        }
        else
        {
            // Caso donde realmente hubo desconexión del servidor usamos ReconnectAndRejoin
            if (!PhotonNetwork.ReconnectAndRejoin())
            {
                ShowErrorPanel();
            }
        }
    }

    private void ShowErrorPanel()
    {
        panelErrorReconnect.SetActive(true);
    }

    public void ButtonExitReconnect()
    {
        panelReconnect.SetActive(false);
    }

    public void ButtonExitError()
    {
        panelErrorReconnect.SetActive(false);
    }
}
