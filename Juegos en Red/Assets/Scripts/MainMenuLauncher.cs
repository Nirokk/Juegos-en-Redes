using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuLauncher : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputField;
    public Button connectionButton;
    private const string nicknameKey = "playerNickname";
    private string nickname;

    // Start is called before the first frame update
    void Start()
    {
        connectionButton.onClick.AddListener(ConnectToServer);
        inputField.onValueChanged.AddListener(VerifyName);
        
    }

    private void VerifyName(string newName)
    {
        if (inputField.text.Length == 0)
        {
            connectionButton.interactable = false;
        }

        if (inputField.text.Length >= 1 && !connectionButton.interactable)
        {
            connectionButton.interactable = true;
        }

        nickname = newName;
    }

    public void ConnectToServer ()
    {

        NetworkManager.Instance.ConnectToServer(GoToLobby);
        NetworkManager.Instance.SetNickname(nickname);
        LootLockerBootStrap.SetPlayerName(nickname);
        connectionButton.interactable = false;

        
    }
    public void GoToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }


    //Antes usabamos el callback de Photon, pero ahora usamos el de NetworkManager
    //public override void OnConnectedToMaster()
    //{
    //    SceneManager.LoadScene("GameScene");
    //}

}
