using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.Selection;

public class CopyPasteManager : MonoBehaviour, ICopyPasteService
{
    private readonly List<CopyData> clipBoard = new();
    
    [SerializeField] [ReadOnly] private bool isPasting;
    public bool IsPasting => isPasting;
    
    [Space] [SerializeField] private PastePreviewService pastePreviewService;
    
    [Inject] private IObjectResolver diContainer;
    [Inject] private IToastService toastService;
    [Inject] private IMouseService mouseService;
    [Inject] private ISelectionStateService selectionStateService;
    [Inject] private IAreaQueryService areaQueryService;
    [Inject] private IEditModeUIBlockerService uiBlockerService;
    
    public void Copy(SelectionArea area)
    {
        if (AnchorAttachManager.Instance.InAttachMode)
        {
            toastService.ShowError("Cannot copy in attach mode", 4);
            return;
        }
        
        clipBoard.Clear();
        
        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();
        
        Collider2D[] hits = areaQueryService.QueryArea(area, LayerManager.Instance.Layers.LevelObjectMask);
        
        List<Vector2> points = HitsToPoints(hits);
        
        if (points.Count == 0)
        {
            toastService.ShowError("Nothing found to copy", 4);
            return;
        }
        
        (Vector2 lowest, Vector2 highest) = SelectionGeometry.GetBoundsMatrix(points);
        
        // center and size of actual controllers user selected
        Vector2 castCenter = ((lowest + highest) / 2).Floor();
        
        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;
            
            if (!hit.TryGetComponent(out LevelObjectController levelObjectController))
            {
                Debug.LogWarning($"Could not find level object controller on hit while copying: {hit.name}");
                continue;
            }

            if (!levelObjectController.IsCopyableNow()) continue;
            
            Data data = levelObjectController.GetData();
            
            Vector2 pos = levelObjectController.transform.position;
            Vector2 relativePos = pos - castCenter;
            
            CopyData copyData = new(data, relativePos);
            
            clipBoard.Add(copyData);
        }
        
        toastService.ShowInfo("Selection copied to clipboard", 4);
    }
    
    private static List<Vector2> HitsToPoints(Collider2D[] hits)
    {
        List<Vector2> points = new();
        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;
            
            points.Add(hit.transform.position);
        }
        
        return points;
    }
    
    public IEnumerator PasteCoroutine()
    {
        if (AnchorAttachManager.Instance.InAttachMode)
        {
            Debug.Log("Cannot paste in attach mode");
            yield break;
        }
        
        // check if there smth. in clipboard
        if (clipBoard.Count == 0) yield break;
        
        StartPaste();
        
        // wait until clicked, cancel if esc is pressed
        while (!Input.GetMouseButton(0))
        {
            // cancel if these things happen
            if (Input.GetKey(KeyCode.Escape) || selectionStateService.IsSelecting || LevelSessionEditManager.Instance.Playing)
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
        
        pastePreviewService.CreatePreview(clipBoard);
        
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
        
        // paste
        LoadClipboard(mousePos);
        
        pastePreviewService.ClearPreview();
        
        uiBlockerService.ReleaseAndShow();
        // original:
        // toolbarTween.SetPlay(false);
        // infobarEditTween.SetPlay(false);
        // playButtonTween.SetPlay(false);
        // TODO: check if uiBlockerService is equivalent
    }
    
    public void LoadClipboard(Vector2 pos)
    {
        // just load clipboard to pos
        foreach (CopyData copyData in clipBoard) copyData.Paste(pos);
    }
}