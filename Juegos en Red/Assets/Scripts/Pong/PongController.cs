using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongController : MonoBehaviour
{
    PongModel _model;
    PhotonView _photonView;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        _model = GetComponent<PongModel>();
    }
    void FixedUpdate()
    {
        if (_photonView.IsMine)
        {

            _model.Move();
            
        }
    }
}
