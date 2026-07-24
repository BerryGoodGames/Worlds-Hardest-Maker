public class PlacementPreviewController : PreviewController
{
    protected override void Start()
    {
        base.Start();
        
        EventBus.Subscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        EventBus.Subscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
    
    private void OnSwitchToPlay(SwitchToPlayEvent evt) => gameObject.SetActive(false);
    private void OnSwitchToEdit(SwitchToEditEvent evt) => Activate();
    
    public void Activate()
    {
        // enable placement preview and place it at mouse
        gameObject.SetActive(true);
        transform.position = FollowMouse.GetCurrentMouseWorldPos(FollowMouseComp.WorldPosition);
    }
    
    private void OnDestroy()
    {
        EventBus.Unsubscribe<SwitchToPlayEvent>(OnSwitchToPlay);
        EventBus.Unsubscribe<SwitchToEditEvent>(OnSwitchToEdit);
    }
}