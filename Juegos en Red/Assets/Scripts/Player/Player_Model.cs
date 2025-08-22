using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Model : MonoBehaviour, IMove_Look
{
    [Header("Photon")]
    public PhotonView _photonView;
    public TMPro.TextMeshPro _playerName;
    public bool banned;



    [Header("Player Stats")]
    public int _maxLife;
    public int _currentLife;
    public float _speed;
    public Rigidbody2D _rb;



    public void BanPlayer()
    {
        Debug.Log("Player Banned");
        banned = true;
        PhotonNetwork.Disconnect();
    }


    public void Move(Vector3 dir)
    {
        
    }

    public void LookDir(Vector3 dir)
    {
        // Obtener la posición del mouse en el mundo
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f; // Asegúrate de que el z esté en 0 para 2D

        // Calcular la dirección desde el jugador hacia el mouse
        Vector3 direction = mousePosition - transform.position;

        // Calcular el ángulo en grados
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Aplicar la rotación
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
