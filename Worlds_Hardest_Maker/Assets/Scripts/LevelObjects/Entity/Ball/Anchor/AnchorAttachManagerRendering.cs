using MyBox;
using UnityEngine;

public partial class AnchorAttachManager
{
    [SerializeField] [InitializationField] [MustBeAssigned] private GameObject anchorAttachBlocker;
    
    public static void HighlightAnchor(AnchorController anchor)
    {
        Instance.anchorAttachBlocker.SetActive(true);
        
        anchor.MergeToLayer();
        anchor.Attachments.ForEach(attachment => attachment.MergeToLayer());
        
        anchor.AttachFade.FadeIn();
    }
    
    public static void Dehighlight(AnchorController anchor)
    {
        Instance.anchorAttachBlocker.SetActive(false);
        
        anchor.ResetLayer();
        anchor.Attachments.ForEach(attachment => attachment.ResetLayer());
        
        anchor.AttachFade.FadeOut();
    }
}