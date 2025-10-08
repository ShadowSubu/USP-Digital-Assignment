using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all hidden objects in the scene, including initialization, placement, and progress tracking. Responsible for spawning, resetting, and communicating
/// game state changes back to the <see cref="GameManager"/> and <see cref="UIManager"/>.
/// </summary>
public class ObjectManager : MonoBehaviour
{
    [Header("Position Settings")]
    [SerializeField, Tooltip("Parent GameObject containing predefined positions where randomized hidden objects can appear.")]
    private GameObject hiddenObjectsPositionList;

    [Tooltip("List of all possible spawn positions for hidden objects.")]
    private List<Vector3> possiblePositions = new List<Vector3>();

    [Header("Parent References")]
    [SerializeField, Tooltip("Parent transform containing fixed hidden objects in the scene.")]
    private Transform fixedObjectsParent;

    [SerializeField, Tooltip("Parent transform where randomized hidden objects will be instantiated.")]
    private Transform randomizedObjectsParent;

    [Header("Prefabs & References")]
    [SerializeField, Tooltip("Prefab used to instantiate new randomized hidden objects.")]
    private HiddenObject hiddenObjectPrefab;

    #region Events
    public EventHandler<ObjectFoundEventArgs> OnObjectFound;
    public EventHandler OnAllObjectsFound;
    public class ObjectFoundEventArgs : EventArgs
    {
        public int objectFoundCount;
        public int totalObjects;
        public int foundObjectId;
    }
    #endregion

    #region Private Fields

    private List<HiddenObject> fixedHiddenObjects = new List<HiddenObject>();
    private List<HiddenObject> randomizedHiddenObjects = new List<HiddenObject>();
    private HashSet<int> foundObjects = new HashSet<int>();

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Cache possible spawn positions from the provided list
        for (int i = 0; i < hiddenObjectsPositionList.transform.childCount; i++)
        {
            possiblePositions.Add(hiddenObjectsPositionList.transform.GetChild(i).position);
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStart;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStarted -= GameManager_OnGameStart;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handles the setup process when the game starts, triggered by the <see cref="GameManager"/>.
    /// </summary>
    private void GameManager_OnGameStart(object sender, LevelConfig levelConfig)
    {
        SetupHiddenObjects(levelConfig);
    }

    #endregion

    #region Setup & Initialization

    /// <summary>
    /// Clears existing data and initializes hidden objects for the current level.
    /// </summary>
    private void SetupHiddenObjects(LevelConfig levelConfig)
    {
        foundObjects.Clear();
        PopulateObjectPool(levelConfig);
        PopulateHiddenObjects();
    }

    /// <summary>
    /// Populates object pools by preparing fixed and randomized hidden objects.
    /// </summary>
    private void PopulateObjectPool(LevelConfig levelConfig)
    {
        // Setup fixed hidden objects if not already initialized
        if (fixedHiddenObjects != null && fixedHiddenObjects.Count > 0) return;

        for (int i = 0; i < fixedObjectsParent.childCount; i++)
        {
            fixedObjectsParent.GetChild(i).TryGetComponent(out HiddenObject obj);
            if (obj != null)
            {
                obj.Initialize(this, obj.ObjectData);
                obj.gameObject.SetActive(false);
                fixedHiddenObjects.Add(obj);
            }
        }

        // Setup randomized hidden objects if not already initialized
        if (randomizedHiddenObjects != null && randomizedHiddenObjects.Count > 0) return;

        for (int i = 0; i < levelConfig.hiddenObjects.Count; i++)
        {
            HiddenObject obj = Instantiate(hiddenObjectPrefab, randomizedObjectsParent);
            obj.Initialize(this, levelConfig.hiddenObjects[i]);
            obj.gameObject.SetActive(false);
            randomizedHiddenObjects.Add(obj);
        }
    }

    /// <summary>
    /// Randomly positions and activates hidden objects in the scene.
    /// </summary>
    private void PopulateHiddenObjects()
    {
        if (possiblePositions.Count == 0 || randomizedHiddenObjects.Count == 0)
        {
            Debug.LogWarning("ObjectManager: No available positions or hidden objects to populate.");
            return;
        }

        // Shuffle spawn positions to randomize placement
        List<Vector3> shuffledPositions = new List<Vector3>(possiblePositions);
        for (int i = 0; i < shuffledPositions.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, shuffledPositions.Count);
            (shuffledPositions[i], shuffledPositions[randomIndex]) = (shuffledPositions[randomIndex], shuffledPositions[i]);
        }

        // Reactivate and reset all fixed objects
        foreach (var obj in fixedHiddenObjects)
        {
            obj.ResetObject();
            obj.gameObject.SetActive(true);
        }

        // Limit spawned randomized objects to available positions
        int spawnCount = Mathf.Min(randomizedHiddenObjects.Count, shuffledPositions.Count);

        for (int i = 0; i < randomizedHiddenObjects.Count; i++)
        {
            HiddenObject obj = randomizedHiddenObjects[i];

            if (i < spawnCount)
            {
                obj.transform.position = shuffledPositions[i];
                float randomRotation = UnityEngine.Random.Range(0f, 360f);
                obj.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

                obj.ResetObject();
                obj.gameObject.SetActive(true);
            }
            else
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Runtime Methods

    /// <summary>
    /// Called when a hidden object is found by the player. Tracks progress and triggers completion events when all are found.
    /// </summary>
    /// <param name="objectId">Unique ID of the object found.</param>
    public void ObjectFound(int objectId)
    {
        if (foundObjects.Contains(objectId)) return;

        foundObjects.Add(objectId);

        OnObjectFound?.Invoke(this, new ObjectFoundEventArgs
        {
            objectFoundCount = foundObjects.Count,
            totalObjects = fixedHiddenObjects.Count + randomizedHiddenObjects.Count,
            foundObjectId = objectId
        });

        // If all objects found, trigger completion event
        if (foundObjects.Count >= fixedHiddenObjects.Count + randomizedHiddenObjects.Count)
        {
            OnAllObjectsFound?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the number of objects that have been found.
    /// </summary>
    public int FoundObjectCount => foundObjects.Count;

    /// <summary>
    /// Gets the total number of hidden objects in the scene.
    /// </summary>
    public int TotalObjectCount => fixedHiddenObjects.Count + randomizedHiddenObjects.Count;

    /// <summary>
    /// Returns the list of fixed hidden objects.
    /// </summary>
    public List<HiddenObject> FixedHiddenObjects => fixedHiddenObjects;

    #endregion
}
