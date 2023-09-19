using System.Collections;
using MyBox;
using UnityEngine;

public class AnchorPositionInputEditManager : MonoBehaviour
{
    public static AnchorPositionInputEditManager Instance { get; private set; }

    [ReadOnly] public bool IsEditing;
    [ReadOnly] public AnchorBlockPositionInputController CurrentEditedPositionInput;

    public void StartPositionInputEdit(AnchorBlockPositionInputController positionInput)
    {
        CurrentEditedPositionInput = positionInput;
        StartCoroutine(EditCoroutine());
    }

    public void OnStartPositionEdit()
    {
        IsEditing = true;

        // block menu from opening
        MenuManager.Instance.BlockMenu = true;

        // disable panels
        ReferenceManager.Instance.ToolbarTween.SetPlay(true);
        ReferenceManager.Instance.InfobarEditTween.SetPlay(true);
        ReferenceManager.Instance.PlayButtonTween.TweenToY(-125, false);

        PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
        PanelManager.Instance.SetPanelHidden(anchorPanel, true);
    }

    public void OnEndPositionEdit()
    {
        if (!IsEditing) return;

        IsEditing = false;
        CurrentEditedPositionInput = null;

        // show panels
        ReferenceManager.Instance.ToolbarTween.SetPlay(EditModeManager.Instance.Playing);
        ReferenceManager.Instance.InfobarEditTween.SetPlay(EditModeManager.Instance.Playing);
        ReferenceManager.Instance.PlayButtonTween.SetPlay(EditModeManager.Instance.Playing);

        PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
        PanelManager.Instance.SetPanelOpen(anchorPanel, EditModeManager.Instance.Editing);

        AnchorManager.Instance.SelectedAnchor.RenderLines();
    }

    public IEnumerator EditCoroutine()
    {
        if (CurrentEditedPositionInput == null) yield break;

        OnStartPositionEdit();

        PositionAnchorBlockController anchorBlockController = CurrentEditedPositionInput.AnchorBlockController;
        PositionAnchorBlockController nextAnchorBlockController = null;

        // get next position anchor block controller
        bool getNext = false;
        bool gotNextController = false;
        bool onlyMoveSecondArrow = false;

        foreach (AnchorBlockController currentAnchorBlock in ReferenceManager.Instance.MainChainController.Children)
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
        {
            // check if loop block is present
            if (AnchorManager.Instance.SelectedAnchor.LoopBlockIndex != -1)
            {
                // get first position block after loop block
                for (int i = AnchorManager.Instance.SelectedAnchor.LoopBlockIndex; i < ReferenceManager.Instance.MainChainController.Children.Count; i++)
                {
                    AnchorBlockController anchorBlock = ReferenceManager.Instance.MainChainController.Children[i];

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
            if (Input.GetKey(KeyCode.Escape) || SelectionManager.Instance.Selecting || EditModeManager.Instance.Playing)
            {
                OnEndPositionEdit();
                yield break;
            }

            // animate current & next line (only if mouse position changed)
            Vector2 mousePos = MouseManager.Instance.MouseWorldPosGrid;

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
        Instance.CurrentEditedPositionInput.SetPositionValues(MouseManager.Instance.MouseWorldPosGrid);

        Instance.OnEndPositionEdit();

        // make sure that the player can't place directly after pasting
        while (!Input.GetMouseButtonUp(0))
        {
            yield return null;
        }
    }


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}