using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour
{
    public Button playButton;
    public Button continueButton;
    public Button backButton;
    public string gameSceneName = "GameScene";
    public string homeSceneName = "SampleScene";

    void Awake()
    {
        if (playButton == null) Debug.LogError("MainMenuScreen: Play Button ঘর খালি");
        else playButton.onClick.AddListener(PlayNew);

        if (continueButton == null) Debug.LogError("MainMenuScreen: Continue Button ঘর খালি");
        else
        {
            continueButton.onClick.AddListener(ContinueGame);
            continueButton.interactable = PlayerData.SavedLevel > 0;
        }

        if (backButton == null) Debug.LogError("MainMenuScreen: Back Button ঘর খালি");
        else backButton.onClick.AddListener(GoBack);
    }

    void PlayNew()
    {
        Debug.Log("PLAY চাপা পড়েছে: নতুন খেলা");
        PlayerData.SavedLevel = 0;
        SceneManager.LoadScene(gameSceneName);
    }

    void ContinueGame()
    {
        Debug.Log("CONTINUE চাপা পড়েছে");
        SceneManager.LoadScene(gameSceneName);
    }

    void GoBack()
    {
        Debug.Log("BACK চাপা পড়েছে");
        SceneManager.LoadScene(homeSceneName);
    }
}