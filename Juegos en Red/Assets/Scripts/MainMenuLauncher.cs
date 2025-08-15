using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuLauncher : MonoBehaviourPunCallbacks
{


    public Button connectionButton;
    // Start is called before the first frame update
    void Start()
    {
        connectionButton.onClick.AddListener(ConnectToServer);
    }

    public void ConnectToServer ()
    {
        PhotonNetwork.ConnectUsingSettings();
        connectionButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("GameScene");
    }
}
