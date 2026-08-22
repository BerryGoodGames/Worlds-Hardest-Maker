public class StopRotatingBlockController : AnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController)
    {
        return new StopRotatingBlock(IsLocked);
    }
}