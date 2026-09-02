using System;
using System.Collections;
using MyBox;
using UnityEngine;
using VContainer;
using WorldsHardestMaker.Selection;

public class AnchorPositionInputEditManager : MonoBehaviour
{
    public static AnchorPositionInputEditManager Instance { get; private set; }
    
    [ReadOnly] public bool IsEditing;
    [ReadOnly] public AnchorBlockPositionInputController CurrentEditedPositionInput;
    
    [SerializeField] [InitializationField] [MustBeAssigned] private ChainController mainChainController;

    [Inject] private EventBus eventBus;
    [Inject] private IAudioService audioService;
    [Inject] private IMouseService mouseService;
    [Inject] private ISelectionStateService selectionStateService;
    [Inject] private IEditModeUIBlockerService uiBlockerService;
    [Inject] private IAnchorManager anchorManager;

    public void StartPositionInputEdit(AnchorBlockPositionInputController positionInput)
    {
        CurrentEditedPositionInput = positionInput;
        StartCoroutine(EditCoroutine());
    }
    
    private void OnStartPositionEdit()
    {
        IsEditing = true;
        
        uiBlockerService.BlockAndDisable();
        
        eventBus.Fire(new AnchorPositionEditStartedEvent());
    }
    
    private void OnEndPositionEdit()
    {
        if (!IsEditing) return;
        
        IsEditing = false;
        CurrentEditedPositionInput = null;
        
        uiBlockerService.ReleaseAndShow();
        // original:
        // toolbarTween.SetPlay(LevelSessionEditManager.Instance.Playing);
        // infobarEditTween.SetPlay(LevelSessionEditManager.Instance.Playing);
        // playButtonTween.SetPlay(LevelSessionEditManager.Instance.Playing);
        // no Menu.BlockMenu = false ! TODO: check if working
        
        anchorManager.SelectedAnchor.RenderLines();
        
        eventBus.Fire(new AnchorPositionEditEndedEvent());
    }
    
    private IEnumerator EditCoroutine()
    {
        if (CurrentEditedPositionInput == null) yield break;
        
        OnStartPositionEdit();
        
        PositionAnchorBlockController anchorBlockController = CurrentEditedPositionInput.AnchorBlockController;
        
        if (anchorBlockController == null)
            throw new Exception("Anchor block controller of position input controller is null. Failed to start position input coroutine.");
        
        PositionAnchorBlockController nextAnchorBlockController = null;
        
        // get next position anchor block controller
        bool getNext = false;
        bool gotNextController = false;
        bool onlyMoveSecondArrow = false;
        
        foreach (AnchorBlockController currentAnchorBlock in mainChainController.Children)
        {
            // skip anchor blocks before this anchor blocks
            if (currentAnchorBlock == anchorBlockController)
            {
                getNext = true;
                continue;
            }
            
            if (!getNext) continue;
            
            if (currentAnchorBlock is not PositionAnchorBlockController controller) continue;
            
            nextAnchorBlockController = controller;
            gotNextController = true;
            break;
        }
        
        if (!gotNextController)
            // check if loop block is present
        {
            if (anchorManager.SelectedAnchor.LoopBlockIndex != -1)
                // get first position block after loop block
            {
                for (int i = anchorManager.SelectedAnchor.LoopBlockIndex;
                     i < mainChainController.Children.Count;
                     i++)
                {
                    AnchorBlockController anchorBlock = mainChainController.Children[i];
                    
                    if (anchorBlock is not PositionAnchorBlockController controller) continue;
                    
                    nextAnchorBlockController = controller;
                    gotNextController = true;
                    onlyMoveSecondArrow = true;
                    break;
                }
            }
        }
        
        Vector2? previousMousePos = null;
        // wait until clicked, cancel if esc is pressed
        while (!Input.GetMouseButton(0))
        {
            // cancel if these things happen
            if (Input.GetKey(KeyCode.Escape) || selectionStateService.IsSelecting || LevelSessionEditManager.Instance.IsPlaying)
            {
                OnEndPositionEdit();
                yield break;
            }
            
            // animate current & next line (only if mouse position changed)
            Vector2 mousePos = mouseService.MouseWorldPosGrid;
            
            if (previousMousePos == null || mousePos != previousMousePos)
            {
                anchorBlockController.Lines.ForEach(line => line.AnimateEnd(mousePos));
                if (gotNextController)
                {
                    if (onlyMoveSecondArrow) nextAnchorBlockController.Lines[^1].AnimateStart(mousePos);
                    else nextAnchorBlockController.Lines[0].AnimateStart(mousePos);
                }
            }
            
            previousMousePos = mousePos;
            yield return null;
        }
        
        // apply position to position input
        CurrentEditedPositionInput.SetPositionValues(mouseService.MouseWorldPosGrid);
        
        // make sure that the player can't place directly after pasting
        while (!Input.GetMouseButtonUp(0)) yield return null;
        
        OnEndPositionEdit();
        
        // play sfx
        audioService.Play(PlaceManager.Instance.DefaultPlaceSfx);
    }
    
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}