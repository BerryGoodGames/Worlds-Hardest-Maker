using UnityEngine;
using VContainer;

/// <summary>
/// DIESER CODE IST SO SCHLECHT HOLY SHIT ES IST ALLES SO UNÜBERSICHTLICH ZEIGE DEINEM ARBEITGEBER NIEMALS DIESEN CODE
/// </summary>
public class PanelManager : MonoBehaviour, IPanelService
{
    [Inject] private EventBus eventBus;
    [Inject] private IPanelRegistry panelRegistry;
    
    public void SetPanelOpen(IPanel panel, bool open, bool noAnimation = true)
    {
        panel.SetOpen(open);
        
        if (!open)
        {
            eventBus.Fire(new PanelClosedEvent(panel));
            return;
        }

        HideExclusionGroupSiblings(panel, noAnimation);

        // if (!hideOtherPanels) return;
        //
        // foreach (IPanel otherPanel in panelRegistry.RegisteredPanels)
        // {
        //     if (otherPanel == panel) continue;
        //     
        //     otherPanel.SetHidden(true);
        // }
    }
    
    public void SetPanelHidden(IPanel panel, bool hidden, bool noAnimation = true)
    {
        panel.SetHidden(hidden, noAnimation);
        
        // // if showing panel, hide every other panel
        // if (hidden) return;
        //
        // if (!noAnimation) return;
        //
        // foreach (IPanel otherPanel in panelRegistry.RegisteredPanels)
        // {
        //     if (otherPanel == panel) continue;
        //     
        //     otherPanel.SetHidden(true);
        // }
    }
    
    public void CloseAllPanels()
    {
        foreach (IPanel panel in panelRegistry.RegisteredPanels)
        {
            SetPanelOpen(panel, false);
        }
    }
    
    public void HideAllPanels()
    {
        foreach (IPanel panel in panelRegistry.RegisteredPanels)
        {
            panel.SetHidden(true);
        }
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
    
    private void HideExclusionGroupSiblings(IPanel panel, bool noAnimation)
    {
        if (panel.ExclusionGroup == PanelExclusionGroup.None)
            return;

        foreach (IPanel otherPanel in panelRegistry.RegisteredPanels)
        {
            if (otherPanel == panel) continue;

            if (otherPanel.ExclusionGroup != panel.ExclusionGroup) continue;

            if (otherPanel.Hidden) continue;

            otherPanel.SetHidden(true, noAnimation);
        }
    }
}