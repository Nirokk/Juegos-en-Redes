using Photon.Pun;
using UnityEngine;

public class GrenadePickup : MonoBehaviourPun, IInteractable
{
    public void Interact(Player_Model player)
    {
        // El jugador toma la granada
        player.GetComponent<Player_Inventory>().GiveGrenade();

        // El pickup debe ser destruido SOLO por el MasterClient
        photonView.RPC("RPC_RequestDestroy", RpcTarget.MasterClient);
    }

    [PunRPC]
    void RPC_RequestDestroy()
    {
        // Solo el MasterClient ejecuta esto
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
