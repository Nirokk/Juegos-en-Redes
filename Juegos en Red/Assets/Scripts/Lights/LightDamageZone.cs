using UnityEngine;
using Photon.Pun;

public class LightDamageZone : MonoBehaviourPun
{
    public int damage = 10;
    public float damageInterval = 1f;

    private float timer = 0;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!PhotonNetwork.IsMasterClient) return;   

        Player_Model target = other.GetComponent<Player_Model>();
        if (target == null) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            //  MODIFIED — correct RPC call with sender info
            target._photonView.RPC("TakeDamage", RpcTarget.All, (int)damage);

            timer = damageInterval;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        timer = 0f; 
    }
}
