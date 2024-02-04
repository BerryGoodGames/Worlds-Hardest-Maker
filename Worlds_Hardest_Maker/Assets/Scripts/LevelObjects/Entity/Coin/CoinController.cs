using MyBox;
using UnityEngine;

public class CoinController : EntityController, IResettable, ICollectible
{
    [InitializationField] [MustBeAssigned] public Animator Animator;

    [HideInInspector] public Vector2 CoinPosition;

    [HideInInspector] public bool PickedUp;

    private static readonly int playingString = Animator.StringToHash("Playing");
    private static readonly int pickedUpString = Animator.StringToHash("PickedUp");


    public override EditMode EditMode => EditModeManager.Coin;

    private void Awake()
    {
        CoinPosition = transform.position;

        CoinManager.Instance.Coins.Add(this);
    }

    private void Start()
    {
        ((IResettable)this).Subscribe();
        PlayManager.Instance.OnSwitchToPlay += ActivateAnimation;
    }
    
    private void OnDestroy()
    {
        // un-cache coin
        CoinManager.Instance.Coins.Remove(this);

        ((IResettable)this).Unsubscribe();
        PlayManager.Instance.OnSwitchToPlay -= ActivateAnimation;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (PickedUp) return;

        // check if edgeCollider is player
        if (!collision.TryGetComponent(out PlayerController controller)) return;

        // check if that player hasn't picked coin up yet
        if (CoinManager.Instance.CollectedCoins.Contains(this)) return;

        Collect();

        // check if player is in goal while collecting coin
        if (!CoinManager.Instance.AllCoinsCollected()) return;

        foreach (FieldController field in controller.CurrentFields)
        {
            FieldMode fieldMode = field.FieldMode;
            if (fieldMode != EditModeManager.Goal) continue;

            controller.Win();
            break;
        }
    }

    public void Collect()
    {
        CoinManager.Instance.CollectedCoins.Add(this);

        // coin counter, sfx, animation
        AudioManager.Instance.Play("PlaceCoin");

        Animator.SetBool(pickedUpString, true);
        PickedUp = true;
    }

    public void ResetState()
    {
        PickedUp = false;

        Animator.SetBool(playingString, false);
        Animator.SetBool(pickedUpString, false);
    }

    public void ActivateAnimation()
    {
        Animator.SetBool(playingString, true);
        Animator.SetBool(pickedUpString, PickedUp);
    }

    public override Data GetData() => new CoinData(this);
}