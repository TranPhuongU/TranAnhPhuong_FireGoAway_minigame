using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    [SerializeField] GameObject settingPanel;

    [Header("Moves")]
    [SerializeField] TMP_Text moveText;

    [Header("Level Name")]
    [SerializeField] TMP_Text levelNameText;
    [SerializeField] TMP_Text levelNameTextWin;
    [SerializeField] TMP_Text levelNameTextLose;

    [Header("Stars")]
    [SerializeField] TMP_Text starTotalText;      
    [SerializeField] TMP_Text starTotalTextWin;   
    [SerializeField] TMP_Text starTotalTextLose;  
    [SerializeField] TMP_Text starWinText;       
    [SerializeField] TMP_Text starLoseText;       

    [SerializeField] private GameObject board;

    [SerializeField] private float settingAnimDuration = 0.25f;

    Tween settingTween;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshMoveText();
        RefreshLevelNames();
        RefreshStarText();

        winPanel.SetActive(false);
        losePanel.SetActive(false);
        if (settingPanel != null)
            settingPanel.SetActive(false);

        GameManager.instance.onGameStateCallBack += OnGameStateChanged;
    }

    public void RefreshMoveText()
    {
        if (moveText != null)
            moveText.text = $"Rotations: {GameManager.instance.currentMoves}";
    }

    public void RefreshLevelNames()
    {
        int levelNumber = LevelSelection.SelectedLevelIndex + 1;

        SetLevelText(levelNameText, $"Level {levelNumber}");
        SetLevelText(levelNameTextWin, levelNumber.ToString());
        SetLevelText(levelNameTextLose, levelNumber.ToString());
    }

    private void SetLevelText(TMP_Text textComp, string value)
    {
        if (textComp != null)
            textComp.text = value;
    }
    public void RefreshStarText()
    {
        int current = GameManager.instance.currentStars;
        int max = GameManager.instance.GetMaxStars();
        string value = $"{current}/{max}";

        SetStarText(starTotalText, value);
        SetStarText(starTotalTextWin, value);
        SetStarText(starTotalTextLose, value);
    }

    private void SetStarText(TMP_Text textComp, string value)
    {
        if (textComp != null)
            textComp.text = value;
    }
    void OnGameStateChanged(GameState state)
    {
        if (board != null)
            board.SetActive(false);

        bool isWin = state == GameState.Win;

        winPanel.SetActive(isWin);
        losePanel.SetActive(state == GameState.Lose);

        if (starWinText != null)
            starWinText.text = isWin ? "+5" : "";

        if (starLoseText != null)
            starLoseText.text = state == GameState.Lose ? "+0" : "";

        RefreshStarText();
    }
    public void NextClicked()
    {
        if (GameManager.instance.GetGameState() == GameState.Win)
            LevelSelection.SelectedLevelIndex++;

        SceneManager.LoadScene("Gameplay");
    }

    public void HomeClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }



    public void OpenSettings()
    {
        if (settingPanel == null) return;
        if (settingPanel.activeSelf) return;

        board.SetActive(false);
        settingPanel.SetActive(true);

        Transform t = settingPanel.transform;

        settingTween?.Kill();

        t.localScale = Vector3.zero;

        settingTween = t.DOScale(Vector3.one, settingAnimDuration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void CloseSettings()
    {
        if (settingPanel == null) return;
        if (!settingPanel.activeSelf) return;

        Transform t = settingPanel.transform;

        settingTween?.Kill();

        settingTween = t.DOScale(Vector3.zero, settingAnimDuration)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                settingPanel.SetActive(false);
                board.SetActive(true);
            });
    }
}