using DG.Tweening;
using UnityEngine;

public class SetEaseBlock : AnchorBlock, IPassiveAnchorBlock
{
    public const Type BLOCK_TYPE = Type.Ease;
    public override Type ImplementedBlockType => BLOCK_TYPE;
    protected override GameObject Prefab => Anchor.PrefabProvider.SetEaseBlockPrefab;
    
    private readonly Ease ease;
    
    public SetEaseBlock(AnchorController anchor, bool isLocked, Ease ease) : base(anchor, isLocked) => this.ease = ease;
    
    public override void Execute()
    {
        Anchor.Ease = ease;
        Anchor.FinishCurrentExecution();
    }
    
    protected override void SetControllerValues(AnchorBlockController c)
    {
        SetEaseBlockController controller = (SetEaseBlockController)c;
        controller.Input.value = GameManager.Instance.GetDropdownValue(SetEaseBlockController.GetOption(ease), controller.Input);
    }
    
    public override AnchorBlockData GetData() => new SetEaseBlockData(IsLocked, ease);
}