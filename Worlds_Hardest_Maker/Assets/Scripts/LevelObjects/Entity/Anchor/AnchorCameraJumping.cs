using MyBox;
using UnityEngine;

public class AnchorCameraJumping : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private Canvas canvas;
    [SerializeField] [InitializationField] [MustBeAssigned] private RectTransform anchorEditorPanel;
    [SerializeField] [InitializationField] [MustBeAssigned] private PanelTween anchorEditorPanelTween;
    [SerializeField] [InitializationField] [MustBeAssigned] private JumpToEntity mainCameraJumper;
    
    /// <summary>
    ///     Lets main camera jump to currently selected anchor if anchor editor panel is open
    /// </summary>
    public void CameraJumpToAnchor()
    {
        if (!anchorEditorPanelTween.Open) return;
        
        if (!mainCameraJumper.HasKey("Anchor")) return;
        
        mainCameraJumper.Jump("Anchor", Vector2.left * GetAnchorOffset());
    }
    
    public float GetAnchorOffset()
    {
        float panelWidth = anchorEditorPanel.rect.width;
        
        if (Camera.main == null) throw new("Couldn't calculate anchor offset because main camera is null");
        
        // calculate offset (offset = panelWidth / 2)
        float panelWidthUnits = UnitPixelUtils.CanvasSpaceToUnit(canvas, panelWidth);
        float anchorOffsetToCamera = panelWidthUnits / 2;
        return anchorOffsetToCamera;
    }
}