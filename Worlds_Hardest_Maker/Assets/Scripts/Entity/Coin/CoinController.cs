using UnityEngine;

public class CoinController : Controller
{
    [HideInInspector] public Vector2 coinPosition;
    [HideInInspector] public bool pickedUp;

    private static readonly int pickedUpString = Animator.StringToHash("PickedUp");

    private void Awake()
    {
        coinPosition = transform.position;
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pickedUp) return;

        // check if edgeCollider is player
        if (!collision.TryGetComponent(out PlayerController controller)) return;

        // check if player is of own client
        if (MultiplayerManager.Instance.Multiplayer && !controller.photonView.IsMine) return;

        // check if that player hasn't picked coin up yet
        if (controller.coinsCollected.Contains(gameObject)) return;

        PickUp(collision.gameObject);

        // check if player is in goal while collecting coin
        if (!controller.CoinsCollected()) return;

        foreach (GameObject field in controller.currentFields)
        {
            FieldType fieldType = (FieldType)FieldManager.GetFieldType(field);
            if (fieldType != FieldType.GOAL_FIELD) continue;

            controller.Win();
            break;
        }
    }

    private void PickUp(GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.coinsCollected.Add(gameObject);

        // coin counter, sfx, animation
        AudioManager.Instance.Play("Coin");

        Animator anim = transform.parent.GetComponent<Animator>();
        anim.SetBool(pickedUpString, true);
        pickedUp = true;
    }

    public override Data GetData()
    {
        return new CoinData(this);
    }
}