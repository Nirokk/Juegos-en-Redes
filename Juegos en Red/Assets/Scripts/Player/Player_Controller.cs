using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    Player_Model _model;
    Player_View _view;



    private void Awake()
    {
       
    }

    private void Start()
    {
        
        _model = GetComponent<Player_Model>();
        _view = GetComponent<Player_View>();

         
        

        

    }

    void Update()
    {
        if (_model._photonView.IsMine )
        {
            _model.Move();
            _model.LookDir();
            
        }
    }


}
