using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.Selection;

namespace WorldsHardestMaker.CopyPaste
{
    public class CopyPasteManager : MonoBehaviour, ICopyPasteService
    {
        private readonly List<CopyData> clipboard = new();
    
        [SerializeField] [ReadOnly] private bool isPasting;
        public bool IsPasting => isPasting;
    
        [Space] [SerializeField] private PastePreviewService pastePreviewService;

        [Inject] private EventBus eventBus;
        [Inject] private IToastService toastService;
        [Inject] private IMouseService mouseService;
        [Inject] private ISelectionStateService selectionStateService;
        [Inject] private IEditModeUIBlockerService uiBlockerService;
        [Inject] private CopyDataFactory copyDataFactory;

        [Inject]
        private void Construct(IObjectResolver diContainer)
        {
            pastePreviewService.Initialize(diContainer);
        }
    
        public void Copy(SelectionArea area)
        {
            if (AnchorAttachManager.Instance.InAttachMode)
            {
                toastService.ShowError("Cannot copy in attach mode", 4);
                return;
            }
        
            AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

            List<CopyData> newClipboardData = copyDataFactory.FromArea(area);
            if (newClipboardData.Count == 0)
            {
                toastService.ShowError("Nothing found to copy", 4);
                return;
            }
        
            clipboard.Clear();
            clipboard.AddRange(newClipboardData);
        
            toastService.ShowInfo("Selection copied to clipboard", 4);
        }
    
        public IEnumerator PasteCoroutine()
        {
            if (clipboard.Count == 0)
            {
                toastService.ShowWarning("Clipboard is empty", 4);
                yield break;
            }
            
            if (AnchorAttachManager.Instance.InAttachMode)
            {
                toastService.ShowError("Cannot paste in attach mode", 4);
                yield break;
            }
        
            StartPaste();
        
            // wait until clicked, cancel if esc is pressed
            while (!Input.GetMouseButton(0))
            {
                // cancel if these things happen
                if (Input.GetKey(KeyCode.Escape) 
                    || selectionStateService.IsSelecting 
                    || LevelSessionEditManager.Instance.IsPlaying)
                {
                    CancelPaste();
                    yield break;
                }
            
                yield return null;
            }
        
            Paste();
        
            // make sure that the player can't place directly after pasting
            while (!Input.GetMouseButtonUp(0)) yield return null;
        
            isPasting = false;
        }
    
        private void StartPaste()
        {
            // // actions the frame the user starts pasting
            isPasting = true;
        
            pastePreviewService.CreatePreview(clipboard);
        
            uiBlockerService.BlockAndDisable();
        }
    
        private void CancelPaste()
        {
            isPasting = false;
        
            pastePreviewService.ClearPreview();
        
            uiBlockerService.ReleaseAndShow();
        }
    
        private void Paste()
        {
            // // actions to actually paste
            // get position where to paste
            Vector2 mousePos = mouseService.MouseWorldPosMatrix;
        
            PasteClipboard(mousePos);
        
            pastePreviewService.ClearPreview();
        
            uiBlockerService.ReleaseAndShow();
            // original:
            // toolbarTween.SetPlay(false);
            // infobarEditTween.SetPlay(false);
            // playButtonTween.SetPlay(false);
            // TODO: check if uiBlockerService is equivalent
        
            eventBus.Fire(new EditActionEvent());
        }
    
        private void PasteClipboard(Vector2 centerPosition)
        {
            foreach (CopyData copyData in clipboard)
            {
                Vector2 position = centerPosition + copyData.RelativePos;
                copyData.Data.ImportToLevel(position);
            }
        }
    }
}