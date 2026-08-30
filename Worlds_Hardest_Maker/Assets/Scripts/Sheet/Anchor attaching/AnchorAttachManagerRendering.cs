using MyBox;
using UnityEngine;

public partial class AnchorAttachManager
{
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject anchorAttachBlocker;
    
    public void HighlightAnchor(AnchorController anchor)
    {
        anchorAttachBlocker.SetActive(true);
        
        anchor.MergeToLayer();
        anchor.Attachments.ForEach(attachment => attachment.MergeToLayer());
        
        anchor.AttachFade.FadeIn();
    }
    
    public void Dehighlight(AnchorController anchor)
    {
        anchorAttachBlocker.SetActive(false);
        
        anchor.ResetLayer();
        anchor.Attachments.ForEach(attachment => attachment.ResetLayer());
        
        anchor.AttachFade.FadeOut();
    }
}