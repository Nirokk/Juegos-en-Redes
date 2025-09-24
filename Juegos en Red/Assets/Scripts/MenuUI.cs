using UnityEngine;
using Photon.Pun;

public class MenuUI : MonoBehaviour
{
    public void LoadGameScene()
    {
        PhotonNetwork.LoadLevel("GameScene"); // carga la escena para todos los jugadores de la sala
    }
}
