using UnityEngine;
using UnityEngine.Serialization;

public class CoinController : Controller
{
    [FormerlySerializedAs("coinPosition")] [HideInInspector] public Vector2 CoinPosition;
    [FormerlySerializedAs("pickedUp")] [HideInInspector] public bool PickedUp;

    private static readonly int pickedUpString = Animator.StringToHash("PickedUp");

    private void Awake()
    {
        CoinPosition = transform.position;
    }

    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PickedUp) return;

        // check if edgeCollider is player
        if (!collision.TryGetComponent(out PlayerController controller)) return;

        // check if player is of own client
        if (MultiplayerManager.Instance.Multiplayer && !controller.PhotonView.IsMine) return;

        // check if that player hasn't picked coin up yet
        if (controller.CoinsCollected.Contains(gameObject)) return;

        PickUp(collision.gameObject);

        // check if player is in goal while collecting coin
        if (!controller.AllCoinsCollected()) return;

        foreach (GameObject field in controller.CurrentFields)
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
        controller.CoinsCollected.Add(gameObject);

        // coin counter, sfx, animation
        AudioManager.Instance.Play("Coin");

        Animator anim = transform.parent.GetComponent<Animator>();
        anim.SetBool(pickedUpString, true);
        PickedUp = true;
    }

    public override Data GetData()
    {
        return new CoinData(this);
    }
}