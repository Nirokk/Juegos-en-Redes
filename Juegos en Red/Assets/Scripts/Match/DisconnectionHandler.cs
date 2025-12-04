using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class DisconnectionHandler : MonoBehaviourPunCallbacks
{
    public static Dictionary<int, GameObject> playerInstances = new Dictionary<int, GameObject>();

    public static void RegisterPlayerInstance(int actorNumber, GameObject playerObj)
    {
        if (!playerInstances.ContainsKey(actorNumber))
        {
            playerInstances.Add(actorNumber, playerObj);
            Debug.Log($"[DH] Registrado player {actorNumber}");
        }
    }

    public static void UnregisterPlayerInstance(int actorNumber)
    {
        if (playerInstances.ContainsKey(actorNumber))
        {
            playerInstances.Remove(actorNumber);
            Debug.Log($"[DH] Unregistrado player {actorNumber}");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[DH] Jugador se desconectó → {otherPlayer.NickName} ({otherPlayer.ActorNumber})");
    }

}
