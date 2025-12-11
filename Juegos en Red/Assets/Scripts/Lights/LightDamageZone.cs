using UnityEngine;
using Photon.Pun;

public class LightDamageZone : MonoBehaviourPun
{
    public int damage = 10;
    public float damageInterval = 1f;

    private float timer = 0;

    private void OnTriggerStay2D(Collider2D other)
    {
        //chequeas tiempo si es 30
        if (!MatchTimer.damagePhase) return;

        //sync
        if (!PhotonNetwork.IsMasterClient) return;   

        //ref jugador
        var targetView = other.GetComponentInParent<PhotonView>();
        if (targetView != null) return;
        
           
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            //RPC
            targetView.RPC("TakeDamage", targetView.Owner, (int)damage, photonView.Owner.ActorNumber);

            timer = damageInterval;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        timer = 0f; 
    }
}
