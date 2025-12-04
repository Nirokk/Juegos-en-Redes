using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectionHandler : MonoBehaviour
{
    

    public static Dictionary<int, GameObject> playerInstances = new Dictionary<int, GameObject>();


    
    public static void RegisterPlayerInstance(int actorNumber, GameObject playerObj)
    {
        if (!playerInstances.ContainsKey(actorNumber))
        {
            playerInstances.Add(actorNumber, playerObj);
            Debug.Log($"Registrado player {actorNumber} en DisconnectionHandler.");
        }
    }

    // Quitar registro (se llama automáticamente cuando el objeto se destruye)
    public static void UnregisterPlayerInstance(int actorNumber)
    {
        if (playerInstances.ContainsKey(actorNumber))
        {
            playerInstances.Remove(actorNumber);
            Debug.Log($"Unregistrado player {actorNumber}");
        }
    }


    // Photon llama esto automáticamente cuando alguien se desconecta
    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Jugador se desconectó: {otherPlayer.NickName} ({otherPlayer.ActorNumber})");

        if (playerInstances.TryGetValue(otherPlayer.ActorNumber, out GameObject playerObj))
        {
            Debug.Log("Encontré su objeto en escena. Procedo a destruirlo.");

            PhotonNetwork.Destroy(playerObj);
            playerInstances.Remove(otherPlayer.ActorNumber);
        }
        else
        {
            Debug.LogWarning(" No se encontró el objeto del jugador desconectado. Revisa si lo registraste.");
        }
    }
    private void OnApplicationQuit()
    {

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }
}

