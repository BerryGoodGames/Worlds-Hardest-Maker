using System.Collections.Generic;

public interface IPanelRegistry
{
    public IReadOnlyCollection<IPanel> RegisteredPanels { get; }

    public void Register(IPanel panel);
    public void Unregister(IPanel panel);
}