using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the creation, management, and updating of hidden object UI icons.
/// Syncs the in-game hidden objects with their corresponding UI elements to reflect found states.
/// </summary>
public class HiddenObjectUITracker : MonoBehaviour
{
    [Header("UI Configuration")]
    [Tooltip("Prefab for individual hidden object UI elements.")]
    [SerializeField] private HiddenObjectUIItem uiItemPrefab;

    [Tooltip("Parent transform under which all UI items will be instantiated.")]
    [SerializeField] private Transform contentParent;

    /// <summary>
    /// Dictionary mapping object IDs to their corresponding UI items for quick lookup and updates.
    /// </summary>
    private readonly Dictionary<int, HiddenObjectUIItem> uiItems = new Dictionary<int, HiddenObjectUIItem>();

    /// <summary>
    /// Reference to the active <see cref="ObjectManager"/> in the scene.
    /// </summary>
    private ObjectManager objectManager;

    /// <summary>
    /// Called during the Awake phase. Finds the active <see cref="ObjectManager"/> instance.
    /// </summary>
    private void Awake()
    {
        objectManager = FindAnyObjectByType<ObjectManager>();
    }

    /// <summary>
    /// Subscribes to game events for updating and populating the UI.
    /// </summary>
    private void Start()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStart;
        objectManager.OnObjectFound += ObjectManager_OnObjectFound;
    }

    /// <summary>
    /// Unsubscribes from all event handlers to prevent memory leaks.
    /// </summary>
    private void OnDestroy()
    {
        GameManager.Instance.OnGameStarted -= GameManager_OnGameStart;
        objectManager.OnObjectFound -= ObjectManager_OnObjectFound;
    }

    /// <summary>
    /// Called when a new game session starts. 
    /// Sets up the UI items corresponding to all hidden objects for that level.
    /// </summary>
    /// <param name="sender">The event sender (GameManager).</param>
    /// <param name="levelConfig">The level configuration used to initialize the UI.</param>
    private void GameManager_OnGameStart(object sender, LevelConfig levelConfig)
    {
        List<HiddenObjectData> hiddenObjectsData = new List<HiddenObjectData>();

        // Include both fixed and randomized objects
        foreach (var item in objectManager.FixedHiddenObjects)
        {
            hiddenObjectsData.Add(item.ObjectData);
        }

        hiddenObjectsData.AddRange(levelConfig.hiddenObjects);
        SetupUIItems(hiddenObjectsData);
    }

    /// <summary>
    /// Populates the UI panel with icons for all hidden objects in the current level.
    /// </summary>
    /// <param name="hiddenObjectsData">List of hidden object data used to generate the UI.</param>
    private void SetupUIItems(List<HiddenObjectData> hiddenObjectsData)
    {
        ClearUIItems();

        foreach (var obj in hiddenObjectsData)
        {
            HiddenObjectUIItem uiItem = Instantiate(uiItemPrefab, contentParent);
            uiItem.Initialize(obj.sprite, obj.objectId);
            uiItems.Add(obj.objectId, uiItem);
        }
    }

    /// <summary>
    /// Updates the UI when an object is found in the game.
    /// </summary>
    /// <param name="sender">The event sender (ObjectManager).</param>
    /// <param name="args">Information about the found object.</param>
    private void ObjectManager_OnObjectFound(object sender, ObjectManager.ObjectFoundEventArgs args)
    {
        if (uiItems.ContainsKey(args.foundObjectId))
        {
            uiItems[args.foundObjectId].SetFoundState(true);
        }
    }

    /// <summary>
    /// Destroys all instantiated UI items and clears the tracking dictionary.
    /// </summary>
    private void ClearUIItems()
    {
        foreach (var item in uiItems.Values)
        {
            Destroy(item.gameObject);
        }

        uiItems.Clear();
    }

    /// <summary>
    /// Resets the visual state of all UI items to the "not found" state.
    /// </summary>
    public void ResetAllItems()
    {
        foreach (var item in uiItems.Values)
        {
            item.SetFoundState(false);
        }
    }
}
