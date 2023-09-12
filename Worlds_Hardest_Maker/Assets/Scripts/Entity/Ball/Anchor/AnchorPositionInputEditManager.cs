using System;
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

        // release menu
        MenuManager.Instance.BlockMenu = false;

        // show panels
        ReferenceManager.Instance.ToolbarTween.SetPlay(EditModeManager.Instance.Playing);
        ReferenceManager.Instance.InfobarEditTween.SetPlay(EditModeManager.Instance.Playing);
        ReferenceManager.Instance.PlayButtonTween.SetPlay(EditModeManager.Instance.Playing);

        PanelController anchorPanel = ReferenceManager.Instance.AnchorPanelController;
        PanelManager.Instance.SetPanelOpen(anchorPanel, EditModeManager.Instance.Editing);
    }

    public IEnumerator EditCoroutine()
    {
        if (CurrentEditedPositionInput == null) yield break;

        OnStartPositionEdit();

        // wait until clicked, cancel if esc is pressed
        while (!Input.GetMouseButton(0))
        {
            // cancel if these things happen
            if (Input.GetKey(KeyCode.Escape) || SelectionManager.Instance.Selecting || EditModeManager.Instance.Playing)
            {
                OnEndPositionEdit();
                yield break;
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