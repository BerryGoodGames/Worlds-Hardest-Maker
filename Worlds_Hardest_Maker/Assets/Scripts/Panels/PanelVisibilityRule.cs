using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace WorldsHardestMaker.Panels
{
    [Serializable]
    public class PanelVisibilityRule
    {
        [SerializeField] [InitializationField] private PanelUIState uiState;
        [SerializeField] [InitializationField] private List<PanelController> visiblePanels;
        [SerializeField] [InitializationField] private List<HideableUIElement> visibleButtons;

        public PanelUIState UIState => uiState;
        public IEnumerable<IHideableUI> VisibleElements => visiblePanels.Concat(visibleButtons.Cast<IHideableUI>());
    }
}