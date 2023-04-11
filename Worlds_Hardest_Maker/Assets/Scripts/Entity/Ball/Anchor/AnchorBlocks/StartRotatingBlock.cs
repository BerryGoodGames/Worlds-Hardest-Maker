using DG.Tweening;
using UnityEngine;

public class StartRotatingBlock : AnchorBlock
{
    public StartRotatingBlock(AnchorController anchor) : base(anchor)
    {
    }

    public const Type BlockType = Type.START_ROTATING;
    public override Type ImplementedBlockType => BlockType;

    public override void Execute()
    {
        float duration;
        if (Anchor.ApplyAngularSpeed)
        {
            float currentZ = Anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + 360;
            float distance = targetZ - currentZ;

            duration = distance / Anchor.AngularSpeed;
        }
        else
        {
            duration = Anchor.AngularSpeed;
        }

        if (Anchor.ApplyAngularSpeed)
        {
            Anchor.InfiniteRotationTween.Kill();
            Anchor.InfiniteRotationTween = Anchor.Rb.DORotate(360, duration).SetRelative().SetLoops(-1)
                .SetEase(Anchor.Ease);
        }

        Anchor.FinishCurrentExecution();
    }

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = Object.Instantiate(PrefabManager.Instance.StartRotatingBlockPrefab, parent);

        // set values in object
        StartRotatingBlockController controller = block.GetComponent<StartRotatingBlockController>();
        controller.Movable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }

    public override AnchorBlockData GetData() => new StartRotatingBlockData();
}