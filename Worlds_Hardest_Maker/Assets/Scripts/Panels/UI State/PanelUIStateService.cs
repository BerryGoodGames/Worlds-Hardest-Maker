namespace WorldsHardestMaker.Panels
{
    public class PanelUIStateService
    {
        public PanelUIState GetCurrentUIState()
        {
            if (LevelSessionEditManager.Instance.IsPlaying) return PanelUIState.Playing;
        
            bool isAnchorSelected = AnchorManager.Instance.SelectedAnchor != null;
            bool isAttaching = AnchorAttachManager.Instance.InAttachMode;
            bool isPositionInputEditing = AnchorPositionInputEditManager.Instance.IsEditing;
        
            if (isPositionInputEditing) return PanelUIState.EditingAnchorPositionInputEditing;
            if (isAttaching) return PanelUIState.EditingAnchorAttaching;
            if (isAnchorSelected) return PanelUIState.EditingAnchorSelected;
            return PanelUIState.EditingGeneral;
        }
    }
}