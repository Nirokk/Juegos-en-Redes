using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    private float speed;
    private float damage;
    private float lifetime;

    public void Initialize(BulletData data)
    {
        speed = data.speed;
        damage = data.damage;
        lifetime = data.lifetime;

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ⚠️ Ahora solo el Master procesa impactos
        if (!PhotonNetwork.IsMasterClient) return;

        var target = collision.GetComponent<Player_Model>();
        if (target != null)
        {
            target._photonView.RPC("TakeDamage", RpcTarget.All, (int)damage, photonView.Owner.ActorNumber);
        }

        if (!collision.CompareTag("Luz"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
