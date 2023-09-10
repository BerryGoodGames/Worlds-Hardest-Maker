using MyBox;

public class SetDirectionBlockController : AnchorBlockController
{
    [Separator("Specifics")] [InitializationField]
    public AnchorBlockDirectionController DirectionInput;

    public override AnchorBlock GetAnchorBlock(AnchorController anchorController) =>
        new SetDirectionBlock(anchorController, IsLocked, DirectionInput.IsClockwise);
}