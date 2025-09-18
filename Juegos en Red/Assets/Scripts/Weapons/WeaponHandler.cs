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

    void Start()
    {
        bulletsLeft = currentWeapon.magazineSize;
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
