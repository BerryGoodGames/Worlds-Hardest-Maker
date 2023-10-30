public class MoveBlockController : PositionAnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController) =>
        new MoveBlock(
            anchorController,
            IsLocked, GetPositionInput()
        );
}