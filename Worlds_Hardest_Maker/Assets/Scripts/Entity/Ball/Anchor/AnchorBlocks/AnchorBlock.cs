using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract class AnchorBlock
{
    public enum Type
    {
        GO_TO,
        MOVE_TO,
        ROTATE,
        START_ROTATING,
        STOP_ROTATING,
        MOVE_AND_ROTATE,
        WAIT,
        EASE,
        SET_SPEED,
        SET_ANGULAR_SPEED
    }

    protected AnchorController Anchor;

    protected AnchorBlock(AnchorController anchor) => Anchor = anchor;

    // ReSharper disable once UnusedMember.Global
    public abstract Type ImplementedBlockType { get; }
    protected abstract GameObject Prefab { get; }

    public abstract void Execute();

    public void CreateAnchorBlockObject(bool insertable = true)
    {
        CreateAnchorBlockObject(ReferenceManager.Instance.MainStringController.transform, insertable);
    }

    public void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = Object.Instantiate(Prefab, parent);

        // set values in object
        AnchorBlockController controller = block.GetComponent<AnchorBlockController>();
        SetControllerValues(controller);
        controller.Movable = insertable;

        // create connector
        RectTransform rt = ((RectTransform)block.transform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

        CreateAnchorConnector(connectorContainer, rt.sizeDelta, insertable, block.transform.GetSiblingIndex());
    }

    protected abstract void SetControllerValues(AnchorBlockController c);

    public static void CreateAnchorConnector(Transform parent, Vector2 size, bool insertable = true,
        int? siblingIndex = null)
    {
        GameObject connector = Object.Instantiate(PrefabManager.Instance.AnchorConnector, parent);
        RectTransform rt = (RectTransform)connector.transform;
        rt.sizeDelta = new(size.x, rt.sizeDelta.y);
        if (siblingIndex == null) siblingIndex = parent.childCount - 1;
        Transform last = parent.GetChild((int)siblingIndex - 1);
        RectTransform lastRt = (RectTransform)connector.transform;
        lastRt.sizeDelta = new(lastRt.sizeDelta.x, size.y);

        if (insertable == false)
        {
            last.GetComponent<AnchorConnectorController>().Dummy = true;
        }
            
    }

    public abstract AnchorBlockData GetData();
}