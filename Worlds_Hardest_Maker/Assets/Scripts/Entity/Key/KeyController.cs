using UnityEngine;
using UnityEngine.Serialization;

public class KeyController : Controller
{
    [FormerlySerializedAs("color")] [HideInInspector]
    public KeyManager.KeyColor Color;

    [FormerlySerializedAs("keyPosition")] [HideInInspector]
    public Vector2 KeyPosition;

    [FormerlySerializedAs("pickedUp")] [HideInInspector]
    public bool PickedUp;

    private static readonly int pickedUpString = Animator.StringToHash("PickedUp");

    private void Awake()
    {
        KeyPosition = transform.position;

        SetOrderInLayer();
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if edgeCollider is player
        if (!collision.TryGetComponent(out PlayerController controller)) return;

        // check if player is of own client
        if (MultiplayerManager.Instance.Multiplayer && !controller.PhotonView.IsMine) return;

        // check if that player hasn't collected key yet
        if (!controller.KeysCollected.Contains(gameObject)) PickUp(collision.gameObject);
    }

    private void SetOrderInLayer()
    {
        int highestOrder = 0;
        foreach (Transform key in ReferenceManager.Instance.KeyContainer)
        {
            int order = key.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder;
            if (order > highestOrder) highestOrder = order;
        }

        GetComponent<Renderer>().sortingOrder = highestOrder + 1;
    }

    private void PickUp(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.KeysCollected.Add(gameObject);

        // random rotation of pickup animation
        int randRotation = Random.Range(0, 2) * 90;
        transform.Rotate(0, 0, randRotation);

        // pickup animation and sound
        Animator anim = transform.parent.GetComponent<Animator>();
        anim.SetBool(pickedUpString, true);
        AudioManager.Instance.Play("Key");

        PickedUp = true;

        CheckAndUnlock(player);
    }

    public void CheckAndUnlock(GameObject player)
    {
        if (!player.GetComponent<PlayerController>().AllKeysCollected(Color)) return;

        string tagColor = Color switch
        {
            KeyManager.KeyColor.RED => "Red",
            KeyManager.KeyColor.GREEN => "Green",
            KeyManager.KeyColor.BLUE => "Blue",
            KeyManager.KeyColor.YELLOW => "Yellow",
            _ => ""
        };

        foreach (GameObject door in GameObject.FindGameObjectsWithTag(tagColor + "KeyDoorField"))
        {
            KeyDoorField controller = door.GetComponent<KeyDoorField>();
            controller.Lock(false);
        }
    }

    public override Data GetData() => new KeyData(this);
}