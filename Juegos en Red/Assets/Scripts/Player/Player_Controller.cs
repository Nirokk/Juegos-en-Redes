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
        Debug.Log(_model);
    }

    void FixedUpdate()
    {
        if (_photonView.IsMine )
        {
            Debug.Log("Is Mine");
            _model.Move();
            _model.LookDir();    
        }
    }


}
