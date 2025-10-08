using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles all UI interactions and visual feedback in the game, 
/// including timer updates, game start/restart transitions, and win/lose states.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Primary Buttons")]
    [SerializeField, Tooltip("Button used to start the game for the first time.")]
    private Button startButton;

    [SerializeField, Tooltip("Button used to restart the game while playing.")]
    private Button restartButton;

    [SerializeField, Tooltip("Button used to replay the game after completion.")]
    private Button replayButton;

    [Header("UI Elements")]
    [SerializeField, Tooltip("Text element displaying remaining time in the game.")]
    private TextMeshProUGUI timerText;

    [SerializeField, Tooltip("Main overlay panel displayed during gameplay.")]
    private GameObject gameOverlayPanel;

    [SerializeField, Tooltip("UI list displaying target objects to find.")]
    private GameObject targetObjectList;

    [SerializeField, Tooltip("Text element showing how many objects have been found.")]
    private TextMeshProUGUI objectFoundText;

    [Header("Game End States")]
    [SerializeField, Tooltip("UI element displayed when the player wins.")]
    private GameObject winObject;

    [SerializeField, Tooltip("UI element displayed when the player loses.")]
    private GameObject loseObject;

    [Header("UI Animation Elements")]
    [SerializeField, Tooltip("Panel used for reset transitions")]
    private Transform resetPanel;

    [Header("References")]
    private ObjectManager objectManager;

    #region Unity Lifecycle

    private void Awake()
    {
        objectManager = FindAnyObjectByType<ObjectManager>();
    }

    private void OnEnable()
    {
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(StartGame);
        restartButton.onClick.RemoveListener(RestartGame);
    }

    private void Start()
    {
        GameManager.Instance.OnTimeElapsed += GameManager_OnTimerTick;
        GameManager.Instance.OnGameComplete += GameManager_OnGameComplete;
        if (objectManager != null)
        {
            objectManager.OnObjectFound += ObjectManager_OnObjectFound; 
        }

        startButton.gameObject.SetActive(true);
        gameOverlayPanel.SetActive(true);
        resetPanel.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnTimeElapsed -= GameManager_OnTimerTick;
        GameManager.Instance.OnGameComplete -= GameManager_OnGameComplete;

        if (objectManager != null)
        {
            objectManager.OnObjectFound -= ObjectManager_OnObjectFound;
        }
    }

    #endregion

    #region UI Button Methods

    /// <summary>
    /// Called when the Start button is pressed.
    /// Initializes the game and hides setup UI.
    /// </summary>
    private void StartGame()
    {
        startButton.gameObject.SetActive(false);
        gameOverlayPanel.SetActive(false);

        GameManager.Instance.StartGame();
        objectFoundText.text = $"Objects Found\n {0}/{objectManager.TotalObjectCount}";

        // Animate panel scaling out
        resetPanel.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
    }

    /// <summary>
    /// Called when the Restart or Replay button is pressed.
    /// Resets game state, clears UI, and restarts the level.
    /// </summary>
    private async void RestartGame()
    {
        // Animate the panel in before resetting
        await resetPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).AsyncWaitForCompletion();

        // Reset visible elements
        gameOverlayPanel.SetActive(false);
        winObject.SetActive(false);
        loseObject.SetActive(false);
        replayButton.gameObject.SetActive(false);

        // Restart game logic
        GameManager.Instance.RestartGame();
        objectFoundText.text = $"Objects Found\n {0}/{objectManager.TotalObjectCount}";

        // Animate panel scaling out
        resetPanel.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Updates the timer text every second with remaining time.
    /// </summary>
    private void GameManager_OnTimerTick(object sender, int time)
    {
        timerText.text = $"Time Remaining: {time}s";
    }

    /// <summary>
    /// Updates the UI when a new object is found.
    /// </summary>
    private void ObjectManager_OnObjectFound(object sender, ObjectManager.ObjectFoundEventArgs objectFoundData)
    {
        objectFoundText.text = $"Objects Found\n {objectFoundData.objectFoundCount}/{objectFoundData.totalObjects}";
    }

    /// <summary>
    /// Handles the UI transition when the game ends, showing either win or lose state.
    /// </summary>
    private void GameManager_OnGameComplete(object sender, bool didWin)
    {
        gameOverlayPanel.SetActive(true);

        // Display appropriate end screen
        winObject.SetActive(didWin);
        loseObject.SetActive(!didWin);

        startButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(true);

        // Ensure replay button resets correctly each time
        replayButton.onClick.AddListener(() =>
        {
            RestartGame();
            replayButton.onClick.RemoveAllListeners();
        });
    }

    #endregion
}
