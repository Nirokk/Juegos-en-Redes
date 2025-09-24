using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TempTeamAssigner : MonoBehaviourPunCallbacks
{
    public static TempTeamAssigner Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Devuelve el "team" temporal según el ActorNumber
    public int GetTeam(Player player)
    {
        // impar = Team A (0), par = Team B (1)
        return (player.ActorNumber % 2 == 0) ? 1 : 0;
    }

    // Para el local
    public int GetMyTeam()
    {
        return GetTeam(PhotonNetwork.LocalPlayer);
    }
}
