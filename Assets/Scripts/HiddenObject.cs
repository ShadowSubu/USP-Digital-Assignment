using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents a single hidden object in the scene that the player can find and interact with.
/// Handles initialization, interaction animations, and communication with the <see cref="ObjectManager"/>.
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class HiddenObject : MonoBehaviour
{
    [Header("Object Data")]
    [SerializeField, Tooltip("ScriptableObject containing information about this hidden object.")]
    private HiddenObjectData objectData;

    [SerializeField, Tooltip("Indicates whether this object has been found by the player.")]
    public bool isFound = false;

    [Header("Events")]
    [Tooltip("Triggered when the object is found and interacted with by the player.")]
    public UnityEvent OnFound;

    [Header("Runtime References")]
    private SpriteRenderer spriteRenderer;
    private ObjectManager objectManager;

    #region Unity Lifecycle

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (isFound) return;
        OnInteract();
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the hidden object with its data and reference to the <see cref="ObjectManager"/>.
    /// </summary>
    /// <param name="objectManager">The manager tracking all hidden objects.</param>
    /// <param name="objectData">The data asset defining this hidden object.</param>
    public void Initialize(ObjectManager objectManager, HiddenObjectData objectData)
    {
        this.objectData = objectData;
        this.objectManager = objectManager;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = objectData.sprite;
    }

    #endregion

    #region Interaction Logic

    /// <summary>
    /// Handles the interaction sequence when the player clicks on this object.
    /// Plays an animation, updates the manager, and disables the object afterward.
    /// </summary>
    public async void OnInteract()
    {
        isFound = true;
        OnFound?.Invoke();

        objectManager.ObjectFound(objectData.objectId);
        await transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InOutBack).AsyncWaitForCompletion();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Resets the object to its default state, making it visible and interactable again.
    /// Used when restarting or replaying the level.
    /// </summary>
    public void ResetObject()
    {
        isFound = false;
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the data asset associated with this hidden object.
    /// </summary>
    public HiddenObjectData ObjectData => objectData;

    #endregion
}
