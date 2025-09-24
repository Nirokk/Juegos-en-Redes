using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    private float speed;
    private float damage;
    private float lifetime;
    private int ownerId;

    public void Initialize(BulletData data, int shooterId)
    {
        speed = data.speed;
        damage = data.damage;
        lifetime = data.lifetime;
        ownerId = shooterId;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return; // Solo el dueño procesa el impacto

        var target = collision.GetComponent<Player_Model>();
        if (target != null)
        {
            // obtenemos el ActorNumber del dueño de la bala
            int killerActorNumber = photonView.Owner.ActorNumber;

            // Llamamos al RPC de TakeDamage y le pasamos killerActorNumber
            target._photonView.RPC("TakeDamage", RpcTarget.All, (int)damage, killerActorNumber);
        }

        PhotonNetwork.Destroy(gameObject);
    }

}
