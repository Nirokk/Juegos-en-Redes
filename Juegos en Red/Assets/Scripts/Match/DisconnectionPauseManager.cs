using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class DisconnectionPauseManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panel")]
    public GameObject panelForceMatchEnded;

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("[DPM] Un jugador se desconectó: " + otherPlayer.NickName);

        // Solo el MasterClient decide finalizar la partida
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_ForceEndMatch), RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_ForceEndMatch()
    {
        // Pausa total
        Time.timeScale = 0f;

        if (MatchTimer.Instance != null)
        {
            MatchTimer.Instance.StopTimerCompletely();
        }

        Debug.Log("[DPM] RPC_ForceEndMatch ejecutado → Pausando partida y mostrando panel.");

 
        // Mostrar panel final
        if (panelForceMatchEnded != null)
            panelForceMatchEnded.SetActive(true);


    }
}
