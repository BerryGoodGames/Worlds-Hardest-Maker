using MyBox;
using UnityEngine;

public class KeyController : EntityController
{
    [ReadOnly] public KeyManager.KeyColor Color;
    [ReadOnly] public Vector2 KeyPosition;
    [ReadOnly] public bool PickedUp;

    [Separator] [InitializationField] [MustBeAssigned]
    public SpriteRenderer SpriteRenderer;

    [InitializationField] [MustBeAssigned] public Animator Animator;
    [InitializationField] [MustBeAssigned] public IntervalRandomAnimation KonamiAnimation;

    private static readonly int pickedUpString = Animator.StringToHash("PickedUp");

    private void Awake()
    {
        KeyPosition = transform.position;

        // cache key controller
        KeyManager.Instance.Keys.Add(this);

        SetOrderInLayer();
    }

    private void OnDestroy()
    {
        KeyManager.Instance.Keys.Remove(this);

        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check key collection
        // check if edgeCollider is player
        if (!collision.TryGetComponent(out PlayerController controller)) return;

        // check if player is of own client
        if (MultiplayerManager.Instance.Multiplayer && !controller.PhotonView.IsMine) return;

        // check if that player hasn't collected key yet
        PlayerController player = collision.GetComponent<PlayerController>();
        if (!controller.KeysCollected.Contains(this)) PickUp(player);
    }

    /// <summary>
    /// Set order in layer to be on top of every other
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

    private void PickUp(PlayerController player)
    {
        player.KeysCollected.Add(this);

        // random rotation of pickup animation
        int randRotation = Random.Range(0, 2) * 90;
        transform.Rotate(0, 0, randRotation);

        // pickup animation and sound
        Animator anim = transform.parent.GetComponent<Animator>();
        anim.SetBool(pickedUpString, true);
        AudioManager.Instance.Play("Key");

        PickedUp = true;

        UnlockKeyDoors(player);
    }

    public void UnlockKeyDoors(PlayerController player)
    {
        if (!player.AllKeysCollected(Color)) return;

        string tagColor = Color switch
        {
            KeyManager.KeyColor.Red => "Red",
            KeyManager.KeyColor.Green => "Green",
            KeyManager.KeyColor.Blue => "Blue",
            KeyManager.KeyColor.Yellow => "Yellow",
            _ => ""
        };

        foreach (GameObject door in GameObject.FindGameObjectsWithTag(tagColor + "KeyDoorField"))
        {
            KeyDoorFieldController controller = door.GetComponent<KeyDoorFieldController>();
            controller.SetLocked(false);
        }
    }

    public override Data GetData() => new KeyData(this);
}