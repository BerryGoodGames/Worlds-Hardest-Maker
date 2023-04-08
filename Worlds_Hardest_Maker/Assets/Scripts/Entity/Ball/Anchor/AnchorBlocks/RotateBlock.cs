using System;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

public class RotateBlock : AnchorBlock
{
    public const Type BlockType = Type.ROTATE;
    public override Type ImplementedBlockType => BlockType;

    private readonly float iterations;

    public RotateBlock(AnchorController anchor, float iterations) : base(anchor)
    {
        this.iterations = iterations;
    }

    public override void Execute()
    {
        float duration;

        if (Anchor.ApplyAngularSpeed)
        {
            float currentZ = Anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + iterations * 360;
            float distance = targetZ - currentZ;

            duration = distance / Anchor.AngularSpeed;
        }
        else
        {
            duration = Anchor.AngularSpeed;
        }

        Anchor.Rb.DORotate(iterations * 360, duration)
            .SetRelative()
            .SetEase(Anchor.Ease)
            .OnComplete(Anchor.FinishCurrentExecution);
    }

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = Object.Instantiate(PrefabManager.Instance.RotateBlockPrefab, parent);

        // set values in object
        RotateBlockController controller = block.GetComponent<RotateBlockController>();
        controller.Input.text = iterations.ToString();
        controller.Movable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }

    public override AnchorBlockData GetData()
    {
        throw new NotImplementedException();
    }
}