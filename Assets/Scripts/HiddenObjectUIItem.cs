using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Manages the visual representation of a hidden object icon in the UI target list.
/// Handles its initialization, found-state animation, and data linkage to the in-game object.
/// </summary>
[RequireComponent(typeof(Image))]
public class HiddenObjectUIItem : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("The Image component displaying this hidden object's icon.")]
    [SerializeField] private Image objectImage;

    /// <summary>
    /// The unique identifier associated with the hidden object this UI item represents.
    /// </summary>
    private int objectId;

    /// <summary>
    /// Initializes this UI item with the provided sprite and identifier.
    /// </summary>
    /// <param name="sprite">The sprite used to visually represent the object.</param>
    /// <param name="id">The unique object ID this UI item corresponds to.</param>
    public void Initialize(Sprite sprite, int id)
    {
        objectId = id;
        objectImage.sprite = sprite;
        objectImage.preserveAspect = true;
        SetFoundState(false);
    }

    /// <summary>
    /// Updates the UI appearance based on whether the object has been found or not.
    /// </summary>
    /// <param name="found">True if the object is found, false otherwise.</param>
    public void SetFoundState(bool found)
    {
        if (found)
        {
            objectImage.DOColor(Color.white, 0.5f);
        }
        else
        {
            objectImage.DOColor(new Color(0f, 0f, 0f, 0.2f), 0.5f);
        }
    }

    /// <summary>
    /// Gets the unique identifier of the hidden object associated with this UI element.
    /// </summary>
    public int ObjectId => objectId;
}
