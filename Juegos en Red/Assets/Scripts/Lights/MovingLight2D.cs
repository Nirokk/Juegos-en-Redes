using UnityEngine;
using Photon.Pun;   

public class MovingLight2D : MonoBehaviourPun, IPunObservable 
{
    public float moveSpeed = 3f;

    private Vector2 targetPos;   
    private bool hasTarget = false; 

    
    public void SetTarget(Vector2 target)
    {
        targetPos = target;
        hasTarget = true;
    }

    void Update()
    {
        if (!hasTarget) return;  

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        //  MODIFIED: Photon destroy + spawn from Master
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            if (PhotonNetwork.IsMasterClient)      
            {
                FindObjectOfType<StadiumLightsManager>().SpawnLight(); 
                PhotonNetwork.Destroy(gameObject);                    
            }
        }
    }

    //  ADDED — Sync the target position
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(targetPos);
        else
        {
            targetPos = (Vector2)stream.ReceiveNext();
            hasTarget = true;
        }
    }
}

