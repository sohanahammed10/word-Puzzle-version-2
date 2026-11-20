using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button playButton;
    public Button continueButton;
    public string gameSceneName = "GameScene";

    void Awake()
    {
        if (playButton == null) Debug.LogError("MainMenuController: Play Button ঘর খালি");
        if (continueButton == null) Debug.LogError("MainMenuController: Continue Button ঘর খালি");

        if (playButton != null) playButton.onClick.AddListener(PlayNew);

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueGame);
            continueButton.interactable = PlayerData.SavedLevel > 0;
        }
    }

    void PlayNew()
    {
        PlayerData.SavedLevel = 0;
        SceneManager.LoadScene(gameSceneName);
    }

    void ContinueGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}