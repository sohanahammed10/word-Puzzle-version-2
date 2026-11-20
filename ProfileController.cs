using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Profile পাতার কাজ। Avatar বদলানোর তালিকা কোড দিয়ে নিজেই বানায় (২০টা প্রাণীর ছবি)।
public class ProfileController : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_Text levelText;
    public TMP_Text starsText;
    public TMP_Text coinsText;
    public TMP_Text completedText;
    public TMP_Text wordsText;
    public TMP_Text streakText;
    public TMP_Text achievementsText;
    public Image avatarImage;
    public Button editNameButton;
    public Button changeAvatarButton;

    GameObject chooserRoot;
    Image[] rings;

    void Awake()
    {
        if (nameInput != null)
        {
            nameInput.characterLimit = 14;
            nameInput.onEndEdit.AddListener(OnNameChanged);
        }
        if (editNameButton != null) editNameButton.onClick.AddListener(FocusName);
        if (changeAvatarButton != null) changeAvatarButton.onClick.AddListener(OpenChooser);
    }

    void OnEnable()
    {
        HideChooser();
        Refresh();
    }

    void OnDisable()
    {
        HideChooser();
    }

    void Refresh()
    {
        if (nameInput != null) nameInput.text = PlayerData.Name;
        if (levelText != null)
            levelText.text = "LEVEL " + PlayerData.PlayerLevel + "  |  " + PlayerData.XpInLevel + "/100 XP";
        SetNum(starsText, PlayerData.Stars);
        SetNum(coinsText, PlayerData.Coins);
        SetNum(completedText, PlayerData.CompletedLevels);
        SetNum(wordsText, PlayerData.CompletedLevels);
        SetNum(streakText, PlayerData.BestStreak);
        if (achievementsText != null) achievementsText.text = PlayerData.Achievements + "/7";
        ShowAvatar();
    }

    void SetNum(TMP_Text t, int v)
    {
        if (t != null) t.text = v.ToString();
    }

    void OnNameChanged(string value)
    {
        value = value.Trim();
        if (value.Length == 0) value = "Player";
        PlayerData.Name = value;
        nameInput.text = value;
    }

    void FocusName()
    {
        if (nameInput == null) return;
        nameInput.Select();
        nameInput.ActivateInputField();
    }

    // ---------- Avatar ----------
    void ShowAvatar()
    {
        if (avatarImage == null) return;
        int i = PlayerData.AvatarIndex;
        if (i < 0 || i >= AvatarArt.Count) i = 0;   // এখনো না বাছলে প্রথমটা (বিড়াল)
        avatarImage.enabled = true;
        avatarImage.sprite = AvatarArt.Get(i);
        avatarImage.color = Color.white;
        avatarImage.preserveAspect = true;
    }

    void OpenChooser()
    {
        if (chooserRoot == null) BuildChooser();
        RefreshRings();
        chooserRoot.SetActive(true);
        chooserRoot.transform.SetAsLastSibling();
    }

    void HideChooser()
    {
        if (chooserRoot != null) chooserRoot.SetActive(false);
    }

    void BuildChooser()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindFirstObjectByType<Canvas>();

        RectTransform win;
        chooserRoot = UIKit.Popup(canvas.rootCanvas.transform, "AvatarChooser", 860f, 1300f, out win);
        UIKit.Title(win, "Choose Avatar", 100, 70, 660, 140, 80);
        UIKit.CloseButton(win, 735, 40, 96, HideChooser);

        rings = new Image[AvatarArt.Count];
        for (int i = 0; i < AvatarArt.Count; i++)
        {
            int index = i;
            float x = 45f + (i % 4) * 200f;
            float y = 250f + (i / 4) * 194f;

            UIKit.Round(win, "TileShadow", x, y + 6, 170, 170, 85, new Color(0f, 0f, 0f, 0.3f));
            rings[i] = UIKit.Round(win, "TileRing", x, y, 170, 170, 85, UIKit.Gold);
            UIKit.Round(win, "TileBack", x + 7, y + 7, 156, 156, 78, UIKit.Navy);

            RectTransform pic = UIKit.Box(win, "Avatar", x + 14, y + 14, 142, 142);
            Image im = pic.gameObject.AddComponent<Image>();
            im.sprite = AvatarArt.Get(i);
            im.preserveAspect = true;
            im.raycastTarget = false;

            UIKit.Hit(win, x, y, 170, 170, () => Choose(index));
        }
        chooserRoot.SetActive(false);
    }

    void RefreshRings()
    {
        int selected = PlayerData.AvatarIndex;
        for (int i = 0; i < rings.Length; i++)
            rings[i].color = (i == selected) ? UIKit.Green : UIKit.Gold;   // বাছা ছবির পাড় সবুজ
    }

    void Choose(int index)
    {
        PlayerData.AvatarIndex = index;
        ShowAvatar();
        HideChooser();
        AudioManager.PlayClick();
    }
}