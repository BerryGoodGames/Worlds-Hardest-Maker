public interface IPanelService
{
    public void SetPanelOpen(IPanel panel, bool open, bool noAnimation = false);

    public void SetPanelHidden(IPanel panel, bool hidden, bool noAnimation = false);

    public void CloseAllPanels();

    public void HideAllPanels();

    public bool TryCloseOnEscape();
}