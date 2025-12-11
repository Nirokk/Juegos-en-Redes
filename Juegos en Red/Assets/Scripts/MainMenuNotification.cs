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

    private void Start()
    {
        
         


        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
        }
    }
    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            panelMessage.SetActive(true); // mostrar el panel al cargar la escena
        }
    }
    private void ClosePanel()
    {
        if (panelMessage != null)
            panelMessage.SetActive(false);
    }
}
