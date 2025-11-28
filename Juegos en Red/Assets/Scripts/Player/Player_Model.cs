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


    public int personalKills;
    public string playerID;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody2D>();

        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_collider == null) _collider = GetComponent<Collider2D>();
        playerID = LootLockerBootStrap.Instance.playerIdentifier;

    }

    private void Start()
    {
        _currentLife = _maxLife;
        DesactivateLights();
        DesactivateName();
        MatchTimer.OnMatchEnded += SendPlayerKills;
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

        MatchTimer.OnMatchEnded -= SendPlayerKills;
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
        if (!_photonView.IsMine)
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
            pv.RPC("ReportKillToMaster", RpcTarget.All, killerActorNumber, PhotonNetwork.LocalPlayer.ActorNumber);
            //pv.RPC("AddPersonalKill", PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber));
            AddPersonalKill(killerActorNumber);
            // Esperar un frame antes de destruir para que el RPC se envíe
            StartCoroutine(RespawnRoutine());
        }

    }

    private IEnumerator RespawnRoutine()
    {
        _isDead = true;


        _photonView.RPC("SetPlayerState", RpcTarget.All, false);


        yield return new WaitForSeconds(3.0f);


        string myTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties["team"];
        Vector3 newSpawnPos = GameStarter.Instance.GetRandomSpawnPoint(myTeam);


        transform.position = newSpawnPos;
        _currentLife = _maxLife;
        _isDead = false;


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

        if (_spriteRenderer != null) _spriteRenderer.enabled = isActive;
        if (_collider != null) _collider.enabled = isActive;


        if (_playerName != null) _playerName.enabled = isActive;


        if (_playerLight != null) _playerLight.enabled = isActive;
        if (_playerLight2 != null) _playerLight2.enabled = isActive;


        if (isActive)
        {
            DesactivateLights();
            DesactivateName();
        }
    }

    [PunRPC]
    public void ReportKillToMaster(int killerActorNumber, int victimActorNumber)
    {
        Photon.Realtime.Player killerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);
        Photon.Realtime.Player victimPlayer = PhotonNetwork.CurrentRoom.GetPlayer(victimActorNumber);

        if (!PhotonNetwork.IsMasterClient)
            return;

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

    [PunRPC]
    public void AddPersonalKill(int killerNumber)
    {
        if(_photonView.Owner.ActorNumber == killerNumber)
        {
            personalKills++;
            Debug.LogError("Kills personales de " + _photonView.Owner.NickName + ": " + personalKills);
        }
       
    }

    public void SendPlayerKills()
    {
        if (!_photonView.IsMine)
            return;
        else
        {
            Debug.LogError("Enviando kills a LootLocker: " + personalKills);
            LootLockerBootStrap.SubmitScore(playerID ,personalKills, "mostkills", success =>
            {
                if (success)
                {
                    Debug.Log("Puntuación enviada correctamente.");
                }
                else
                {
                    Debug.LogError("Error al enviar la puntuación.");
                }
            });
        }
    }
}
