public class LoopBlockController : AnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController) => new LoopBlock(anchorController, IsLocked);
}