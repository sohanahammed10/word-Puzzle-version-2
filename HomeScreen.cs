using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{
    public Button startButton;
    public Button profileButton;
    public Button settingButton;
    public Button soundButton;
    public GameObject profilePanel;
    public Button closeButton;
    public Button backButton;
    public string nextSceneName = "MainMenu";

    ProfileMenu profileMenu;
    public SettingsPanel settingsPanel;
    public SoundPanel soundPanel;

    // Settings আর Sound পাতা না পেলে নিজেই বানিয়ে নেয়, হাতে কিছু টানতে হয় না
    static T FindOrAdd<T>() where T : Component
    {
        T[] all = Resources.FindObjectsOfTypeAll<T>();
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].gameObject.scene.IsValid()) return all[i];
        }
        GameObject go = GameObject.Find("PanelManager");
        if (go == null) go = new GameObject("PanelManager");
        return go.AddComponent<T>();
    }

    void Awake()
    {
        if (settingsPanel == null) settingsPanel = FindOrAdd<SettingsPanel>();
        if (soundPanel == null) soundPanel = FindOrAdd<SoundPanel>();
        profileMenu = FindOrAdd<ProfileMenu>();

        if (profilePanel == null) Debug.LogError("HomeScreen: Profile Panel ঘর খালি");
        else profilePanel.SetActive(false);

        Hook(startButton, StartGame, "Start Button");
        Hook(profileButton, OpenProfile, "Profile Button");
        Hook(settingButton, OnSetting, "Setting Button");
        Hook(soundButton, OnSound, "Sound Button");
        Hook(closeButton, CloseProfile, "Close Button");
        Hook(backButton, CloseProfile, "Back Button");
    }

    void Hook(Button b, UnityAction action, string label)
    {
        if (b == null)
        {
            Debug.LogError("HomeScreen: " + label + " ঘর খালি");
            return;
        }
        b.onClick.AddListener(AudioManager.PlayClick);
        b.onClick.AddListener(action);
    }

    void StartGame()
    {
        Debug.Log("START চাপা পড়েছে, যাচ্ছে: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }

    void OpenProfile()
    {
        Debug.Log("Profile: খোলা হলো");
        profileMenu.Open();
    }

    void CloseProfile()
    {
        Debug.Log("Profile: বন্ধ হলো");
        profilePanel.SetActive(false);
    }

    void OnSetting()
    {
        Debug.Log("Setting বাটন চাপা পড়েছে");
        if (settingsPanel == null)
        {
            Debug.LogError("HomeScreen: Settings Panel ঘর খালি");
            return;
        }
        settingsPanel.Open();
    }

    void OnSound()
    {
        Debug.Log("Sound বাটন চাপা পড়েছে");
        if (soundPanel == null)
        {
            Debug.LogError("HomeScreen: Sound Panel ঘর খালি");
            return;
        }
        soundPanel.Open();
    }
}