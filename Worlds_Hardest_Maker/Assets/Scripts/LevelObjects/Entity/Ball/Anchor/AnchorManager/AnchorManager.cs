using System.Collections.Generic;
using UnityEngine;

public partial class AnchorManager : MonoBehaviour
{
    public static AnchorManager Instance { get; private set; }

    private static readonly int selected = Animator.StringToHash("Selected");
    private static readonly int playing = Animator.StringToHash("Playing");

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        EditModeManagerOther.Instance.OnPlay += GameManager.DeselectInputs;
        EditModeManagerOther.Instance.OnPlay += UpdateBlockListInSelectedAnchor;

        EditModeManagerOther.Instance.OnEdit += () => ReferenceManager.Instance.AnchorInPlayModeScreen.SetVisible(false);
        EditModeManagerOther.Instance.OnPlay += () => ReferenceManager.Instance.AnchorInPlayModeScreen.SetVisible(true);
    }

    private void Update() => CheckAnchorSelection();

    /// <summary>
    ///     If anchor selected, convert anchor blocks in UI to <see cref="List{T}">List</see>&lt;<see cref="AnchorBlock" />&gt;
    ///     and apply it to selected anchor
    /// </summary>
    public void UpdateBlockListInSelectedAnchor()
    {
        if (SelectedAnchor == null) return;

        ReferenceManager.Instance.MainChainController.UpdateChildrenArray();
        List<AnchorBlock> blocksInChain = ReferenceManager.Instance.MainChainController.GetAnchorBlocks(SelectedAnchor);

        SelectedAnchor.Blocks = new(blocksInChain);
    }

    public void UpdateSelectedAnchorLines()
    {
        AnchorController selectedAnchor = Instance.SelectedAnchor;
        if (selectedAnchor == null) return;

        // update list of blocks in anchor
        Instance.UpdateBlockListInSelectedAnchor();

        selectedAnchor.RenderLines();
    }

    public void ResetStates()
    {
        // reset anchors
        foreach (Transform t in ReferenceManager.Instance.AnchorContainer)
        {
            AnchorParentController parent = t.GetComponent<AnchorParentController>();
            AnchorController anchor = parent.Child;

            anchor.ResetExecution();
            anchor.Animator.SetBool(playing, false);

            if (SelectedAnchor == anchor &&
                EditModeManagerOther.Instance.CurrentEditMode.IsAnchorRelated()) anchor.SetLinesActive(true);
        }
    }

    public void StartExecuting()
    {
        UpdateBlockListInSelectedAnchor();

        // let anchors start executing
        foreach (Transform t in ReferenceManager.Instance.AnchorContainer)
        {
            AnchorParentController parent = t.GetComponent<AnchorParentController>();
            AnchorController anchor = parent.Child;

            anchor.StartExecuting();

            anchor.SetLinesActive(false);

            if (SelectedAnchor == anchor &&
                EditModeManagerOther.Instance.CurrentEditMode.IsAnchorRelated()) continue;

            anchor.Animator.SetBool(playing, true);
        }
    }
}