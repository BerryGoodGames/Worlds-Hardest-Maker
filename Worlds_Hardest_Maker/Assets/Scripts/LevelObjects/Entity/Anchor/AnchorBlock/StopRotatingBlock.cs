using DG.Tweening;
using UnityEngine;

public class StopRotatingBlock : AnchorBlock, IActiveAnchorBlock
{
    public StopRotatingBlock(AnchorController anchor, bool isLocked) : base(anchor, isLocked) { }
    
    public const Type BLOCK_TYPE = Type.StopRotating;
    public override Type ImplementedBlockType => BLOCK_TYPE;
    protected override GameObject Prefab => PrefabManager.Instance.StopRotatingBlockPrefab;
    
    public override void Execute()
    {
        Anchor.RotationTween.Kill();
        Anchor.FinishCurrentExecution();
    }
    
    protected override void SetControllerValues(AnchorBlockController c) { }
    
    public override AnchorBlockData GetData() => new StopRotatingBlockData(IsLocked);
}