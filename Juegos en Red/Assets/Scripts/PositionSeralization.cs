using UnityEngine;
using Photon.Pun;


[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody2D))]
public class PositionSeralization : MonoBehaviourPun, IPunObservable
{
    public Vector3 networkPosition;
    //public Vector3 pos = Vector3.zero;
    //public PhotonView pv;

    private Rigidbody2D _rb;



    private void Start()
    {
        //networkPosition = GetComponent<RectTransform>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            _rb.position = Vector3.MoveTowards(_rb.position, networkPosition, Time.fixedDeltaTime * 10);
        }
    }




    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_rb.position);
            stream.SendNext(_rb.rotation);
            stream.SendNext(_rb.velocity);

        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            _rb.velocity = (Vector3)stream.ReceiveNext();
            
            //networkPosition.position = aux;
            //pos = aux;
        }
    }
}
