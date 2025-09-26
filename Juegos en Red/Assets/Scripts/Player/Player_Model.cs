using Photon.Pun;
using Photon.Realtime;
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

        Vector2 moveDir = new Vector2(moveX, moveY).normalized;
        _rb.velocity = moveDir * _speed;
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

    private void Die(int killerActorNumber)
    {
        if (!_photonView.IsMine) return;

        Debug.Log("Morí yo");

        // Victim team
        string victimTeam = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("team")
            ? PhotonNetwork.LocalPlayer.CustomProperties["team"].ToString()
            : "TeamA";

        // Killer
        Player killerPlayer = PhotonNetwork.CurrentRoom.GetPlayer(killerActorNumber);
        if (killerPlayer == null)
        {
            Debug.LogWarning($"? No encontré al killer con ActorNumber {killerActorNumber}");
            return;
        }

        string killerTeam = killerPlayer.CustomProperties.ContainsKey("team")
            ? killerPlayer.CustomProperties["team"].ToString()
            : "TeamA";

        int victimIndex = victimTeam == "TeamA" ? 0 : 1;
        int killerIndex = killerTeam == "TeamA" ? 0 : 1;

        // Score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(killerIndex, victimIndex);

            PhotonView masterPV = ScoreManager.Instance.GetComponent<PhotonView>();
            if (masterPV != null)
            {
                masterPV.RPC(
                    "ReportKillToMaster",
                    RpcTarget.MasterClient,
                    killerActorNumber,               // killer
                    _photonView.Owner.ActorNumber    // victim
                );
            }
        }
        else
        {
            Debug.LogError("? ScoreManager.Instance es NULL en esta escena.");
        }

        // "Muerte" ? desactivar
        gameObject.SetActive(false);
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
