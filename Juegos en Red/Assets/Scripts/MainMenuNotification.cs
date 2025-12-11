using UnityEngine;
using UnityEngine.UI;

public class MainMenuNotification : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject panelMessage; // tu panel con TMP

    [Header("Button")]
    public Button closeButton;

    private void Start()
    {
        if (panelMessage != null)
            panelMessage.SetActive(true); // mostrar el panel al cargar la escena

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
        }
    }

    private void ClosePanel()
    {
        if (panelMessage != null)
            panelMessage.SetActive(false);
    }
}
