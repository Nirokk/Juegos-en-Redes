using UnityEngine;
using Photon.Pun;   

public class MovingLight2D : MonoBehaviourPun, IPunObservable 
{
    public float moveSpeed = 3f;

    private Vector2 targetPos;   
    private bool hasTarget = false;
    public UnityEngine.Rendering.Universal.Light2D spotLight;


    public void SetTarget(Vector2 target)
    {
        targetPos = target;
        hasTarget = true;
    }

    void Update()
    {
        if (!hasTarget) return;

        if (MatchTimer.damagePhase)
            spotLight.color = Color.red;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        //  photon stuff
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            if (PhotonNetwork.IsMasterClient)      
            {
                FindObjectOfType<StadiumLightsManager>().SpawnLight(); 
                PhotonNetwork.Destroy(gameObject);                    
            }
        }
    }

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

