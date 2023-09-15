using System;
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
        EditModeManager.Instance.OnPlay += GameManager.DeselectInputs;
        EditModeManager.Instance.OnPlay += UpdateBlockListInSelectedAnchor;

        EditModeManager.Instance.OnEdit += () => ReferenceManager.Instance.AnchorInPlayModeScreen.SetVisible(false);
        EditModeManager.Instance.OnPlay += () => ReferenceManager.Instance.AnchorInPlayModeScreen.SetVisible(true);
    }

    /// <summary>
    /// If anchor selected, convert anchor blocks in UI to <see cref="List{T}">List</see>&lt;<see cref="AnchorBlock"/>&gt; and apply it to selected anchor
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
}