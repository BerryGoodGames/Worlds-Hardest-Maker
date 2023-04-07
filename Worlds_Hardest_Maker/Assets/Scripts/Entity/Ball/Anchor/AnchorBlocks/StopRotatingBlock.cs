using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StopRotatingBlock : AnchorBlock
{
    public StopRotatingBlock(AnchorController anchor) : base(anchor)
    {
    }
    public const Type BlockType = Type.STOP_ROTATING;
    public override Type ImplementedBlockType => BlockType;
    public override void Execute()
    {
        Anchor.InfiniteRotationTween.Kill();
        Anchor.FinishCurrentExecution();
    }

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = Object.Instantiate(PrefabManager.Instance.StopRotatingBlockPrefab, parent);

        // set values in object
        StopRotatingBlockController controller = block.GetComponent<StopRotatingBlockController>();
        controller.Movable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }
}
