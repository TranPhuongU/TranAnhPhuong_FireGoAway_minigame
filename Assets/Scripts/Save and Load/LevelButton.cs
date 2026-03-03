using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class LevelButton : MonoBehaviour
{
    [HideInInspector] public int levelIndex;

    public Button button;
    public TMP_Text levelText;

    [Header("State Image")]
    public Image stateImage;

    [Header("Sprites")]
    public Sprite lockedSprite;
    public Sprite easySprite;
    public Sprite hardSprite;

    void Awake()
    {
        levelIndex = ExtractIndexFromName(gameObject.name);

        if (button == null)
            button = GetComponent<Button>();

        button.onClick.RemoveListener(OnClickLevel);
        button.onClick.AddListener(OnClickLevel);
    }

    void Start()
    {
        levelText.text = (levelIndex + 1).ToString();

        bool unlocked = SaveAndLoad.IsLevelUnlocked(levelIndex);

        UpdateVisual(unlocked);
    }

    int ExtractIndexFromName(string objName)
    {
        Match match = Regex.Match(objName, @"(\d+)$");

        if (match.Success)
        {
            int parsed = int.Parse(match.Value);
            return Mathf.Max(0, parsed - 1);
        }

        Debug.LogWarning($"Cannot parse level index from {objName}");
        return 0;
    }
    void UpdateVisual(bool unlocked)
    {
        if (stateImage == null) return;

        if (!unlocked)
        {
            stateImage.sprite = lockedSprite;

            if (levelText != null)
                levelText.gameObject.SetActive(false);
        }
        else
        {
            if (levelText != null)
                levelText.gameObject.SetActive(true);

            if (levelIndex <= 2) 
                stateImage.sprite = easySprite;
            else
                stateImage.sprite = hardSprite;
        }
    }
    void OnClickLevel()
    {
        if (!SaveAndLoad.IsLevelUnlocked(levelIndex))
            return;

        LevelSelection.SelectedLevelIndex = levelIndex;
        SceneManager.LoadScene("Gameplay");
    }
}

public static class LevelSelection
{
    public static int SelectedLevelIndex = 0;
}