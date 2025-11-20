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
        Debug.Log(this.gameObject.name);

        if (!photonView.IsMine) return; // Solo el dueño procesa el impacto

        // Ejemplo: si choca contra un jugador
        var target = collision.GetComponent<Player_Model>();
        if (target != null)
        {
            target._photonView.RPC("TakeDamage", RpcTarget.All, (int)damage);
        }

        if (!collision.CompareTag("Luz"))
        {
            PhotonNetwork.Destroy(gameObject);
        }


        //PhotonNetwork.Destroy(gameObject);
    }
}
