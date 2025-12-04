using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ButtonsGameScene : MonoBehaviour
{
    public GameObject panelMenuPlayer;

    public void ButtonBackToSearchLobby()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }

    public void ButtonDesconectarse()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Main Menu");
    }

    public void ButtonCerrar()
    {
        panelMenuPlayer.SetActive(false);
    }
}
