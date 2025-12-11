using UnityEngine;
using Photon.Pun;
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


    // BOTÓN RECONNECT
    public void ButtonReconectar()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (NetworkManager.wasInMatchBefore && !string.IsNullOrEmpty(NetworkManager.lastRoomName))
            {
                Debug.Log("Intentando RejoinRoom: " + NetworkManager.lastRoomName);
                PhotonNetwork.RejoinRoom(NetworkManager.lastRoomName);
                // NO cargar escena acá. Esperar OnJoinedRoom.
            }
            else ShowErrorPanel();
        }
        else
        {
            if (!PhotonNetwork.ReconnectAndRejoin())
                ShowErrorPanel();
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
