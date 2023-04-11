using DG.Tweening;
using UnityEngine;

public class MoveBlock : AnchorBlock
{
    public const Type BlockType = Type.MOVE_TO;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.MoveToBlockPrefab;

    private readonly Vector2 target;

    #region Constructors

    public MoveBlock(AnchorController anchor, Vector2 target) : base(anchor) => this.target = target;

    public MoveBlock(AnchorController anchor, float x, float y) : this(anchor,
        new(x, y))
    {
    }

    #endregion

    public override void Execute()
    {
        float duration;
        float dist = Vector2.Distance(target, Anchor.Rb.position);

        if (Anchor.ApplySpeed)
        {
            float speed = Anchor.Speed;

            duration = dist / speed;
        }
        else
        {
            duration = Anchor.Speed;
        }

        Anchor.DOKill();
        Anchor.Rb.DOMove(target, duration)
            .SetEase(Anchor.Ease)
            .OnComplete(Anchor.FinishCurrentExecution);
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        MoveToBlockController controller = (MoveToBlockController)c;
        controller.InputX.text = target.x.ToString();
        controller.InputY.text = target.y.ToString();
    }

    public override AnchorBlockData GetData() => new MoveBlockData(target);
}