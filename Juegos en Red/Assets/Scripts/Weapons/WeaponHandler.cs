using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;

public class WeaponHandler : MonoBehaviourPun
{
    [Header("Weapon")]
    public WeaponData currentWeapon;   // Asignás tu pistola desde el inspector

    private float nextFireTime = 0f;
    private int bulletsLeft;
    private bool isReloading = false;

    [Header("Grenade System")]
    public GameObject grenadePrefab;
    private Player_Inventory inventory;

    void Start()
    {
        bulletsLeft = currentWeapon.magazineSize;
        inventory = GetComponentInParent<Player_Inventory>();
        Debug.Log("INVENTORY ENCONTRADO = " + (inventory != null));
    }

    void Update()
    {
        if (!photonView.IsMine) return; // Solo control local

        // Disparo
        if (Input.GetMouseButton(0) && !isReloading) // click izq
        {
            TryShoot();
        }

        // Recarga manual
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && bulletsLeft < currentWeapon.magazineSize)
        {
            StartCoroutine(Reload());
        }

        // Lanzar granada
        if (Input.GetKeyDown(KeyCode.Q) && inventory != null)
        {
            if (inventory.TryUseGrenade())
            {
                Debug.Log("LANZANDO GRANADA!");
                ThrowGrenade();
            }
        }
    }

    void ThrowGrenade()
    {
        Vector3 spawnPos = transform.position + transform.up * 0.5f;

        GameObject grenade = PhotonNetwork.Instantiate(
            grenadePrefab.name,
            spawnPos,
            transform.rotation
        );

        grenade.GetComponent<GrenadeProjectile>().Initialize(transform.up, PhotonNetwork.LocalPlayer.ActorNumber);
    }


    void TryShoot()
    {
        if (Time.time < nextFireTime) return; // respeta fireRate

        if (bulletsLeft > 0)
        {
            Shoot();
        }
        else
        {
            StartCoroutine(Reload()); // recarga automática al vaciar cargador
        }
    }

    void Shoot()
    {
        nextFireTime = Time.time + 1f / currentWeapon.fireRate;
        bulletsLeft--;

        // Instanciar bala en red
        GameObject bullet = PhotonNetwork.Instantiate(
            currentWeapon.bulletType.prefabBullet.name,
            transform.position,
            transform.rotation
        );

        // Asignar stats de la bala
        bullet.GetComponent<Bullet>().Initialize(currentWeapon.bulletType);
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        yield return new WaitForSeconds(currentWeapon.reloadTime);

        bulletsLeft = currentWeapon.magazineSize;
        isReloading = false;

        Debug.Log("Reload Complete");
    }
}
