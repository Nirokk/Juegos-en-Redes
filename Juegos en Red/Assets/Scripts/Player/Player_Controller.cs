using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    Player_Model _model;
    Player_View _view;
    PhotonView _photonView;


    private void Awake()
    {
       
    }

    private void Start()
    {
        _photonView= GetComponent<PhotonView>();
        _model = GetComponent<Player_Model>();
        _view = GetComponent<Player_View>();
    }

    void FixedUpdate()
    {
        if (_photonView.IsMine )
        {
            _model.Move();
            _model.LookDir();    
        }
    }


}
