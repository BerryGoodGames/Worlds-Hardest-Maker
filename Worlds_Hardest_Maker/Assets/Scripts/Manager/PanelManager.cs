using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    [ReadOnly] public List<PanelController> Panels;

    public void SetPanelOpen(PanelController panel, bool open)
    {
        panel.SetOpen(open);

        // if opening panel, hide every other panel
        if (!open) return;

        foreach (PanelController panelController in Panels)
        {
            if (panelController == panel) continue;

            panelController.SetHidden(true);
        }
    }

    public void SetPanelHidden(PanelController panel, bool hidden)
    {
        panel.SetHidden(hidden);

        // if showing panel, hide every other panel
        if (hidden) return;

        foreach (PanelController panelController in Panels)
        {
            if (panelController == panel) continue;

            panelController.SetHidden(true);
        }
    }

    public void CloseAllPanels()
    {
        foreach (PanelController panel in Panels)
        {
            SetPanelOpen(panel, false);
        }
    }

    public void HideAllPanels()
    {
        foreach (PanelController panel in Panels)
        {
            SetPanelHidden(panel, true);
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}