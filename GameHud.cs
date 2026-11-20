using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHud : MonoBehaviour
{
    public TMP_Text scoreText;
    public Button backButton;
    public string backSceneName = "MainMenu";

    void Awake()
    {
        if (scoreText == null) Debug.LogError("GameHud: Score Text ঘর খালি");

        if (backButton == null) Debug.LogError("GameHud: Back Button ঘর খালি");
        else backButton.onClick.AddListener(GoBack);
    }

    void Start()
    {
        InvokeRepeating(nameof(RefreshScore), 0f, 0.25f);
    }

    void RefreshScore()
    {
        if (scoreText == null) return;
        scoreText.text = "COINS  " + PlayerData.Coins + "\nSTARS  " + PlayerData.Stars;
    }

    void GoBack()
    {
        Debug.Log("BACK চাপা পড়েছে: " + backSceneName);
        SceneManager.LoadScene(backSceneName);
    }
}