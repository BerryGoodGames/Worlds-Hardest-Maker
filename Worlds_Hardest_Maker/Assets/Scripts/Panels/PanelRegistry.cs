using System.Collections.Generic;

namespace WorldsHardestMaker.Panels
{
    public class PanelRegistry : IPanelRegistry
    {
        private readonly List<IPanel> panels = new();

        public IReadOnlyCollection<IPanel> RegisteredPanels => panels.AsReadOnly();
    
        public void Register(IPanel panel)
        {
            if(!panels.Contains(panel)) panels.Add(panel);
        }

        public void Unregister(IPanel panel)
        {
            panels.Remove(panel);
        }
    }
}