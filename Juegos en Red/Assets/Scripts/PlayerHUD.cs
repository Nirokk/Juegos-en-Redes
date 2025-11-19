using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerHUD : MonoBehaviour
{
    [Header("Barra de Vida")]
    public Slider healthBar;

    [Header("Granada")]
    public Image grenadeIcon;

    // Referencias
    private Player_Inventory inv;
    private Player_Model playerModel;
    private PhotonView pv;

    void Start()
    {
        pv = GetComponentInParent<PhotonView>();

        // Seguridad: si no es mi jugador oculto el HUD
        if (pv == null || !pv.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }

        // Buscar componentes dentro del mismo Player
        inv = GetComponentInParent<Player_Inventory>();
        playerModel = GetComponentInParent<Player_Model>();

        // Inicializar slider si existe la referencia al player
        if (healthBar != null && playerModel != null)
        {
            healthBar.maxValue = playerModel._maxLife;
            healthBar.value = playerModel._currentLife;
        }
    }

    void Update()
    {
        // Si el HUD fue desactivado (no es local), no hacemos nada
        if (pv == null || !pv.IsMine) return;

        // Actualizar barra de vida
        if (playerModel != null && healthBar != null)
        {
            healthBar.value = playerModel._currentLife;
        }

        // Icono de granada
        if (inv != null && grenadeIcon != null)
        {
            grenadeIcon.enabled = inv.hasGrenade;
        }
    }
}
