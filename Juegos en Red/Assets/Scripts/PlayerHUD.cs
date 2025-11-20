using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHUD : MonoBehaviour
{
    [Header("Barra de Vida")]
    public Slider healthBar;

    [Header("Granada")]
    public Image grenadeIcon;

    Player_Model playerModel;
    Player_Inventory inv;
    PhotonView pv;

    void Start()
    {
        pv = GetComponentInParent<PhotonView>();

        // Si NO es mi jugador, oculto el HUD
        if (pv == null || !pv.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }

        playerModel = GetComponentInParent<Player_Model>();
        inv = GetComponentInParent<Player_Inventory>();

        if (healthBar != null && playerModel != null)
        {
            healthBar.maxValue = playerModel._maxLife;
            healthBar.value = playerModel._currentLife;
        }
    }

    void Update()
    {
        if (pv == null || !pv.IsMine) return;

        if (playerModel != null)
            healthBar.value = playerModel._currentLife;

        if (inv != null)
            grenadeIcon.enabled = inv.hasGrenade;
    }
}
