public class PlacementPreviewController : PreviewController
{
    protected override void Start()
    {
        base.Start();
        
        PlayManager.Instance.OnSwitchToPlay += () => gameObject.SetActive(false);
        PlayManager.Instance.OnSwitchToEdit += Activate;
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
