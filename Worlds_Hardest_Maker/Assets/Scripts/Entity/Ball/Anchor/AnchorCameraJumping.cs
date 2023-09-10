using System;
using UnityEngine;

public class AnchorCameraJumping : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform anchorEditorPanel;
    [SerializeField] private PanelTween anchorEditorPanelTween;

    /// <summary>
    /// Lets main camera jump to currently selected anchor if anchor editor panel is open
    /// </summary>
    public void CameraJumpToAnchor()
    {
        if (!anchorEditorPanelTween.Open) return;

        if (!ReferenceManager.Instance.MainCameraJumper.HasKey("Anchor")) return;

        ReferenceManager.Instance.MainCameraJumper.Jump("Anchor", Vector2.left * GetAnchorOffset());
    }

    public float GetAnchorOffset()
    {
        float panelWidth = anchorEditorPanel.rect.width;

        if (Camera.main == null) throw new Exception("Couldn't calculate anchor offset because main camera is null");

        // calculate offset (offset = panelWidth / 2)
        float panelWidthUnits = UnitPixelUtils.CanvasSpaceToUnit(canvas, panelWidth);
        float anchorOffsetToCamera = panelWidthUnits / 2;
        return anchorOffsetToCamera;
    }
}