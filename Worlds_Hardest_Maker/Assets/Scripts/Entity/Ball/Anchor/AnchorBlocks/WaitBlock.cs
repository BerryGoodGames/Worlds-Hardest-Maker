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

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = Object.Instantiate(PrefabManager.Instance.WaitBlockPrefab, parent);

        // set values in object
        WaitBlockController controller = block.GetComponent<WaitBlockController>();
        controller.DurationInput.text = input.ToString();
        controller.UnitInput.value =
            GameManager.GetDropdownValue(WaitBlockController.GetOption(unit), controller.UnitInput);
        controller.Movable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }

    public override AnchorBlockData GetData() => new WaitBlockData(input, unit);
}