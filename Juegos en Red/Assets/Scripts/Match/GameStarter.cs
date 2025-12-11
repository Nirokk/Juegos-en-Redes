using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class GameStarter : MonoBehaviourPunCallbacks
{
    //  Creamos una instancia estática para acceder desde otros scripts
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
        // Configuración del Singleton
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
        //SpawnPlayer();
        //NetworkManager.lastGameScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        //NetworkManager.lastGameScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            Debug.Log("[GameStarter] GameScene cargada → Spawneando player...");
            StartCoroutine(DelayedSpawn());
        }
    }
    private IEnumerator DelayedSpawn()
    {
        // Esperar hasta que Photon confirme ROOM READY
        while (!PhotonNetwork.InRoom)
            yield return null;

        // Esperar 1 frame más por seguridad (nivel Unity cargado)
        yield return null;

        Debug.Log("[GameStarter] DelayedSpawn ejecutado → Spawneando player correctamente.");
        SpawnPlayer();
    }
    public override void OnJoinedRoom()
    {
        // Si la room tiene la propiedad currentScene, forcemos la misma escena.
        if (PhotonNetwork.CurrentRoom != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("currentScene", out object sceneObj))
        {
            string sceneName = (string)sceneObj;

            if (!string.IsNullOrEmpty(sceneName) && SceneManager.GetActiveScene().name != sceneName)
            {
                Debug.Log("[OnJoinedRoom] Room indica escena: " + sceneName + " → cargando sincronizada.");
                PhotonNetwork.LoadLevel(sceneName);
            }
        }
    }


    // Método nuevo para que el Player pida un punto de spawn
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
        if (AlreadyHasPlayerInstance())
        {
            Debug.Log("[GameStarter] Ya existe un player para este actor → NO instancio otro.");
            return;
        }

        Debug.Log("SpawnPlayer() ejecutado.");

        string myTeam = (string)PhotonNetwork.LocalPlayer.CustomProperties["team"];
        Vector3 spawnPos = GetRandomSpawnPoint(myTeam);

        GameObject playerObject = PhotonNetwork.Instantiate("NewPlayer", spawnPos, Quaternion.identity);
        DisconnectionHandler.RegisterPlayerInstance(PhotonNetwork.LocalPlayer.ActorNumber, playerObject);

        if (myTeam == "A") _playersInAteam.Add(PhotonNetwork.LocalPlayer);
        else _playersInBteam.Add(PhotonNetwork.LocalPlayer);
    }
    private bool AlreadyHasPlayerInstance()
    {
        Player_Model[] players = FindObjectsOfType<Player_Model>();

        foreach (var p in players)
        {
            if (p.photonView != null &&
                p.photonView.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                if (!p.gameObject.activeSelf)
                {
                    Debug.Log("[Reconnect] Reactivando player desactivado.");
                    p.gameObject.SetActive(true);
                }

                return true;
            }
        }

        return false;
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
                //PhotonNetwork.Destroy(view.gameObject); //Antes lo destruiamos
                view.gameObject.SetActive(false); //Ahora solo lo desactivamos
            }
        }
        //CheckPlayersCount();
    }

    //private void CheckPlayersCount()
    //{
    //    NetworkManager.Instance.LoadSceneForEveryone("LobbyScene");
    //}
}