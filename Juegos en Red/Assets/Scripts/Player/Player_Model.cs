using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player_Model : MonoBehaviour, IMove_Look
{
    Rigidbody2D _rb;
    [SerializeField] private Light2D _playerLight;
    [SerializeField] private Light2D _playerLight2;

    [Header("Photon")]
    public PhotonView _photonView;
    [SerializeField] private TextMeshProUGUI _playerName;
    public bool _banned;

    [Header("Player Stats")]
    public int _maxLife;
    public int _currentLife;
    public float _speed;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody2D>();
       
       

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
    #endregion

    #region Player Movement
    public void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDir = new Vector2(moveX, moveY).normalized;
        //_rb.velocity = moveDir * _speed;
        //moveDir *= _speed;
        transform.position += (Vector3)(moveDir * _speed * Time.deltaTime);
        
    }

    public void LookDir()
    {
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
        if (!_photonView.IsMine) return;

        Debug.Log("Morí yo");

        // Equipo de la víctima
        string victimTeam = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("team")
            ? PhotonNetwork.LocalPlayer.CustomProperties["team"].ToString()
            : "TeamA";

        // Obtener al killer
        Player killerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);
        if (killerPlayer == null)
        {
            Debug.LogWarning($"No encontré al killer con ActorNumber {killerActorNumber}");
            return;
        }

        string killerTeam = killerPlayer.CustomProperties.ContainsKey("team")
            ? killerPlayer.CustomProperties["team"].ToString()
            : "TeamA";

        //// 🔹 Sumar kills al killer
        //var killerProps = new ExitGames.Client.Photon.Hashtable();
        //int currentKills = killerPlayer.CustomProperties.ContainsKey("kills")
        //    ? (int)killerPlayer.CustomProperties["kills"]
        //    : 0;
        //killerProps["kills"] = currentKills + 1;
        //killerPlayer.SetCustomProperties(killerProps);

        //// 🔹 Actualizar score del equipo
        //if (ScoreManager.Instance != null)
        //{
        //    int killerTeamIndex = killerTeam == "TeamA" ? 0 : 1;
        //    int victimTeamIndex = victimTeam == "TeamA" ? 0 : 1;
        //    ScoreManager.Instance.AddScore(killerTeamIndex, victimTeamIndex);
        //}

        //Avisar al master quién mató a quién
        _photonView.RPC("ReportKillToMaster", RpcTarget.MasterClient, killerActorNumber, PhotonNetwork.LocalPlayer.ActorNumber);

        // 🔹 Destruir al jugador muerto (solo él mismo)
        PhotonNetwork.Destroy(gameObject);
    }



    [PunRPC]
    public void ReportKillToMaster(int killerActorNumber, int victimActorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Player killerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);
        Player victimPlayer = PhotonNetwork.CurrentRoom.GetPlayer(victimActorNumber);

        if (killerPlayer == null || victimPlayer == null) return;

        string killerTeam = killerPlayer.CustomProperties.ContainsKey("team")
            ? killerPlayer.CustomProperties["team"].ToString()
            : "TeamA";

        string victimTeam = victimPlayer.CustomProperties.ContainsKey("team")
            ? victimPlayer.CustomProperties["team"].ToString()
            : "TeamA";

        int killerIndex = killerTeam == "TeamA" ? 0 : 1;
        int victimIndex = victimTeam == "TeamA" ? 0 : 1;

        ScoreManager.Instance?.AddScore(killerIndex, victimIndex);
    }
    #endregion
}
