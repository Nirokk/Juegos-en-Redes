using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject panelMenuPlayer;
    private PhotonView localView;

    private void Start()
    {
        // Buscamos al jugador local una vez que ya está spawneado
        Invoke(nameof(FindLocalPlayer), 1f);
    }

    private void FindLocalPlayer()
    {
        foreach (var view in FindObjectsOfType<PhotonView>())
        {
            if (view.IsMine)
            {
                localView = view;
                break;
            }
        }
    }

    private void Update()
    {
        if (localView == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePlayerMenu();
    }

    private void TogglePlayerMenu()
    {
        panelMenuPlayer.SetActive(!panelMenuPlayer.activeSelf);
    }

}
