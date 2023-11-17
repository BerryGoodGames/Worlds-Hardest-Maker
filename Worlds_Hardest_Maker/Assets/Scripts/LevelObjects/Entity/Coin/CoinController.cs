using MyBox;
using UnityEngine;

public class CoinController : EntityController
{
    [InitializationField] [MustBeAssigned] public Animator Animator;

    [HideInInspector] public Vector2 CoinPosition;

    [HideInInspector] public bool PickedUp;

    private static readonly int pickedUpString = Animator.StringToHash("PickedUp");


    public override EditMode EditMode => EditMode.Coin;

    private void Awake()
    {
        CoinPosition = transform.position;

        CoinManager.Instance.Coins.Add(this);
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
            if (fieldType != FieldType.GoalField) continue;

            controller.Win();
            break;
        }
    }

    private void OnDestroy() =>
        // un-cache coin
        CoinManager.Instance.Coins.Remove(this);

    private void PickUp(GameObject player)
    {
        PlayerController controller = player.GetComponent<PlayerController>();
        controller.CoinsCollected.Add(gameObject);

        // coin counter, sfx, animation
        AudioManager.Instance.Play("Coin");

        Animator.SetBool(pickedUpString, true);
        PickedUp = true;
    }
    public override Data GetData() => new CoinData(this);
}