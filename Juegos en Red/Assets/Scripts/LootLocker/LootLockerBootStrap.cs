using LootLocker.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootLockerBootStrap : MonoBehaviour
{
    public static bool SessionStarted { get; private set; }

    [SerializeField] string playerIdentifier = DateTime.Now.ToString();

    private static LootLockerBootStrap _instance;
    public static LootLockerBootStrap Instance { get => _instance; set => _instance = value; }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        StartGuest();
    }


    void StartGuest()
    {
        LootLockerSDKManager.StartGuestSession(playerIdentifier, response =>
        {
            if (!response.success)
            {
                Debug.LogError("Fallo");
                Debug.LogError(response.errorData.message);
                return;
            }
            SessionStarted = true;
            Debug.Log("Conectado");
        });
    }
    public static void SetPlayerName(string name)
    {
        LootLockerSDKManager.SetPlayerName(name, resp =>
        {
            if (!resp.success) Debug.LogError("Fallo nombre");
            else Debug.Log("Se puso el nombre");
        });
    }
    public static void SubmitScore(int score, string leaderboardKey, System.Action<bool> onDone = null)
    {
        LootLockerSDKManager.SubmitScore("", score, leaderboardKey, response =>
        {
            if (!response.success)
            {
                Debug.LogError("Fallo el score");
                onDone?.Invoke(false);

                return;
            }
            Debug.Log("Se envio el score");
            onDone?.Invoke(true);
        });
    }
}

