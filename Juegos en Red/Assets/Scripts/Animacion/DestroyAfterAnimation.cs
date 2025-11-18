using UnityEngine;
using Photon.Pun;
using System.Collections;

public class DestroyAfterAnimation : MonoBehaviourPun
{
    public float lifetime = 0.5f; // lo que dure tu animación

    void Start()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(DestroyAfterTime());
        }
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        PhotonNetwork.Destroy(gameObject);
    }
}
