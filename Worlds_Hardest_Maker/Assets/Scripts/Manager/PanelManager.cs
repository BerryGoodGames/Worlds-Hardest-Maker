using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public static PanelManager Instance { get; private set; }

    [ReadOnly] public List<PanelController> Panels;

    public bool WasAnchorPanelOpen { get; set; }

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
        foreach (PanelController panel in Panels) SetPanelOpen(panel, false);
    }

    public void HideAllPanels()
    {
        foreach (PanelController panel in Panels) SetPanelHidden(panel, true);
    }

    private void OnSwitchToPlay()
    {
        // hide all panels
        PanelController levelSettingsPanel = ReferenceManager.Instance.LevelSettingsPanelController;
        SetPanelHidden(levelSettingsPanel, true);

        PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
        WasAnchorPanelOpen = anchorPanel.Open;
        SetPanelHidden(anchorPanel, true);
    }

    private void OnSwitchToEdit()
    {
        // show level setting / anchor panel
        bool isEditModeAnchorRelated = LevelSessionEditManager.Instance.CurrentEditMode.Attributes.IsAnchorRelated;
        PanelController levelSettingsPanel = ReferenceManager.Instance.LevelSettingsPanelController;
        PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
        if (isEditModeAnchorRelated)
        {
            if (WasAnchorPanelOpen) SetPanelOpen(anchorPanel, true);
            else SetPanelHidden(anchorPanel, false);
        }
        else SetPanelHidden(levelSettingsPanel, false);
    }

    private void Start()
    {
        PlayManager.Instance.OnSwitchToPlay += OnSwitchToPlay;
        PlayManager.Instance.OnSwitchToEdit += OnSwitchToEdit;
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}