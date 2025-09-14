using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Asteroids;

public class WeaponHandler : MonoBehaviourPun
{
    [Header("Weapon")]
    public WeaponData currentWeapon;

    private float nextFireTime = 0f;
    private int bulletsLeft;
    private bool isReloading = false;

    private BulletPool pool;

    void Start()
    {
        bulletsLeft = currentWeapon.magazineSize;
        pool = GetComponentInChildren<BulletPool>(); // pool del jugador
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetMouseButton(0) && !isReloading)
            TryShoot();

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && bulletsLeft < currentWeapon.magazineSize)
            StartCoroutine(Reload());
    }

    void TryShoot()
    {
        if (Time.time < nextFireTime) return;

        if (bulletsLeft > 0)
        {
            Shoot();
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        nextFireTime = Time.time + 1f / currentWeapon.fireRate;
        bulletsLeft--;

        GameObject bullet = pool.GetBullet(transform.position, transform.rotation);

        if (bullet != null)
            bullet.GetComponent<Bullet>().Initialize(currentWeapon.bulletType);
    }

    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        bulletsLeft = currentWeapon.magazineSize;
        isReloading = false;
    }
}
