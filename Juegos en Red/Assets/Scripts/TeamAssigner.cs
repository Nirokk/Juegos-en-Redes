using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TeamAssigner : MonoBehaviourPunCallbacks
{
    public override void OnJoinedRoom()
    {
        AssignTeam(PhotonNetwork.LocalPlayer);
    }

    void AssignTeam(Player player)
    {
        int actorNumber = player.ActorNumber; // número único que Photon asigna

        string team = (actorNumber % 2 == 0) ? "TeamA" : "TeamB";

        // Guardar el team en las CustomProperties del jugador
        var props = new ExitGames.Client.Photon.Hashtable();
        props["team"] = team;
        player.SetCustomProperties(props);

        Debug.Log($"Jugador {player.NickName} (ActorNumber {actorNumber}) asignado a {team}");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("team"))
        {
            Debug.Log($"Jugador {targetPlayer.NickName} está en {changedProps["team"]}");
        }
    }
}
