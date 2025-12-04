using UnityEngine;
using Photon.Pun;

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
            !PhotonNetwork.InRoom &&
            PhotonNetwork.NetworkingClient != null &&
            PhotonNetwork.NetworkingClient.LoadBalancingPeer != null;

        panelReconnect.SetActive(canTryReconnect);
        panelErrorReconnect.SetActive(false);
    }


    public void ButtonReconectar()
    {
        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            panelErrorReconnect.SetActive(true);
        }
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
