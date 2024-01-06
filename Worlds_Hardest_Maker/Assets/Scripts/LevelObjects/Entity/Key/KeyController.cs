using MyBox;
using UnityEngine;

public class KeyController : EntityController, IResettable, ICollectible
{
    [ReadOnly] public KeyColor Color;
    [ReadOnly] public Vector2 KeyPosition;
    [ReadOnly] public bool Collected;

    [Separator] [InitializationField] [MustBeAssigned] public SpriteRenderer SpriteRenderer;

    [InitializationField] [MustBeAssigned] public Animator Animator;
    [InitializationField] [MustBeAssigned] public IntervalRandomAnimation KonamiAnimation;

    private static readonly int playingString = Animator.StringToHash("Playing");
    private static readonly int pickedUpString = Animator.StringToHash("PickedUp");

    public override EditMode EditMode =>
        Color switch
        {
            KeyColor.Gray => EditModeManager.GrayKey,
            KeyColor.Red => EditModeManager.RedKey,
            KeyColor.Green => EditModeManager.GreenKey,
            KeyColor.Blue => EditModeManager.BlueKey,
            KeyColor.Yellow => EditModeManager.YellowKey,
            _ => throw new("There is no edit mode assigned for color " + Color),
        };

    private void Awake()
    {
        KeyPosition = transform.position;

        // cache key controller
        KeyManager.Instance.Keys.Add(this);

        SetOrderInLayer();
    }

    private void Start()
    {
        ((IResettable)this).Subscribe();
        PlayManager.Instance.OnSwitchToPlay += ActivateAnimation;
    }

    private void OnDestroy() => KeyManager.Instance.Keys.Remove(this);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check key collection: collider is player, key is not yet collected
        if (collision.CompareTag("Player") && !Collected) Collect();
    }

    /// <summary>
    ///     Set order in layer to be on top of every other
    /// </summary>
    private void SetOrderInLayer()
    {
        int highestOrder = 0;
        foreach (KeyController key in KeyManager.Instance.Keys)
        {
            int order = key.SpriteRenderer.sortingOrder;
            if (order > highestOrder) highestOrder = order;
        }

        SpriteRenderer.sortingOrder = highestOrder + 1;
    }

    public void Collect()
    {
        KeyManager.Instance.CollectedKeys.Add(this);

        // pickup animation and sound
        Animator.SetBool(pickedUpString, true);
        AudioManager.Instance.Play("PlaceKey");

        Collected = true;

        UnlockKeyDoors();
    }

    public void UnlockKeyDoors()
    {
        if (!KeyManager.Instance.AllKeysCollected(Color)) return;

        string tagColor = Color switch
        {
            KeyColor.Red => "Red",
            KeyColor.Green => "Green",
            KeyColor.Blue => "Blue",
            KeyColor.Yellow => "Yellow",
            _ => "Gray",
        };

        foreach (GameObject door in GameObject.FindGameObjectsWithTag(tagColor + "KeyDoor"))
        {
            KeyDoorFieldController controller = door.GetComponent<KeyDoorFieldController>();
            controller.SetLocked(false);
        }
    }

    public void ResetState()
    {
        Collected = false;

        Animator.SetBool(playingString, false);
        Animator.SetBool(pickedUpString, false);
    }

    public void ActivateAnimation()
    {
        Animator.SetBool(playingString, true);
        Animator.SetBool(pickedUpString, Collected);
    }

    public override Data GetData() => new KeyData(this);
}