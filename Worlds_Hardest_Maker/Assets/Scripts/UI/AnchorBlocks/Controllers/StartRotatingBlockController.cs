public class StartRotatingBlockController : AnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController) => new StartRotatingBlock(anchorController, IsLocked);
}