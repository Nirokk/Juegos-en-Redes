using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Model : MonoBehaviour, IMove_Look
{
    
    Rigidbody2D _rb;


    [Header("Photon")]
    public PhotonView _photonView;
    [SerializeField] private TMPro.TextMeshPro _playerName;
    public bool _banned;


    [Header("Player Stats")]
    public int _maxLife;
    public int _currentLife;
    public float _speed;
    

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody2D>();
        _playerName = GetComponentInChildren<TMPro.TextMeshPro>();
    }
    private void Start()
    {
        _currentLife = _maxLife;
    }
    #region Pun methods
    public void BanPlayer()
    {
        Debug.Log("Player Banned");
        _banned = true;
        PhotonNetwork.Disconnect();
    }
    #endregion

    #region Player Movement
    public void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // Normalize so diagonal isn’t faster
        Vector2 moveDir = new Vector2(moveX, moveY).normalized;

        // Move with physics
        _rb.velocity = moveDir * _speed;
    }

    public void LookDir()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Direction from player to mouse
        Vector2 direction = mousePos - transform.position;

        // Angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Rotate (adjust -90 if your sprite points up instead of right)
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    [PunRPC]
    public void TakeDamage(int amount)
    {
        _currentLife -= amount;
        print(_currentLife);
        if (_currentLife <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!_photonView.IsMine) return;

        // Avisamos al GameManager que este jugador murió
        GameManager.Instance.photonView.RPC("PlayerDied", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);

        // Desactivar jugador (queda "muerto" hasta la próxima ronda)
        gameObject.SetActive(false);
    }

    #endregion
}
