using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all the configuration data for a hidden object level, 
/// including general settings and a list of hidden objects.
/// </summary>
[CreateAssetMenu(fileName = "NewLevelConfig", menuName = "Hidden Object Game/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("General Settings")]

    /// <summary>
    /// Unique identifier for the level, used internally to load and manage data.
    /// </summary>
    [Tooltip("Unique name or ID for this level (can be used internally).")]
    public string levelId = "Level_01";

    /// <summary>
    /// The display name for this level, shown in UI and menus.
    /// </summary>
    [Tooltip("Display name shown in the UI.")]
    public string levelName = "Mystery Room";

    /// <summary>
    /// The total time (in seconds) allotted to complete this level.
    /// </summary>
    [Tooltip("Time limit for this level (in seconds).")]
    [Range(10, 600)]
    public float timerDuration = 60f;

    [Header("Hidden Object Settings")]

    /// <summary>
    /// List of hidden object data that defines which objects will appear in this level.
    /// </summary>
    [Tooltip("List of hidden objects that will be placed in this level.")]
    public List<HiddenObjectData> hiddenObjects = new List<HiddenObjectData>();
}
