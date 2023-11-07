public class TeleportBlockController : PositionAnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController) =>
        new TeleportBlock(
            anchorController,
            IsLocked, GetPositionInput()
        );
}