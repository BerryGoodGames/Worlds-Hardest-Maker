using DG.Tweening;
using System;
using System.Globalization;
using UnityEngine;
using Object = UnityEngine.Object;

public class MoveBlock : AnchorBlock
{
    public const Type BlockType = Type.MOVE_TO;
    public override Type ImplementedBlockType => BlockType;

    private readonly Vector2 target;

    #region Constructors

    public MoveBlock(AnchorController anchor, Vector2 target) : base(anchor)
    {
        this.target = target;
    }

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

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = Object.Instantiate(PrefabManager.Instance.MoveToBlockPrefab, parent);

        // set values in object
        MoveToBlockController controller = block.GetComponent<MoveToBlockController>();
        controller.InputX.text = target.x.ToString();
        controller.InputY.text = target.y.ToString();
        controller.Movable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }

    public override AnchorBlockData GetData()
    {
        return new MoveBlockData(target);
    }
}