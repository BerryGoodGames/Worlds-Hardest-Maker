using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

[Serializable]
public class PanelVisibilityRule
{
    [SerializeField] [InitializationField] private PanelUIState uiState;
    [SerializeField] [InitializationField] private List<PanelController> visiblePanels;

    public PanelUIState UIState => uiState;
    public IReadOnlyList<PanelController> VisiblePanels => visiblePanels.AsReadOnly();
}