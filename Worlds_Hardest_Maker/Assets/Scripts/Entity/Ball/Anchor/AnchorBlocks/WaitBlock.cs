using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitBlock : AnchorBlock, IActiveAnchorBlock, IDurationBlock
{
    public enum Unit
    {
        Seconds,
        Minutes,
        Hours,
        Days
    }

    private static readonly Dictionary<Unit, float> factors = new()
    {
        { Unit.Seconds, 1 },
        { Unit.Minutes, 60 },
        { Unit.Hours, 3600 },
        { Unit.Days, 86400 }
    };

    public const Type BlockType = Type.Wait;
    public override Type ImplementedBlockType => BlockType;
    protected override GameObject Prefab => PrefabManager.Instance.WaitBlockPrefab;

    private readonly float input;

    private readonly Unit unit;

    public WaitBlock(AnchorController anchor, bool isLocked, float input, Unit unit) : base(anchor, isLocked)
    {
        this.input = input;
        this.unit = unit;
    }

    public override void Execute() => Anchor.WaitCoroutine = Anchor.StartCoroutine(WaitCoroutine());

    private IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(input * factors[unit]);

        Anchor.FinishCurrentExecution();
    }

    protected override void SetControllerValues(AnchorBlockController c)
    {
        WaitBlockController controller = (WaitBlockController)c;
        controller.DurationInput.text = input.ToString();
        controller.UnitInput.value =
            GameManager.GetDropdownValue(WaitBlockController.GetOption(unit), controller.UnitInput);
    }

    public override AnchorBlockData GetData() => new WaitBlockData(IsLocked, input, unit);
    public bool HasCurrentlyDuration() => input > 0;
}