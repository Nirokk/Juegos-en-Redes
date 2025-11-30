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
    public Button reconnectButton;
    private const string nicknameKey = "playerNickname";
    private string nickname;

    // Start is called before the first frame update
    void Start()
    {
        connectionButton.onClick.AddListener(ConnectToServer);
        inputField.onValueChanged.AddListener(VerifyName);

        // Si no había partida previa, ocultamos el botón
        if (PlayerPrefs.GetInt("PendingReconnect", 0) == 0)
        {
            reconnectButton.gameObject.SetActive(false);
            return;
        }

        // Si había partida previa, intentar comprobar si la sala existe
        string lastRoom = PlayerPrefs.GetString("LastRoomName", "");

        if (string.IsNullOrEmpty(lastRoom))
        {
            reconnectButton.gameObject.SetActive(false);
            return;
        }

        reconnectButton.gameObject.SetActive(true);
        reconnectButton.onClick.AddListener(() => ReconnectToRoom(lastRoom));
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
        PlayerPrefs.SetInt("PendingReconnect", 0);
        PlayerPrefs.Save();
        NetworkManager.Instance.ConnectToServer(GoToLobby);
        NetworkManager.Instance.SetNickname(nickname);
        LootLockerBootStrap.SetPlayerName(nickname);
        connectionButton.interactable = false;

        reconnectButton.gameObject.SetActive(false);

    }
    public void GoToLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster (MainMenuLauncher)");

        if (!string.IsNullOrEmpty(pendingReconnectRoom))
        {
            Debug.Log("Intentando reconectar a " + pendingReconnectRoom);
            PhotonNetwork.JoinRoom(pendingReconnectRoom);
            pendingReconnectRoom = "";
        }
    }

    public void ReconnectToRoom(string roomName)
    {
        reconnectButton.interactable = false;

        StartCoroutine(ReconnectRoutine(roomName));
    }

    private string pendingReconnectRoom = "";

    private IEnumerator ReconnectRoutine(string roomName)
    {
        pendingReconnectRoom = roomName;

        PhotonNetwork.ConnectUsingSettings();

        // Esperamos hasta que estemos conectados y listos (estamos en Master y podemos JoinRoom)
        while (!PhotonNetwork.IsConnectedAndReady)
            yield return null;
    }



    private void SaveReconnectData()
    {
        PlayerPrefs.SetInt("PendingReconnect", 1);
        PlayerPrefs.SetString("LastRoomName", PhotonNetwork.CurrentRoom.Name);
        PlayerPrefs.Save();
    }

    public override void OnJoinedRoom()
    {
        PlayerPrefs.SetInt("PendingReconnect", 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameScene");
    }


    //Antes usabamos el callback de Photon, pero ahora usamos el de NetworkManager
    //public override void OnConnectedToMaster()
    //{
    //    SceneManager.LoadScene("GameScene");
    //}

}
