namespace WorldsHardestMaker.Panels
{
    public interface IPanelService
    {
        public void SetPanelOpen(IPanel panel, bool open);

        public void SetPanelHidden(IPanel panel, bool hidden);

        public void CloseAllPanels();

        public void HideAllPanels();

        public bool TryCloseOnEscape();
    }
}