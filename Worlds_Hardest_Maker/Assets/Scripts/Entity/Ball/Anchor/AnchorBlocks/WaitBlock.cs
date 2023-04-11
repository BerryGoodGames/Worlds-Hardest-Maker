using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WaitBlock : AnchorBlock
{
    public enum Unit
    {
        SECONDS,
        MINUTES,
        HOURS,
        DAYS
    }

    private static readonly Dictionary<Unit, float> factors = new()
    {
        { Unit.SECONDS, 1 },
        { Unit.MINUTES, 60 },
        { Unit.HOURS, 3600 },
        { Unit.DAYS, 86400 }
    };

    public const Type BlockType = Type.WAIT;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.WaitBlockPrefab;

    private readonly float input;

    private readonly Unit unit;

    public WaitBlock(AnchorController anchor, float input, Unit unit) : base(anchor)
    {
        this.input = input;
        this.unit = unit;
    }

    public override void Execute()
    {
        Anchor.Rb.DOMove(Anchor.Rb.position, input * factors[unit])
            .OnComplete(Anchor.FinishCurrentExecution);
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        WaitBlockController controller = (WaitBlockController)c;
        controller.DurationInput.text = input.ToString();
        controller.UnitInput.value =
            GameManager.GetDropdownValue(WaitBlockController.GetOption(unit), controller.UnitInput);
    }

    public override AnchorBlockData GetData() => new WaitBlockData(input, unit);
}