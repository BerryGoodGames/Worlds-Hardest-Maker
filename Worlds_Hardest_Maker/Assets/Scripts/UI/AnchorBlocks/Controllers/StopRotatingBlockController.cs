public class StopRotatingBlockController : AnchorBlockController
{
    public override AnchorBlock GetAnchorBlock(AnchorController anchorController) =>
        new StopRotatingBlock(anchorController, IsLocked);
}