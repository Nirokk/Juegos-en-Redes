using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletPool : MonoBehaviourPun
{
    [Header("Pool Settings")]
    public BulletData bulletData;
    public int poolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Start()
    {
        // Solo el dueño de este pool crea sus balas
        if (!photonView.IsMine) return;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = PhotonNetwork.Instantiate(
                bulletData.prefabBullet.name,
                Vector3.zero,
                Quaternion.identity
            );

            obj.SetActive(false);

            // Transferimos ownership al jugador dueño de este pool
            obj.GetComponent<PhotonView>().TransferOwnership(photonView.Owner);

            pool.Enqueue(obj);
        }
    }

    public GameObject GetBullet(Vector3 pos, Quaternion rot)
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.SetActive(true);
            return obj;
        }
        return null;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        pool.Enqueue(bullet);
    }
}
