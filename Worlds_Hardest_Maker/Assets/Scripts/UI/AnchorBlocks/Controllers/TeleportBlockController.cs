public class TeleportBlockController : PositionAnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        return new TeleportBlock(anchorController, IsLocked, GetPositionInput() + anchorController.GetPosition());
    }
}
