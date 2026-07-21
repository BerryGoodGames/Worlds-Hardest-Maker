public class PlacementPreviewController : PreviewController
{
    protected override void Start()
    {
        base.Start();
        
        EventBus.Subscribe<SwitchToPlayEvent>(_ => gameObject.SetActive(false));
        EventBus.Subscribe<SwitchToEditEvent>(_ => Activate());
    }
    
    public void Activate()
    {
        // enable placement preview and place it at mouse
        gameObject.SetActive(true);
        transform.position =
            FollowMouse.GetCurrentMouseWorldPos(
                FollowMouseComp.WorldPosition
            );
    }
}