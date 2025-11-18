using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHUD : MonoBehaviour
{
    public Image grenadeIcon;
    private Player_Inventory inv;
    private PhotonView pv;

    void Start()
    {
        pv = GetComponentInParent<PhotonView>();

        // Si NO es mi jugador = oculto el HUD
        if (!pv.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }

        // Buscar inventario DENTRO de mi player
        inv = GetComponentInParent<Player_Inventory>();
    }

    void Update()
    {
        if (inv != null)
            grenadeIcon.enabled = inv.hasGrenade;
    }
}
