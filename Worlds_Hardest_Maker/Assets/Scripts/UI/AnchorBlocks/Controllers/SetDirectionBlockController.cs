using MyBox;

public class SetDirectionBlockController : AnchorBlockController
{
    [Separator("Specifics")] [InitializationField] public AnchorBlockDirectionController DirectionInput;
    
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        return new SetDirectionBlock(IsLocked, DirectionInput.IsClockwise);
    }
}