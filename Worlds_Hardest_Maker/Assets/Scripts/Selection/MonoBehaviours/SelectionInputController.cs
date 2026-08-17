using UnityEngine;
using VContainer;
using Vector2 = UnityEngine.Vector2;

namespace WorldsHardestMaker.Selection
{
    public class SelectionInputController : MonoBehaviour
    {
        private IMouseService  mouseService;
        private ISelectionStateService selectionStateService;
    
        private Vector2 prevStart;
        private Vector2 prevEnd;

        [Inject]
        private void Construct(IMouseService mouseService, ISelectionStateService selectionStateService)
        {
            this.mouseService = mouseService;
            this.selectionStateService = selectionStateService;
        }
    
        private void Update()
        {
            if (!LevelSessionManager.Instance.IsEdit || AnchorAttachManager.Instance.InAttachMode) return;

            bool playing = LevelSessionEditManager.Instance.IsPlaying;
        
            if (!playing && KeyBinds.GetKeyBindDown("Editor_Select"))
            {
                Vector2 start = mouseService.DragStart ?? mouseService.MouseWorldPos;
                selectionStateService.StartSelection(start);
            }
        
            // update selection markings
            if (!playing
                && selectionStateService.IsSelecting
                && mouseService.DragStart != null
                && mouseService.DragCurrent != null)
            {
                (Vector2 start, Vector2 end) = mouseService.GetDragPositions();
            
                if (KeyBinds.GetKeyBindUp("Editor_Select")) selectionStateService.EndSelection(start, end);
            
                if (!prevStart.Equals(start) || !prevEnd.Equals(end)) selectionStateService.UpdateSelection(start, end);
            }
        
            if (Input.GetKeyDown(KeyCode.Escape)) selectionStateService.CancelSelection();
        }
    }
}