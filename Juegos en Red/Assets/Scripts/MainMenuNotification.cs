using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuNotification : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject panelMessage; // tu panel con TMP

    [Header("Button")]
    public Button closeButton;

    public static MainMenuNotification Instance { get; private set; }

    // Flag para saber si hay que mostrar el panel
    public static bool showNoVotePanel = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
        }

        // Mostrar solo si corresponde
        if (showNoVotePanel)
        {
            panelMessage.SetActive(true);
            showNoVotePanel = false; // reseteamos para la próxima vez
        }
    }

    private void ClosePanel()
    {
        if (panelMessage != null)
            panelMessage.SetActive(false);
    }
}
