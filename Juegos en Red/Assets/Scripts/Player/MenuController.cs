using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject panelMenuPlayer;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePlayerMenu();
    }

    private void TogglePlayerMenu()
    {
        panelMenuPlayer.SetActive(!panelMenuPlayer.activeSelf);
    }
}
