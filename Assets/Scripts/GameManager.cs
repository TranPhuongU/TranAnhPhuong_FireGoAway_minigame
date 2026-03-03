using System;
using UnityEngine;

public enum GameState
{
    Gameplay,
    Win,
    Lose
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentMoves { get; private set; }
    int startMoves = 25;

    // ⭐ STAR SYSTEM
    public int currentStars { get; private set; }
    const int STAR_WIN_REWARD = 5;
    const string STAR_KEY = "PLAYER_STARS";
    const int MAX_STARS = 999; // bạn chỉnh nếu muốn

    private GameState gameState;
    public Action<GameState> onGameStateCallBack;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Application.targetFrameRate = 90;

        // ⭐ Load stars
        currentStars = PlayerPrefs.GetInt(STAR_KEY, 0);
    }

    private void Start()
    {
        currentMoves = startMoves;
        gameState = GameState.Gameplay;
    }

    public GameState GetGameState() => gameState;

    public void GameStateCallBack(GameState _gameState)
    {
        if (gameState == _gameState) 
            return;

        gameState = _gameState;
        if (gameState == GameState.Win)
        {
            HandleWin();
            SoundManager.Instance?.PlayWin();   
        }
        else if (gameState == GameState.Lose)
        {
            SoundManager.Instance?.PlayLose();  
        }
        onGameStateCallBack?.Invoke(gameState);
    }

    void HandleWin()
    {
        int currentLevel = LevelSelection.SelectedLevelIndex;
        int nextLevel = currentLevel + 1;
        SaveAndLoad.UnlockLevel(nextLevel);

        AddStars(STAR_WIN_REWARD);
    }

    void AddStars(int amount)
    {
        currentStars += amount;
        currentStars = Mathf.Clamp(currentStars, 0, MAX_STARS);

        PlayerPrefs.SetInt(STAR_KEY, currentStars);
        PlayerPrefs.Save();

        UIManager.Instance.RefreshStarText();
    }
    public void UseMove()
    {
        if (GetGameState() != GameState.Gameplay)
            return;

        currentMoves--;
        currentMoves = Mathf.Max(0, currentMoves);
        UIManager.Instance.RefreshMoveText();

        if (currentMoves <= 0)
            GameStateCallBack(GameState.Lose);
    }

    public void SetStartMoves(int moves)
    {
        startMoves = moves;
        currentMoves = startMoves;
        UIManager.Instance.RefreshMoveText();
    }
    public int GetMaxStars() => MAX_STARS;
}