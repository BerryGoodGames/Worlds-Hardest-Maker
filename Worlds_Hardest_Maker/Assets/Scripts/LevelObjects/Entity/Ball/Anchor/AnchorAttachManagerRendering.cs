using MyBox;
using UnityEngine;

public partial class AnchorAttachManager
{
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject anchorAttachBlocker;
    
    private static void HighlightAnchor(AnchorController anchor)
    {
        Instance.anchorAttachBlocker.SetActive(true);
        
        AnchorManager.Instance.SelectedAnchor.MergeToLayer();
        anchor.Attachments.ForEach(attachment => attachment.MergeToLayer());
        
        anchor.AttachFade.FadeIn();
    }
    
    private static void Dehighlight(AnchorController anchor)
    {
        Instance.anchorAttachBlocker.SetActive(false);
        
        AnchorManager.Instance.SelectedAnchor.ResetLayer();
        anchor.Attachments.ForEach(attachment => attachment.ResetLayer());
        
        anchor.AttachFade.FadeOut();
    }
}