using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class Player_Model : MonoBehaviour, IMove_Look
{
    Rigidbody2D _rb;
    [SerializeField] private Light2D _playerLight;
    [SerializeField] private Light2D _playerLight2;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _collider;

    [Header("Photon")]
    public PhotonView _photonView;
    [SerializeField] private TextMeshProUGUI _playerName;
    public bool _banned;

    [Header("Player Stats")]
    public int _maxLife;
    public int _currentLife;
    public float _speed;

    private bool _isDead = false;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody2D>();
        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_collider == null) _collider = GetComponent<Collider2D>();

    }

    private void Start()
    {
        _currentLife = _maxLife;
        DesactivateLights();
        DesactivateName();
    }

    #region Pun methods
    public void BanPlayer()
    {
        Debug.Log("Player Banned");
        _banned = true;
        PhotonNetwork.Disconnect();
    }

    void OnDestroy()
    {
        if (_photonView != null)
            DisconnectionHandler.UnregisterPlayerInstance(_photonView.Owner.ActorNumber);
    }


    private void OnApplicationQuit()
    {
        if (_photonView != null && _photonView.IsMine)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }
    }

    #endregion

    #region Player Movement
    public void Move()
    {
        // Si está muerto, no permitimos mover
        if (_isDead)
        {
            _rb.velocity = Vector2.zero;
            return;
        }
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDir = new Vector2(moveX, moveY).normalized;
        //_rb.velocity = moveDir * _speed;
        //moveDir *= _speed;
        transform.position += (Vector3)(moveDir * _speed * Time.deltaTime);
        
    }

    public void LookDir()
    {
        if (_isDead) return;
        Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 direction = mousePos - new Vector3(0.5f, 0.5f, 0);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    [PunRPC]
    public void TakeDamage(int amount, PhotonMessageInfo info)
    {
        if (_isDead) return;
        _currentLife -= amount;

        if (_currentLife <= 0)
        {
            int killerActorNumber = info.Sender.ActorNumber;
            Die(killerActorNumber);
        }
    }


    private void DesactivateLights()
    {
        if(!_photonView.IsMine)
        {
            _playerLight.enabled = false;
            _playerLight2.enabled = false;
            
        }
    }
    private void DesactivateName()
    {
        if (_photonView.IsMine)
            return; 

        var _myActorId = _photonView.Owner.ActorNumber;
        var _thisPlayer = PhotonNetwork.CurrentRoom.GetPlayer(_myActorId);
        var _myTeam = (string)_thisPlayer.CustomProperties["team"];
        string localTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties["team"];
        if (localTeam != _myTeam)
        {
            _playerName.enabled = false;
        }
        
    }


    private void Die(int killerActorNumber)
    {
        if (!_photonView.IsMine)
            return;

        Debug.Log($"Morí yo ({PhotonNetwork.LocalPlayer.NickName})");

        Player killerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);
        if (killerPlayer == null)
        {
            Debug.LogWarning($"[Die] No encontré al killer con ActorNumber {killerActorNumber}");
            return;
        }

        // Enviar RPC ANTES de destruir
        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null)
        {
            Debug.Log($"[Die] Enviando RPC ReportKillToMaster -> killer:{killerPlayer.NickName} victim:{PhotonNetwork.LocalPlayer.NickName}");
            pv.RPC("ReportKillToMaster", RpcTarget.MasterClient, killerActorNumber, PhotonNetwork.LocalPlayer.ActorNumber);

            // Esperar un frame antes de destruir para que el RPC se envíe
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        _isDead = true;

        // 1. Desactivar visuales y colisiones en TODOS los clientes
        _photonView.RPC("SetPlayerState", RpcTarget.All, false);

        // 2. Esperar tiempo de respawn (ej. 3 segundos)
        yield return new WaitForSeconds(3.0f);

        // 3. Obtener nueva posición del Singleton GameStarter
        string myTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties["team"];
        Vector3 newSpawnPos = GameStarter.Instance.GetRandomSpawnPoint(myTeam);

        // 4. Mover el transform y resetear vida
        transform.position = newSpawnPos;
        _currentLife = _maxLife;
        _isDead = false;

        // 5. Reactivar visuales en TODOS los clientes
        _photonView.RPC("SetPlayerState", RpcTarget.All, true);
    }

    private System.Collections.IEnumerator DestroyAfterRPC()
    {
        yield return new WaitForSeconds(0.1f); // pequeño delay
        PhotonNetwork.Destroy(gameObject);
    }




    [PunRPC]

    public void SetPlayerState(bool isActive)
    {
        // Sprites y Colisionadores
        if (_spriteRenderer != null) _spriteRenderer.enabled = isActive;
        if (_collider != null) _collider.enabled = isActive;

        // Canvas del nombre
        if (_playerName != null) _playerName.enabled = isActive;

        // Luces (Solo si es local o si queremos que otros vean que se apaga la luz)
        // Dependiendo de tu lógica de DesactivateLights, quizás quieras forzar apagado aquí
        if (_playerLight != null) _playerLight.enabled = isActive;
        if (_playerLight2 != null) _playerLight2.enabled = isActive;

        // Si NO es mi jugador, asegúrate de volver a aplicar la lógica de luces/nombre
        // cuando revive (isActive = true) para que no vea luces enemigas, por ejemplo.
        if (isActive)
        {
            DesactivateLights(); // Tu método original para filtrar luces ajenas
            DesactivateName();   // Tu método original para filtrar nombres enemigos
        }
    }

    [PunRPC]
    public void ReportKillToMaster(int killerActorNumber, int victimActorNumber)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        Photon.Realtime.Player killerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);
        Photon.Realtime.Player victimPlayer = PhotonNetwork.CurrentRoom.GetPlayer(victimActorNumber);

        if (killerPlayer == null || victimPlayer == null)
        {
            Debug.LogWarning("[ReportKillToMaster] Killer o Victim no encontrados en la sala.");
            return;
        }

        string killerTeam = killerPlayer.CustomProperties.ContainsKey("team")
            ? (string)killerPlayer.CustomProperties["team"]
            : "Unknown";
        string victimTeam = victimPlayer.CustomProperties.ContainsKey("team")
            ? (string)victimPlayer.CustomProperties["team"]
            : "Unknown";

        Debug.Log($"[ReportKillToMaster] Killer:{killerPlayer.NickName}({killerTeam}) -> Victim:{victimPlayer.NickName}({victimTeam})");

        // 🔧 CORREGIDO: Comparar con "A" y "B" en lugar de "TeamA" y "TeamB"
        int killerTeamIndex = killerTeam == "A" ? 0 : 1;
        int victimTeamIndex = victimTeam == "A" ? 0 : 1;

        ScoreManager.Instance.AddScore(killerTeamIndex, victimTeamIndex);
    }
    #endregion
}
