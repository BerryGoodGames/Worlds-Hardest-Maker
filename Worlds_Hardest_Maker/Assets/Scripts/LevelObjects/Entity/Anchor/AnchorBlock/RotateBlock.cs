using DG.Tweening;
using UnityEngine;

public class RotateBlock : AnchorBlock, IActiveAnchorBlock, IDurationBlock
{
    public const Type BLOCK_TYPE = Type.Rotate;
    public override Type ImplementedBlockType => BLOCK_TYPE;
    protected override GameObject Prefab => Anchor.PrefabProvider.RotateBlockPrefab;
    
    private readonly float iterations;
    
    public bool HasCurrentlyDuration => iterations > 0;
    
    public RotateBlock(AnchorController anchor, bool isLocked, float iterations) : base(anchor, isLocked) => this.iterations = iterations;
    
    public override void Execute()
    {
        float duration;
        
        if (Anchor.RotationSpeedUnit is SetRotationBlock.Unit.Degrees or SetRotationBlock.Unit.Iterations)
        {
            float speed = SetRotationBlock.GetSpeed(Anchor.RotationInput, Anchor.RotationSpeedUnit);
            
            float currentZ = Anchor.transform.localRotation.eulerAngles.z;
            float targetZ = currentZ + iterations * 360;
            float distance = targetZ - currentZ;
            
            duration = distance / speed;
        }
        else duration = Anchor.RotationInput;
        
        // negate rotation depending on direction
        int direction = Anchor.IsClockwise ? -1 : 1;
        
        Anchor.RotationTween.Kill();
        Anchor.RotationTween.Kill();
        Anchor.RotationTween = Anchor.transform.DORotate(iterations * 360 * direction * Vector3.forward, duration, RotateMode.FastBeyond360)
            .SetRelative()
            .SetEase(Anchor.Ease)
            .OnComplete(Anchor.FinishCurrentExecution);
    }
    
    protected override void SetControllerValues(AnchorBlockController c)
    {
        RotateBlockController controller = (RotateBlockController)c;
        controller.IterationsInput.text = iterations.ToString();
    }
    
    public override AnchorBlockData GetData() => new RotateBlockData(IsLocked, iterations);
}