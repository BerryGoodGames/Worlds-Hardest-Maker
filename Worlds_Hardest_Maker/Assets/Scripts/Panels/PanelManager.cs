using UnityEngine;
using VContainer;

/// <summary>
/// DIESER CODE IST SO SCHLECHT HOLY SHIT ES IST ALLES SO UNÜBERSICHTLICH ZEIGE DEINEM ARBEITGEBER NIEMALS DIESEN CODE
/// </summary>
public class PanelManager : MonoBehaviour, IPanelService
{
    [Inject] private IPanelRegistry panelRegistry;
    
    public void SetPanelOpen(IPanel panel, bool open, bool hideOtherPanels = true)
    {
        panel.SetOpen(open);
        
        // if opening panel, hide every other panel
        if (!open) return;
        
        if (!hideOtherPanels) return;
        
        foreach (IPanel otherPanel in panelRegistry.RegisteredPanels)
        {
            if (otherPanel == panel) continue;
            
            otherPanel.SetHidden(true);
        }
    }
    
    public void SetPanelHidden(IPanel panel, bool hidden, bool hideOtherPanels = true)
    {
        panel.SetHidden(hidden);
        
        // if showing panel, hide every other panel
        if (hidden) return;
        
        if (!hideOtherPanels) return;
        
        foreach (IPanel otherPanel in panelRegistry.RegisteredPanels)
        {
            if (otherPanel == panel) continue;
            
            otherPanel.SetHidden(true);
        }
    }
    
    public void CloseAllPanels()
    {
        foreach (IPanel panel in panelRegistry.RegisteredPanels) SetPanelOpen(panel, false);
    }
    
    public void HideAllPanels()
    {
        foreach (IPanel panel in panelRegistry.RegisteredPanels) SetPanelHidden(panel, true);
    }

    public bool TryCloseOnEscape()
    {
        bool closingPanel = false;
        
        if (!Input.GetKeyDown(KeyCode.Escape)) return false;
        
        foreach (IPanel panel in panelRegistry.RegisteredPanels)
        {
            if (!panel.Open || !panel.CloseOnEscape) continue;
            
            SetPanelOpen(panel, false);
            closingPanel = true;
        }
        
        return closingPanel;
    }
}