using System.Collections;
using MyBox;
using UnityEngine;

public class AnchorBlockIndexInputEditManager : MonoBehaviour
{
    public static AnchorBlockIndexInputEditManager Instance { get; private set; }

    [SerializeField] [ReadOnly] private bool isEditing;
    [SerializeField] [ReadOnly] private AnchorBlockIndexInputController currentEditedIndexInput;

    public void StartIndexInputEdit(AnchorBlockIndexInputController indexInput)
    {
        currentEditedIndexInput = indexInput;
        StartCoroutine(EditCoroutine());
    }

    private void OnStartIndexEdit()
    {
        isEditing = true;

        // block menu from opening
        MenuManager.Instance.BlockMenu = true;

        // disable panels
        ReferenceManager.Instance.ToolbarTween.SetPlay(true);
        ReferenceManager.Instance.InfobarEditTween.SetPlay(true);
        ReferenceManager.Instance.PlayButtonTween.TweenToY(-125, false);
    }

    private void OnEndIndexEdit()
    {
        if (!isEditing) return;

        isEditing = false;
        currentEditedIndexInput = null;

        // release menu
        MenuManager.Instance.BlockMenu = false;

        // show panels
        ReferenceManager.Instance.ToolbarTween.SetPlay(EditModeManager.Instance.Playing);
        ReferenceManager.Instance.InfobarEditTween.SetPlay(EditModeManager.Instance.Playing);
        ReferenceManager.Instance.PlayButtonTween.SetPlay(EditModeManager.Instance.Playing);
    }

    private IEnumerator EditCoroutine()
    {
        if (currentEditedIndexInput == null) yield break;

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
        Instance.currentEditedIndexInput.SetIndexValue(AnchorBlockManager.Instance.HoveredBlockIndex);

        Instance.OnEndIndexEdit();

        // make sure that the player can't place directly after pasting
        while (!Input.GetMouseButtonUp(0)) yield return null;
    }


    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
}