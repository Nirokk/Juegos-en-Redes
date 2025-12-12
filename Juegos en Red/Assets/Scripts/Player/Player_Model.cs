using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
using ExitGames.Client.Photon;

public class Player_Model : MonoBehaviourPunCallbacks, IMove_Look
{
    Rigidbody2D _rb;
    [SerializeField] private Light2D _playerLight;
    [SerializeField] private Light2D _playerLight2;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _collider;

    [Header("Photon")]
    public PhotonView _photonView;
    [SerializeField] private TextMeshProUGUI _playerName;
    private string _myPlayerName;
    public bool _banned;

    [Header("Player Stats")]
    public int _maxLife;
    public int _currentLife;
    public float _speed;

    private bool _isDead = false;

    public int myKillerActorNumber;

    public int personalKills;
    //public string playerID => PlayerPrefs.GetString("LL_ID");

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody2D>();

        if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_collider == null) _collider = GetComponent<Collider2D>();

        Debug.Log($"[Awake] Player_Model instanciado. Owner = {_photonView.Owner?.NickName}  ViewID = {_photonView.ViewID}");

        _myPlayerName = _photonView.Owner.NickName;

    }

    private void Start()
    {
        _currentLife = _maxLife;
        DesactivateLights();
        DesactivateName();

        if (photonView.IsMine)
        {
            // ESTE ES EL CORRECTO
            PhotonNetwork.LocalPlayer.TagObject = this.gameObject;

            Debug.Log($"[TagObject] Seteado correctamente para {PhotonNetwork.LocalPlayer.NickName} → {this.gameObject.name}");
        }
        //Debug.Log("PLAYER_MODEL Start() — SUSCRIBO al evento → " + this.gameObject.name);
        //MatchTimer.OnMatchEnded += SendPlayerKills;
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
        {
            DisconnectionHandler.UnregisterPlayerInstance(_photonView.Owner.ActorNumber);
            //MatchTimer.OnMatchEnded -= SendPlayerKills;
        }



    }


    private void OnApplicationQuit()
    {
        if (_photonView != null && _photonView.IsMine)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }
        //TrySendKills();
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
    public void TakeDamage(int amount, int info)
    {
        if (!_photonView.IsMine) return; // <<< IMPORTANTE

        if (_isDead) return;

        _currentLife -= amount;

        if (_currentLife <= 0)
        {
            int killerActorNumber = info;
            myKillerActorNumber = killerActorNumber;
            Photon.Realtime.Player killerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);
            
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

        Debug.Log($"Morí yo ({PhotonNetwork.LocalPlayer.NickName}), me mató actor {killerActorNumber}");

        Player killerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);
        if (killerPlayer == null)
        {
            Debug.LogWarning($"[Die] No encontré al killer con ActorNumber {killerActorNumber}");
            return;
        }

        // 1) Avisar al master para sumar puntos de equipo
        _photonView.RPC("ReportKillToMaster", RpcTarget.All, killerActorNumber, PhotonNetwork.LocalPlayer.ActorNumber);
        _photonView.RPC("AddPersonalKill", killerPlayer , killerActorNumber);
        StartCoroutine(RespawnRoutine());
    }
    private PhotonView GetPhotonViewByActorNumber(int actorNumber)
    {
        foreach (PhotonView pv in FindObjectsOfType<PhotonView>())
        {
            if (pv.Owner != null && pv.Owner.ActorNumber == actorNumber)
                return pv;
        }
        return null;
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
    [PunRPC]
    public void AddPersonalKill(int killerActorNumber)
    {
        // Buscar todos los Player_Model de escena
        Player_Model[] players = FindObjectsOfType<Player_Model>();

        foreach (var player in players)
        {
            if (player.photonView.OwnerActorNr == killerActorNumber)
            {
                player.personalKills++;
                Debug.Log($"🔥 Kill sumada a {player.photonView.Owner.NickName}. Total: {player.personalKills}");
                return;
            }
        }

        Debug.LogWarning($"❌ No encontré al jugador con ActorNumber {killerActorNumber}");
    }

    public void SaveKillsToLootLocker()
    {
        
            LootLockerBootStrap.SubmitScore(myKillerActorNumber, personalKills, "mostkills", (success) =>
            {
                if (success)
                    Debug.Log($"📌 Kills guardadas = {personalKills}");
                else
                    Debug.LogError("❌ Error enviando kills.");
            });
        
        
        
    }
 #endregion 
}
