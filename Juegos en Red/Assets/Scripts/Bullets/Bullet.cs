using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    private float speed;
    private float damage;
    private float lifetime;
    private float timer;

    public void Initialize(BulletData data)
    {
        speed = data.speed;
        damage = data.damage;
        lifetime = data.lifetime;
        timer = 0f;
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        transform.Translate(Vector2.up * speed * Time.deltaTime);

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return; // solo el dueño hace daño

        var target = collision.GetComponent<Player_Model>();
        if (target != null)
        {
            target._photonView.RPC("TakeDamage", RpcTarget.All, (int)damage);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        // Buscar el pool asociado al dueño de esta bala
        BulletPool pool = photonView.Owner.TagObject as BulletPool;
        if (pool != null)
        {
            pool.ReturnBullet(gameObject);
        }
        else
        {
            // fallback si no encuentra pool
            gameObject.SetActive(false);
        }
    }
}
