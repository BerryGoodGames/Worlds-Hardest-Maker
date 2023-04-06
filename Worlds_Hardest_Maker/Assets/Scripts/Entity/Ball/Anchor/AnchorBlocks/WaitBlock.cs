using DG.Tweening;
using UnityEngine;

public class WaitBlock : AnchorBlock
{
    public const Type BlockType = Type.WAIT;
    public override Type ImplementedBlockType => BlockType;

    private readonly float waitTime;
    private float duration;

    public WaitBlock(AnchorController anchor, float waitTime) : base(anchor)
    {
        this.waitTime = waitTime;
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
        controller.Input.text = waitTime.ToString();
        controller.Movable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }
}