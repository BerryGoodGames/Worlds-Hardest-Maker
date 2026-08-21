namespace WorldsHardestMaker.Panels
{
    public class PanelClosedEvent
    {
        public IPanel ClosedPanel { get; private set; }

        public PanelClosedEvent(IPanel closedPanel)
        {
            ClosedPanel = closedPanel;
        }
    }
}