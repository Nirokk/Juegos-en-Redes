using Photon.Pun;
using UnityEngine;
using System.Collections;

public class GrenadeProjectile : MonoBehaviourPun
{
    Rigidbody2D rb;
    Vector2 dir;
    int ownerActorID;

    public float speed = 8f;
    public float fuseTime = 2f;
    public float explosionRadius = 2.5f;
    public int damage = 40;

    public void Initialize(Vector2 direction, int actorID)
    {
        dir = direction.normalized;
        ownerActorID = actorID;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = dir * speed;
        StartCoroutine(Countdown());
    }

    public void SetOwner(int actorID)
    {
        ownerActorID = actorID;
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    void Explode()
    {
        if (photonView.IsMine)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

            foreach (var hit in hits)
            {
                Player_Model model = hit.GetComponentInParent<Player_Model>(); // <<< ESTE ES EL FIX

                if (model != null)
                {
                    PhotonView pv = model._photonView; // o GetComponentInParent<PhotonView>()
                    pv.RPC("TakeDamage", RpcTarget.All, damage);
                }
            }
            // Antes de destruir la granada:
            PhotonNetwork.Instantiate("ExplosionVFX", transform.position, Quaternion.identity);
        }

        PhotonNetwork.Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
