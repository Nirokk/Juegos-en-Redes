using UnityEngine;
using Photon.Pun;

public class PositionSerialization : MonoBehaviourPun, IPunObservable
{
    public Transform playerChild; // referencia al hijo “Player”
    private Vector3 networkPos;
    private Vector3 networkRot;
    private float lerpRate = 10f;

    private void Awake()
    {
        // si no arrastrás el hijo desde el inspector, lo buscamos por nombre
        if (playerChild == null)
        {
            playerChild = transform.Find("Player");
            
        }

        networkPos = playerChild.position;
        networkRot = playerChild.eulerAngles;
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            // interpolación suave del hijo
            playerChild.position = Vector3.Lerp(
                playerChild.position,
                networkPos,
                Time.deltaTime * lerpRate
            );
            playerChild.eulerAngles = Vector3.Lerp(
                playerChild.eulerAngles,
                networkRot,
                Time.deltaTime * lerpRate
            );
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerChild.position);
            stream.SendNext(playerChild.eulerAngles);
        }
        else
        {
            networkPos = (Vector3)stream.ReceiveNext();
            networkRot = (Vector3)stream.ReceiveNext();
        }
    }
}