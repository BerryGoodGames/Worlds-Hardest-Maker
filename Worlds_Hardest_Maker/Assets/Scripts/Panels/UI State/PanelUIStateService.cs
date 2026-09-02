namespace WorldsHardestMaker.Panels
{
    public class PanelUIStateService
    {
        private readonly IAnchorManager anchorManager;
        
        public PanelUIStateService(IAnchorManager anchorManager)
        {
            this.anchorManager = anchorManager;
        }
        
        public PanelUIState GetCurrentUIState()
        {
            if (LevelSessionEditManager.Instance.IsPlaying) return PanelUIState.Playing;
        
            bool isAnchorSelected = anchorManager.SelectedAnchor != null;
            bool isAttaching = AnchorAttachManager.Instance.InAttachMode;
            bool isPositionInputEditing = AnchorPositionInputEditManager.Instance.IsEditing;
        
            if (isPositionInputEditing) return PanelUIState.EditingAnchorPositionInputEditing;
            if (isAttaching) return PanelUIState.EditingAnchorAttaching;
            if (isAnchorSelected) return PanelUIState.EditingAnchorSelected;
            return PanelUIState.EditingGeneral;
        }
    }
}