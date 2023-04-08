using System;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

public class WaitBlock : AnchorBlock
{
    public enum Unit
    {
        SECONDS, MINUTES, HOURS, DAYS
    }

    public const Type BlockType = Type.WAIT;
    public override Type ImplementedBlockType => BlockType;

    private readonly float waitTime;

    public WaitBlock(AnchorController anchor, float waitTime, Unit unit) : base(anchor)
    {
        float factor = unit switch
        {
            Unit.SECONDS => 1,
            Unit.MINUTES => 60,
            Unit.HOURS => 3600,
            Unit.DAYS => 86400,
            _ => throw new Exception("WaitBlock: Couldn't parse unit")
        };

        this.waitTime = waitTime * factor;
    }

    public override void Execute()
    {
        Anchor.Rb.DOMove(Anchor.Rb.position, waitTime)
            .OnComplete(Anchor.FinishCurrentExecution);
    }

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = Object.Instantiate(PrefabManager.Instance.WaitBlockPrefab, parent);

        // set values in object
        WaitBlockController controller = block.GetComponent<WaitBlockController>();
        controller.DurationInput.text = waitTime.ToString();
        controller.Movable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }

    public override AnchorBlockData GetData()
    {
        throw new NotImplementedException();
    }
}