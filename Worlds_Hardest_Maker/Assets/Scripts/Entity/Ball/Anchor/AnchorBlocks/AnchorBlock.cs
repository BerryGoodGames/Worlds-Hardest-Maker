using UnityEngine;
using UnityEngine.UI;

public abstract class AnchorBlock
{
    public enum Type
    {
        Loop,
        Move,
        Teleport,
        Rotate,
        StartRotating,
        StopRotating,
        MoveAndRotate,
        Wait,
        Ease,
        SetSpeed,
        SetRotation,
        SetDirection
    }

    protected AnchorController Anchor;
    protected bool IsLocked;

    public AnchorBlockController Controller { get; set; }

    protected AnchorBlock(AnchorController anchor, bool isLocked)
    {
        Anchor = anchor;
        IsLocked = isLocked;
    }

    public abstract Type ImplementedBlockType { get; }
    protected abstract GameObject Prefab { get; }

    public void CreateAnchorBlockObject() =>
        CreateAnchorBlockObject(ReferenceManager.Instance.MainChainController.transform);

    private void CreateAnchorBlockObject(Transform parent)
    {
        // create object
        GameObject anchorBlock = Object.Instantiate(Prefab, parent);

        // set values in object
        Controller = anchorBlock.GetComponent<AnchorBlockController>();
        SetControllerValues(Controller);
        Controller.IsLocked = IsLocked;
        Controller.Block = this;

        // restrict
        UIRestrictInRectTransform restrict = anchorBlock.GetComponent<UIRestrictInRectTransform>();
        restrict.RectTransform = ReferenceManager.Instance.AnchorBlockChainContainer;

        // rebuild
        RectTransform anchorBlockRectTransform = (RectTransform)anchorBlock.transform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(anchorBlockRectTransform);

        // move anchor connector
        ReferenceManager.Instance.AnchorBlockConnectorController.UpdateYAtEndOfFrame();
    }

    public abstract void Execute();
    protected abstract void SetControllerValues(AnchorBlockController c);
    public abstract AnchorBlockData GetData();
}