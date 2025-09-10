using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class CtrlJuegoMP : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nombreJugador;
    PhotonView photonView;
    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }


    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            nombreJugador.text = photonView.Owner.NickName;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
