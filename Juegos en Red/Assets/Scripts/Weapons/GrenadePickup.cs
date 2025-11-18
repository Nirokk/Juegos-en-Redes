using Photon.Pun;
using UnityEngine;

public class GrenadePickup : MonoBehaviour, IInteractable
{
    public void Interact(Player_Model player)
    {
        Player_Inventory inv = player.GetComponent<Player_Inventory>();

        if (inv != null)
        {
            inv.GiveGrenade();
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
