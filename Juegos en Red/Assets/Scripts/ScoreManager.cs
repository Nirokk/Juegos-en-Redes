using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public static ScoreManager Instance;

    private const string TEAM_A_SCORE = "TeamAScore";
    private const string TEAM_B_SCORE = "TeamBScore";

    public event System.Action OnScoreUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // persiste entre escenas
        }
        else
        {
            Destroy(gameObject); // evita duplicados si se recarga
        }
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ScoreManager.Instance.InitScores();
        }
    }

    // Inicializa los scores al empezar la partida
    public void InitScores()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable scoreProps = new Hashtable
            {
                { TEAM_A_SCORE, 0 },
                { TEAM_B_SCORE, 0 }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(scoreProps);
        }
    }

    // Llamar cuando un jugador mata a otro
    public void AddScore(int killerTeam, int victimTeam)
    {
        if (!PhotonNetwork.IsMasterClient) return; // solo el master actualiza

        int teamAScore = (int)PhotonNetwork.CurrentRoom.CustomProperties[TEAM_A_SCORE];
        int teamBScore = (int)PhotonNetwork.CurrentRoom.CustomProperties[TEAM_B_SCORE];

        if (killerTeam == victimTeam)
        {
            // Team Kill = restar punto pero no bajar de 0
            if (killerTeam == 0) teamAScore = Mathf.Max(0, teamAScore - 1);
            else teamBScore = Mathf.Max(0, teamBScore - 1);
        }
        else
        {
            // Kill normal = sumar punto
            if (killerTeam == 0) teamAScore++;
            else teamBScore++;
        }

        Hashtable scoreProps = new Hashtable
        {
            { TEAM_A_SCORE, teamAScore },
            { TEAM_B_SCORE, teamBScore }
        };

        if (OnScoreUpdated != null)
            OnScoreUpdated.Invoke();

        PhotonNetwork.CurrentRoom.SetCustomProperties(scoreProps);
    }

    // Método para obtener el score actual de cada equipo
    public int GetScore(int team)
    {
        if (PhotonNetwork.CurrentRoom == null) return 0;

        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (team == 0 && props.ContainsKey(TEAM_A_SCORE))
            return (int)props[TEAM_A_SCORE];
        if (team == 1 && props.ContainsKey(TEAM_B_SCORE))
            return (int)props[TEAM_B_SCORE];

        return 0; // default si no está inicializado
    }

    // Este callback se ejecuta en todos los jugadores cuando cambian propiedades
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(TEAM_A_SCORE) ||
            propertiesThatChanged.ContainsKey(TEAM_B_SCORE))
        {
            Debug.Log($"Score actualizado → A: {GetScore(0)} | B: {GetScore(1)}");
            if (OnScoreUpdated != null)
                OnScoreUpdated.Invoke();
        }
    }
}
