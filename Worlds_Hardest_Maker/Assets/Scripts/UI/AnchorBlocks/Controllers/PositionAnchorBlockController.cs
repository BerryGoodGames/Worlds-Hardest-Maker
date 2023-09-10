using MyBox;

public abstract class PositionAnchorBlockController : AnchorBlockController
{
    [Separator("Position")] [InitializationField] [MustBeAssigned]
    public AnchorBlockPositionInputController PositionInput;
}