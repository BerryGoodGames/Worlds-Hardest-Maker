public class MoveBlockController : PositionAnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        return new MoveBlock(anchorController, IsLocked, GetPositionInput() + anchorController.GetPosition());
    }
}