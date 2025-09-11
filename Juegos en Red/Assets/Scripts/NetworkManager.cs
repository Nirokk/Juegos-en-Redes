using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager :  MonoBehaviour
{

    private static NetworkManager _instance;
    public static NetworkManager Instance { get => _instance; set => _instance = value; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void ConnectToServer(string nickname)
    {
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.ConnectUsingSettings();
        
    }
}
