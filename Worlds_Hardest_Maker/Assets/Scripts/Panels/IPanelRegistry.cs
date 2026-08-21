using System.Collections.Generic;

namespace WorldsHardestMaker.Panels
{
    public interface IPanelRegistry
    {
        public IReadOnlyCollection<IPanel> RegisteredPanels { get; }

        public void Register(IPanel panel);
        public void Unregister(IPanel panel);
    }
}