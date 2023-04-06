using System;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class AnchorBlock
{
    public enum Type
    {
        GO_TO,
        MOVE_TO,
        ROTATE,
        MOVE_AND_ROTATE,
        WAIT,
        EASE,
        SET_SPEED,
        SET_ANGULAR_SPEED
    }

    protected AnchorController Anchor;

    protected AnchorBlock(AnchorController anchor)
    {
        Anchor = anchor;
    }

    // ReSharper disable once UnusedMember.Global
    public abstract Type ImplementedBlockType { get; }

    public abstract void Execute();

    public void CreateAnchorBlockObject(bool insertable = true)
    {
        CreateAnchorBlockObject(ReferenceManager.Instance.MainStringController.transform, insertable);
    }

    public abstract void CreateAnchorBlockObject(Transform parent, bool insertable = true);

    public void CreateAnchorConnector(Transform parent, int siblingIndex, bool insertable = true)
    {
        Object.Instantiate(PrefabManager.Instance.AnchorConnector, parent);
        if (insertable == false)
            parent.GetChild(siblingIndex - 1).GetComponent<AnchorConnectorController>().Dummy = true;
    }
}