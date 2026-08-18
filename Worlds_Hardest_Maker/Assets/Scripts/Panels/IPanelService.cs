public interface IPanelService
{
    public void SetPanelOpen(IPanel panel, bool open, bool hideOtherPanels = true);

    public void SetPanelHidden(IPanel panel, bool hidden, bool hideOtherPanels = true);

    public void CloseAllPanels();

    public void HideAllPanels();

    public bool TryCloseOnEscape();
}