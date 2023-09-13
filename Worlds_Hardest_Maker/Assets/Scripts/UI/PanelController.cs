using System;
using MyBox;
using UnityEngine;

[RequireComponent(typeof(PanelTween))]
public class PanelController : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned]
    private PanelTween panelTween;

    [SerializeField] [InitializationField] [MustBeAssigned]
    private PanelTween buttonPanelTween;

    [field:Separator("Initial settings")][field:SerializeField][field:InitializationField] public bool Open { get; private set; }
    [field:SerializeField][field:InitializationField] public bool Hidden { get; private set; }
    [field:SerializeField][field:InitializationField] public bool CloseOnEscape { get; private set; }

    public void ToggleOpen(bool noAnimation = false)
    {
        SetOpen(!Open, noAnimation);
    }

    public void SetOpen(bool open, bool noAnimation = false)
    {
        // open/close panel
        Open = open;

        panelTween.SetOpen(Open, noAnimation);

        // un-hide panel if hidden
        if (Open & Hidden) SetHidden(false, noAnimation);
    }

    public void ToggleHidden(bool noAnimation = false)
    {
        SetHidden(!Hidden, noAnimation);
    }

    public void SetHidden(bool hidden, bool noAnimation = false)
    {
        // hide/show button
        Hidden = hidden;

        // close panel if open
        if (Hidden && Open) SetOpen(false, noAnimation);

        buttonPanelTween.SetOpen(!Hidden, noAnimation);
    }

    private void Start()
    {
        // track this in manager list
        PanelManager.Instance.Panels.Add(this);

        panelTween.SetOpen(Open, true);
        buttonPanelTween.SetOpen(!Hidden, true);
    }

    public void OnButtonToggle()
    {
        if (!buttonPanelTween.Open) return;

        PanelManager.Instance.SetPanelOpen(this, !Open);
    }

    private void OnDestroy()
    {
        // track this in manager list
        PanelManager.Instance.Panels.Remove(this);
    }
}
