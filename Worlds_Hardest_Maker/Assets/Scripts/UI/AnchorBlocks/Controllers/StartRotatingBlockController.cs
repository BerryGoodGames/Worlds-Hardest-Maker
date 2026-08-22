public class StartRotatingBlockController : AnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        return new StartRotatingBlock(IsLocked);
    }
}