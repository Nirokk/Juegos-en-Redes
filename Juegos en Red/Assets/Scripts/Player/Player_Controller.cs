using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    Player_Model _model;
    Player_View _view;



    private void Awake()
    {
        if (_model._photonView.IsMine && !_model._banned)
        {
            _model = GetComponent<Player_Model>();
            _view = GetComponent<Player_View>();

        }

    }

    void Update()
    {
        if (_model._photonView.IsMine && !_model._banned)
        {
            _model.Move();
            _model.LookDir();
            
        }
    }


}
