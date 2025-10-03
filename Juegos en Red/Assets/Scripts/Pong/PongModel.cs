using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PongModel : MonoBehaviour
{
    Rigidbody2D _rb;

    [Header("Photon")]
    public PhotonView _photonView;

    [Header("Player Stats")]
    public float _speed;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody2D>();  
    }
    public void Move()
    {
        //float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDir = new Vector2(0, moveY).normalized;
        
        //_rb.velocity = moveDir * _speed;
        //moveDir *= _speed;
        transform.position += (Vector3)(moveDir * _speed * Time.deltaTime); 
    }

}
