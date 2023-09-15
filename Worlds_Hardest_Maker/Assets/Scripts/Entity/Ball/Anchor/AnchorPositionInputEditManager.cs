using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using DG.Tweening;
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

        // activate preview
        // AnchorController anchorController = AnchorManager.Instance.SelectedAnchor;
        // PositionAnchorBlockController anchorBlockController = CurrentEditedPositionInput.AnchorBlockController;
        // PositionAnchorBlock anchorBlock = anchorBlockController.Block;
        // Vector2 start = anchorController.GetLinePreviewStartVertex(anchorBlockController);
        // bool dashed = anchorBlock.IsLinePreviewDashed();
        // anchorController.ActivatePreview(start, dashed);

    }

    public void OnEndPositionEdit()
    {
        if (!IsEditing) return;

        IsEditing = false;
        CurrentEditedPositionInput = null;

        // release menu
        // MenuManager.Instance.BlockMenu = false;

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

        foreach (Transform t in anchorBlockController.transform.parent)
        {
            if (t.CompareTag("StartBlock")) continue;
            if (t.CompareTag("AnchorBlockPreview")) continue;
            if (t == anchorBlockController.transform)
            {
                getNext = true;
                continue;
            }
            if (!getNext) continue;

            nextAnchorBlockController = t.GetComponent<PositionAnchorBlockController>();
            gotNextController = true;
            break;
        }

        // wait until clicked, cancel if esc is pressed
        while (!Input.GetMouseButton(0))
        {
            // cancel if these things happen
            if (Input.GetKey(KeyCode.Escape) || SelectionManager.Instance.Selecting || EditModeManager.Instance.Playing)
            {
                OnEndPositionEdit();
                yield break;
            }

            // animate current line
            anchorBlockController.LineAnimator.AnimatePoint(1, MouseManager.Instance.MouseWorldPosGrid, 0.05f, Ease.Linear);
            
            (Vector2 arrowVertex1, Vector2 arrowVertex2, Vector2 arrowCenter) = AnchorManager.Instance.SelectedAnchor.GetArrowHeadPoints(MouseManager.Instance.MouseWorldPosGrid, anchorBlockController.Line.GetPosition(0));
            anchorBlockController.ArrowLines.line1.AnimateAllPoints(new() { arrowCenter, arrowVertex1 }, 0.05f,
                Ease.Linear);
            anchorBlockController.ArrowLines.line2.AnimateAllPoints(new() { arrowCenter, arrowVertex2 }, 0.05f, Ease.Linear);
            
            // animate next line
            if (gotNextController)
            {
                nextAnchorBlockController.LineAnimator.AnimatePoint(0, MouseManager.Instance.MouseWorldPosGrid,
                    0.05f, Ease.Linear);
                (Vector2 nextArrowVertex1, Vector2 nextArrowVertex2, Vector2 nextArrowCenter) = AnchorManager.Instance.SelectedAnchor.GetArrowHeadPoints(nextAnchorBlockController.Line.GetPosition(1), MouseManager.Instance.MouseWorldPosGrid);
                nextAnchorBlockController.ArrowLines.line1.AnimateAllPoints(new() { nextArrowCenter, nextArrowVertex1 }, 0.05f,
                    Ease.Linear);
                nextAnchorBlockController.ArrowLines.line2.AnimateAllPoints(new() { nextArrowCenter, nextArrowVertex2 }, 0.05f, Ease.Linear);
            }

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