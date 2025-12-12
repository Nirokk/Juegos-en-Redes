using UnityEngine;
using LootLocker.Requests;
using System;

public class LootLockerBootStrap : MonoBehaviour
{
    public static bool SessionStarted { get; private set; }
    public static LootLockerBootStrap Instance { get; private set; }

    public static int playerID; // 👈 ID real de LootLocker
    private string playerIdentifier; // Usado solo para iniciar sesión

    private void Awake()
    {
        // Singleton seguro
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartGuest();
    }

   

    void StartGuest()
    {
        LootLockerSDKManager.StartGuestSession(playerIdentifier, response =>
        {
            if (!response.success)
            {
                Debug.LogError("[LootLocker] Error al iniciar sesión: " + response.errorData.message);
                return;
            }

            // GUARDAR ID REAL DEL PLAYER DEVUELTO POR LOOTLOCKER 👇
            playerID = response.player_id;
            //PlayerPrefs.SetInt("LL_PLAYER_ID", playerID);
            //PlayerPrefs.Save();

            SessionStarted = true;
            Debug.Log($"[LootLocker] Sesión iniciada. Player ID real: {playerID}");
        });
    }

    public static void SetPlayerName(string name)
    {
        if (!SessionStarted)
        {
            Debug.LogWarning("[LootLocker] No se puede asignar nombre, sesión no iniciada aún.");
            return;
        }

        LootLockerSDKManager.SetPlayerName(name, resp =>
        {
            if (!resp.success) Debug.LogError("[LootLocker] Error asignando nombre");
            else Debug.Log("[LootLocker] Nombre asignado correctamente");
        });
    }

    public static void SubmitScore(int KilleNumber,int score, string leaderboardKey, Action<bool> onDone = null)
    {
        Debug.LogError("my killernumber is" + KilleNumber + "my playerid is:" + playerID);
        if(KilleNumber == playerID)
        {
            LootLockerSDKManager.SubmitScore(playerID.ToString(), score, leaderboardKey, response =>
            {
                if (!response.success)
                {
                    Debug.LogError("[LootLocker] Fallo el score");
                    onDone?.Invoke(false);
                    return;
                }
                Debug.Log("[LootLocker] Score enviado correctamente!");
                onDone?.Invoke(true);
            });
        }
        // Solo playerID real!
        
    }
}

