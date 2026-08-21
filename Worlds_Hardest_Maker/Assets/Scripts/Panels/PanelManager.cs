using UnityEngine;
using VContainer;

namespace WorldsHardestMaker.Panels
{
    public class PanelManager : MonoBehaviour, IPanelService
    {
        [Inject] private EventBus eventBus;
        [Inject] private IPanelRegistry panelRegistry;
    
        public void SetPanelOpen(IPanel panel, bool open)
        {
            panel.SetOpen(open);
        
            if (!open)
            {
                eventBus.Fire(new PanelClosedEvent(panel));
                return;
            }

            HideExclusionGroupSiblings(panel);
        }
    
        public void SetPanelHidden(IPanel panel, bool hidden)
        {
            panel.SetHidden(hidden);
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
                if (!panel.IsOpen || !panel.CloseOnEscape) continue;
            
                SetPanelOpen(panel, false);
                closingPanel = true;
            }
        
            return closingPanel;
        }
    
        private void HideExclusionGroupSiblings(IPanel panel)
        {
            if (panel.ExclusionGroup == PanelExclusionGroup.None)
            {
                return;
            }

            foreach (IPanel otherPanel in panelRegistry.RegisteredPanels)
            {
                if (otherPanel == panel) continue;

                if (otherPanel.ExclusionGroup != panel.ExclusionGroup) continue;

                if (otherPanel.IsHidden) continue;

                otherPanel.SetHidden(true);
            }
        }
    }
}