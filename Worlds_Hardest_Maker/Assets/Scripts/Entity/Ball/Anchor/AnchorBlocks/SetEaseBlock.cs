using DG.Tweening;
using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class SetEaseBlock : AnchorBlock
{
    public const Type BlockType = Type.EASE;
    public override Type ImplementedBlockType => BlockType;

    private readonly Ease ease;

    public SetEaseBlock(AnchorController anchor, Ease ease) : base(anchor)
    {
        this.ease = ease;
    }

    public override void Execute()
    {
        Anchor.Ease = ease;
        Anchor.FinishCurrentExecution();
    }

    public override void CreateAnchorBlockObject(Transform parent, bool insertable = true)
    {
        Transform connectorContainer = parent.GetChild(0);

        // create object
        GameObject block = Object.Instantiate(PrefabManager.Instance.SetEaseBlockPrefab, parent);

        // set values in object
        SetEaseBlockController controller = block.GetComponent<SetEaseBlockController>();
        controller.Input.value = GameManager.GetDropdownValue(SetEaseBlockController.GetOption(ease), controller.Input);
        controller.Movable = insertable;

        // create connector
        CreateAnchorConnector(connectorContainer, block.transform.GetSiblingIndex(), insertable);
    }

    public override AnchorBlockData GetData()
    {
        throw new System.NotImplementedException();
    }
}