using System;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Controls the overall game flow, including starting, restarting, 
/// tracking time, and detecting game completion or failure.
/// Implements a Singleton pattern for global access.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the GameManager.
    /// </summary>
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField, Tooltip("Current state of the game (Idle, Playing, Complete, Failed).")]
    private GameState currentState = GameState.Idle;

    [Header("Level Configuration")]
    [SerializeField, Tooltip("Configuration for the level to start with, including timer duration and object references.")]
    private LevelConfig startingLevel;

    [Header("Runtime Variables")]
    private float gameStartTime;
    private float elapsedTime;
    private ObjectManager objectManager;

    #region Events

    /// <summary>
    /// Invoked when the game starts, passing the current <see cref="LevelConfig"/>.
    /// </summary>
    public event EventHandler<LevelConfig> OnGameStarted;

    /// <summary>
    /// Invoked when the game is completed or failed.
    /// The boolean parameter indicates success (true) or failure (false).
    /// </summary>
    public event EventHandler<bool> OnGameComplete;

    /// <summary>
    /// Invoked every second to notify listeners of the remaining time.
    /// </summary>
    public event EventHandler<int> OnTimeElapsed;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        objectManager = FindAnyObjectByType<ObjectManager>();
    }

    private void Start()
    {
        objectManager.OnAllObjectsFound += HandleGameComplete;

        gameStartTime = Time.time;
        elapsedTime = 0f;
        currentState = GameState.Playing;
    }

    private void OnDestroy()
    {
        objectManager.OnAllObjectsFound -= HandleGameComplete;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Starts the game and begins tracking time.
    /// </summary>
    public void StartGame()
    {
        currentState = GameState.Playing;
        gameStartTime = Time.time;
        elapsedTime = 0f;

        OnGameStarted?.Invoke(this, startingLevel);

        CancelInvoke(nameof(Tick));
        InvokeRepeating(nameof(Tick), 0f, 1f);

        Debug.Log("GameManager: Game Started");
    }

    /// <summary>
    /// Restarts the game from the beginning of the current level.
    /// </summary>
    public void RestartGame()
    {
        currentState = GameState.Playing;
        CancelInvoke(nameof(Tick));
        gameStartTime = Time.time;
        elapsedTime = 0f;

        StartGame();

        Debug.Log("GameManager: Game Restarted");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles game completion logic when all hidden objects are found.
    /// </summary>
    private async void HandleGameComplete(object sender, EventArgs e)
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.Complete;
        CancelInvoke(nameof(Tick));

        await Task.Delay(500); // Slight delay for end sequence

        OnGameComplete?.Invoke(this, true);
        Debug.Log("GameManager: Game Complete!");
    }

    /// <summary>
    /// Handles game failure logic when time runs out.
    /// </summary>
    private void HandleGameFailed()
    {
        if (currentState != GameState.Playing) return;

        currentState = GameState.Failed;
        OnGameComplete?.Invoke(this, false);
        Debug.Log("GameManager: Game Failed - Time's Up!");
    }

    /// <summary>
    /// Called every second while the game is active.
    /// Updates elapsed time and triggers failure when time expires.
    /// </summary>
    private async void Tick()
    {
        if (currentState != GameState.Playing)
            return;

        elapsedTime = Time.time - gameStartTime;
        int remainingTime = Mathf.FloorToInt(startingLevel.timerDuration - elapsedTime);

        OnTimeElapsed?.Invoke(this, remainingTime);

        if (remainingTime <= 0)
        {
            OnTimeElapsed?.Invoke(this, 0);
            await Task.Delay(500);
            HandleGameFailed();
        }
    }

    #endregion
}

/// <summary>
/// Represents the current state of the game.
/// </summary>
public enum GameState
{
    Idle,
    Playing,
    Complete,
    Failed
}
