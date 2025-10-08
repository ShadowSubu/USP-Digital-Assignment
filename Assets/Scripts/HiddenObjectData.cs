using UnityEngine;

/// <summary>
/// Stores data for an individual hidden object, including its unique identifier,
/// display name, and sprite representation. 
/// Used by the <see cref="HiddenObject"/> and <see cref="LevelConfig"/> systems.
/// </summary>
[CreateAssetMenu(fileName = "NewHiddenObjectData", menuName = "Hidden Object Game/Hidden Object Data")]
public class HiddenObjectData : ScriptableObject
{
    /// <summary>
    /// Unique identifier for this hidden object, used internally for tracking and saving progress.
    /// </summary>
    [Tooltip("Unique identifier for this object (used internally by code).")]
    public int objectId;

    /// <summary>
    /// The display name shown in UI elements such as the target object list.
    /// </summary>
    [Tooltip("Display name for this object, shown in UI target lists.")]
    public string displayName;

    /// <summary>
    /// The sprite image used to represent this object in the game.
    /// </summary>
    [Tooltip("Sprite asset representing this object visually.")]
    public Sprite sprite;
}
