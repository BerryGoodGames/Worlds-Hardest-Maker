public class LoopBlockController : AnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        return new LoopBlock(IsLocked);
    }
}