using DG.Tweening;
using UnityEngine;

public interface IAnchorBlockExecutionContext
{
    public Component TweenComponent { get; }
    public MonoBehaviour CoroutineRunner { get; }
    
    public Transform Transform { get; }

    public Vector2 Position
    {
        get => Transform.position;
        set => Transform.position = value;
    }
    public float ZAngle { get; }
    
    public SetSpeedBlock.Unit SpeedUnit { get; set; }
    public float SpeedInput { get; set; }
    
    public RotationUnit RotationUnit { get; set; }
    public float RotationInput { get; set; }
    public bool IsClockwise { get; set; }
    public Tween RotationTween { get; set; }
    
    public Ease Ease { get; set; }

    public Coroutine WaitCoroutine { set; }
    
    public void StoreCurrentLoopIndex();
    public void FinishCurrentExecution();
}