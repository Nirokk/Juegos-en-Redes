using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameStarter : MonoBehaviourPunCallbacks
{
    // 1. Creamos una instancia estática para acceder desde otros scripts
    public static GameStarter Instance;

    public Transform[] spawnPoints;
    // public Player _player { get; private set; } // No parece necesario exponer esto así, pero lo dejo comentado

    [Header("Spawn Points - Team A")]
    public List<Transform> teamAspawnPointsList;

    [Header("Spawn Points - Team B")]
    public List<Transform> teamBspawnPointsList;

    // Inicializar listas para evitar errores si no se usan
    private List<Player> _playersInAteam = new List<Player>();
    private List<Player> _playersInBteam = new List<Player>();

    private void Awake()
    {
        // 2. Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SpawnPlayer();
    }

    // 3. Método nuevo para que el Player pida un punto de spawn
    public Vector3 GetRandomSpawnPoint(string team)
    {
        if (team == "A" && teamAspawnPointsList.Count > 0)
        {
            return teamAspawnPointsList[Random.Range(0, teamAspawnPointsList.Count)].position;
        }
        else if (team == "B" && teamBspawnPointsList.Count > 0)
        {
            return teamBspawnPointsList[Random.Range(0, teamBspawnPointsList.Count)].position;
        }

        // Fallback por si algo falla, retorna vector zero
        return Vector3.zero;
    }

    public void SpawnPlayer()
    {
        string myTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties["team"];

        
        Vector3 spawnPos = GetRandomSpawnPoint(myTeam);

        if (myTeam == "A" || myTeam == "B")
        {
            GameObject playerObject = PhotonNetwork.Instantiate("NewPlayer", spawnPos, Quaternion.identity);
            DisconnectionHandler.RegisterPlayerInstance(PhotonNetwork.LocalPlayer.ActorNumber, playerObject);

            if (myTeam == "A") _playersInAteam.Add(PhotonNetwork.LocalPlayer);
            else _playersInBteam.Add(PhotonNetwork.LocalPlayer);
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CleanupPlayerObjects(otherPlayer);
    }

    private void CleanupPlayerObjects(Player player)
    {
        
        PhotonView[] allViews = FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in allViews)
        {
            if (view.Owner != null && view.Owner.ActorNumber == player.ActorNumber)
            {
                PhotonNetwork.Destroy(view.gameObject);
            }
        }
        CheckPlayersCount();
    }

    private void CheckPlayersCount()
    {
        NetworkManager.Instance.LoadSceneForEveryone("LobbyScene");
    }
}