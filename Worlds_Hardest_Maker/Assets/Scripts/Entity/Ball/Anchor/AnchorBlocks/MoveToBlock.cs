using DG.Tweening;
using System;
using System.Globalization;
using UnityEngine;
using Object = UnityEngine.Object;

public class MoveToBlock : AnchorBlock
{
    public const Type BlockType = Type.MOVE_TO;
    public override Type ImplementedBlockType => BlockType;

    private readonly Vector2 target;

    private readonly RotateBlock rotateBlock;

    #region Constructors

    public MoveToBlock(AnchorController anchor, Vector2 target, RotateBlock rotateBlock = null) : base(anchor)
    {
        this.target = target;
        this.rotateBlock = rotateBlock;
    }

    public MoveToBlock(AnchorController anchor, float x, float y, RotateBlock rotateBlock = null) : this(anchor,
        new(x, y), rotateBlock)
    {
    }

    #endregion

    public override void Execute(bool executeNext = true)
    {
        Debug.Log("asd");
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

        // TODO: rethink about other system
        if (rotateBlock != null)
        {
            rotateBlock.CustomTime = duration;
            rotateBlock.CustomIterations = Anchor.AngularSpeed / 360 * duration;
            rotateBlock.Execute(false);
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
        controller.IsInsertable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }
}