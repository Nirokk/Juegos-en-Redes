using UnityEngine;

public class PlayerDisconnectedPanel : MonoBehaviour
{
    [Header("UI")]
    public GameObject disconnectedPanel;
    public UnityEngine.UI.Button closeButton;

    private void Start()
    {
        if (disconnectedPanel != null)
            disconnectedPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(HidePanel);
    }

    public void ShowPanel()
    {
        if (disconnectedPanel != null)
            disconnectedPanel.SetActive(true);
    }
    private void HidePanel()
    {
        if (disconnectedPanel != null)
            disconnectedPanel.SetActive(false);
    }
}
