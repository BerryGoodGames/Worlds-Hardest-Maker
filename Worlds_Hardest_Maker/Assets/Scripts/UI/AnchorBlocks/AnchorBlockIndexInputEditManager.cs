using System.Collections;
using MyBox;
using UnityEngine;

public class AnchorBlockIndexInputEditManager : MonoBehaviour
{
    public static AnchorBlockIndexInputEditManager Instance { get; private set; }

    [ReadOnly] public bool IsEditing;
    [ReadOnly] public AnchorBlockIndexInputController CurrentEditedIndexInput;

    public void StartIndexInputEdit(AnchorBlockIndexInputController indexInput)
    {
        CurrentEditedIndexInput = indexInput;
        StartCoroutine(EditCoroutine());
    }

    public void OnStartIndexEdit()
    {
        IsEditing = true;

        // block menu from opening
        MenuManager.Instance.BlockMenu = true;

        // disable panels
        ReferenceManager.Instance.ToolbarTween.SetPlay(true);
        ReferenceManager.Instance.InfobarEditTween.SetPlay(true);
        ReferenceManager.Instance.PlayButtonTween.TweenToY(-125, false);
    }

    public void OnEndIndexEdit()
    {
        if (!IsEditing) return;

        IsEditing = false;
        CurrentEditedIndexInput = null;

        // release menu
        MenuManager.Instance.BlockMenu = false;

        // show panels
        ReferenceManager.Instance.ToolbarTween.SetPlay(EditModeManager.Instance.Playing);
        ReferenceManager.Instance.InfobarEditTween.SetPlay(EditModeManager.Instance.Playing);
        ReferenceManager.Instance.PlayButtonTween.SetPlay(EditModeManager.Instance.Playing);
    }

    public IEnumerator EditCoroutine()
    {
        if (CurrentEditedIndexInput == null) yield break;

        OnStartIndexEdit();

        // wait until clicked, cancel if esc is pressed
        while (!Input.GetMouseButton(0) || !AnchorBlockManager.IsAnyBlockHovered(true))
        {
            // cancel if these things happen
            if (Input.GetKey(KeyCode.Escape) || SelectionManager.Instance.Selecting || EditModeManager.Instance.Playing)
            {
                OnEndIndexEdit();
                yield break;
            }

            yield return null;
        }

        // apply index to index input
        Instance.CurrentEditedIndexInput.SetIndexValue(AnchorBlockManager.Instance.HoveredBlockIndex);

        Instance.OnEndIndexEdit();

        // make sure that the player can't place directly after pasting
        while (!Input.GetMouseButtonUp(0)) yield return null;
    }


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}