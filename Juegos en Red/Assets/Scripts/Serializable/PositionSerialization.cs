using UnityEngine;
using Photon.Pun;

public class PositionSerialization : MonoBehaviourPun, IPunObservable
{
    public Transform playerChild; // referencia al hijo “Player”
    private Vector3 networkPos;
    private float lerpRate = 10f;

    private void Awake()
    {
        // si no arrastrás el hijo desde el inspector, lo buscamos por nombre
        if (playerChild == null)
        {
            playerChild = transform.Find("Player");
        }

        networkPos = playerChild.position;
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
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerChild.position);
        }
        else
        {
            networkPos = (Vector3)stream.ReceiveNext();
        }
    }
}