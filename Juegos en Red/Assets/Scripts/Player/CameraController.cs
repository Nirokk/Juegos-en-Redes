using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Player_Model playerModel;


    private void Awake()
    {
       playerModel = transform.parent.GetComponentInChildren<Player_Model>();

    }
    private void Start()
    {
        if (!playerModel._photonView.IsMine)
        {
            Destroy(this);
        }
    }
    private void Update()
    {
        transform.position = playerModel.transform.position + new Vector3(0, 0, -10);
    }


}
