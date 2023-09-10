using MyBox;

public partial class AnchorBlockManager
{
    [ReadOnly] public int HoveredBlockIndex = -1;

    public AnchorBlockController ExecuteBlockOnHover { get; set; }
    public AnchorBlockController ExecuteBlockOnUnhover { get; set; }
    public bool ExecutePreviewOnUnhover { get; set; }
    public bool ExecutePeriblockerOnUnhover { get; set; }
    public bool ExecuteConnectorOnHover { get; set; }
    public bool ExecuteConnectorOnUnhover { get; set; }


    private void HoveringLateUpdate()
    {
        // OnUnhover always before OnHover
        // OnUnhover stuff
        if (ExecuteBlockOnUnhover != null)
        {
            ExecuteBlockOnUnhover.OnUnhover();
            ExecuteBlockOnUnhover = null;
        }

        if (ExecutePreviewOnUnhover)
        {
            ReferenceManager.Instance.AnchorBlockPreview.OnUnhover();
            ExecutePreviewOnUnhover = false;
        }

        if (ExecutePeriblockerOnUnhover)
        {
            ReferenceManager.Instance.AnchorBlockPeriblocker.OnUnhover();
            ExecutePeriblockerOnUnhover = false;
        }

        if (ExecuteConnectorOnUnhover)
        {
            ReferenceManager.Instance.AnchorBlockConnectorController.OnUnhover();
            ExecuteConnectorOnUnhover = false;
        }

        // OnHover stuff
        // OnBlockHover before OnConnectorHover
        if (ExecuteBlockOnHover != null)
        {
            ExecuteBlockOnHover.OnHover();
            ExecuteBlockOnHover = null;
        }

        if (ExecuteConnectorOnHover)
        {
            ReferenceManager.Instance.AnchorBlockConnectorController.OnHover();
            ExecuteConnectorOnHover = false;
        }
    }
}