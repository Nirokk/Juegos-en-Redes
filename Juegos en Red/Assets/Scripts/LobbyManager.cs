using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LobbyManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private TMP_InputField _roomName;
    [SerializeField] private Button _createRoom;
    private bool _joinedToLobby;

    // Start is called before the first frame update
    void Start()
    {
        _createRoom.onClick.AddListener(CreateRoomOnClicked);
        _roomName.onValueChanged.AddListener(VerifyName);
        NetworkManager.Instance.JoinLobby();
        NetworkManager.Instance.OnJoinedRoom += GoToTeamSelection;

    }

    private void VerifyName(string newName)
    {
        if (_roomName.text.Length == 0)
        {
            _createRoom.interactable = false;
        }
        if (_roomName.text.Length >= 1 && !_createRoom.interactable && _joinedToLobby)
        {
            _createRoom.interactable = true;
        }
        _roomName.text = newName;
    }

    private void OnJoinLobby()
    {
        _joinedToLobby = true;
    }

    private void CreateRoomOnClicked()
    {  
        NetworkManager.Instance.CreateRoom(_roomName.text);
        _createRoom.interactable = false;
    }

    private void GoToTeamSelection()
    {
        NetworkManager.Instance.LoadSceneForEveryone("TeamSelection");
        //SceneManager.LoadScene("GameScene");
        //mandar ambos players al team selection pero para la segunda entrega duh
        
    }
}
