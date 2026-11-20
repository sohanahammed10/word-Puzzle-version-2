using UnityEngine;
using UnityEngine.UI;

public class ProfilePopup : MonoBehaviour
{
    public GameObject profilePanel;
    public Button profileButton;
    public Button closeButton;
    public Button backButton;

    void Awake()
    {
        if (profilePanel == null) Debug.LogError("ProfilePopup: Profile Panel ঘর খালি");
        if (profileButton == null) Debug.LogError("ProfilePopup: Profile Button ঘর খালি");

        if (profilePanel != null) profilePanel.SetActive(false);
        if (profileButton != null) profileButton.onClick.AddListener(Open);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (backButton != null) backButton.onClick.AddListener(Close);
    }

    void Open()
    {
        Debug.Log("Profile: খোলা হলো");
        profilePanel.SetActive(true);
        profilePanel.transform.SetAsLastSibling();
    }

    void Close()
    {
        Debug.Log("Profile: বন্ধ হলো");
        profilePanel.SetActive(false);
    }
}