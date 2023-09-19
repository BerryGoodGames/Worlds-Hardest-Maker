using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public partial class AnchorBlockManager : MonoBehaviour
{
    public static AnchorBlockManager Instance { get; private set; }

    [ReadOnly] public bool DraggingBlock;
    [ReadOnly] public AnchorBlockController DraggedBlock;

    public bool IsConnectorHovered => ReferenceManager.Instance.AnchorBlockConnectorController.MouseOverUIRect.Over;
    public bool IsPreviewHovered => ReferenceManager.Instance.AnchorBlockPreview.MouseOverUIRect.Over;
    public bool IsPeriblockerHovered => ReferenceManager.Instance.AnchorBlockPreview.Periblocker.MouseOverUIRect.Over;

    #region Block insertion

    /// <summary>
    /// Inserts given anchor block into given string at given index
    /// </summary>
    /// <param name="anchorBlock">The anchor block to insert, if nothing passed then <c>AnchorManager.Instance.DraggedBlock</c> is passed</param>
    /// <param name="paramChain">The chain the anchor block gets inserted to, if nothing passed then <c>ReferenceManager.Instance.MainChainController</c> is passed</param>
    /// <param name="siblingIndex">The sibling index the anchor block gets inserted at, if nothing passed then anchor block gets inserted at the end</param>
    private static void InsertAnchorBlockIntoChain(AnchorBlockController anchorBlock = null,
        ChainController paramChain = null, int siblingIndex = -1)
    {
        if (anchorBlock == null) anchorBlock = Instance.DraggedBlock;
        if (paramChain == null) paramChain = ReferenceManager.Instance.MainChainController;

        // move dragged block to this string
        Transform anchorBlockTransform = anchorBlock.transform;
        Transform stringTransform = paramChain.transform;

        anchorBlockTransform.SetParent(stringTransform);
        if (siblingIndex > 0) anchorBlockTransform.SetSiblingIndex(siblingIndex);

        // disable preview
        ReferenceManager.Instance.AnchorBlockPreview.Deactivate();

        // reset tracking of hovered block
        Instance.HoveredBlockIndex = -1;

        // update position of connector
        ReferenceManager.Instance.AnchorBlockConnectorController.UpdateY();

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)stringTransform);

        // update list of blocks in anchor
        ReferenceManager.Instance.MainChainController.UpdateChildrenArray();
        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

        // check warnings
        AnchorManager.Instance.CheckStartRotatingWarnings();
        AnchorManager.Instance.CheckStackOverflowWarnings();

        // track loop block index
        if (anchorBlock is LoopBlockController)
        {
            AnchorManager.Instance.SelectedAnchor.LoopBlockIndex = siblingIndex;
        }

        AnchorController selectedAnchor = AnchorManager.Instance.SelectedAnchor;
        selectedAnchor.RenderLines();

        // highlight arrow
        if (anchorBlock is PositionAnchorBlockController controller)
        {
            controller.SetBlurVisible(true);
        }
    }

    /// <summary>
    /// Checks if dragged block is over any block in main string and inserts if so
    /// </summary>
    public static void CheckBlockInsert()
    {
        if (!IsAnyBlockHovered() && !Instance.IsPreviewHovered && !Instance.IsPeriblockerHovered) return;

        InsertAnchorBlockIntoChain(siblingIndex: Instance.HoveredBlockIndex + 1);
    }

    /// <summary>
    /// Checks if dragged block is over any block and returns the hovered block if successful
    /// </summary>
    public static bool IsAnyBlockHovered(bool includeLockedBlocks = false) =>
        GetOverdraggedBlock(includeLockedBlocks) != null;

    public static bool IsBlockHovered(int stringIndex)
    {
        AnchorBlockController overdraggedBlock = GetOverdraggedBlock(true);

        if (overdraggedBlock == null) return false;

        return overdraggedBlock.GetChainIndex() == stringIndex;
    }

    /// <summary>
    /// Returns the block the user is currently dragging the dragged block into (or null is failed)
    /// </summary>
    public static AnchorBlockController GetOverdraggedBlock(bool includeLockedBlocks = false)
    {
        ChainController mainChain = ReferenceManager.Instance.MainChainController;
        AnchorBlockController[] anchorBlocksInChain = mainChain.GetComponentsInChildren<AnchorBlockController>();

        foreach (AnchorBlockController anchorBlock in anchorBlocksInChain)
        {
            MouseOverUIRect mouseOver = anchorBlock.GetComponent<MouseOverUIRect>();

            if (!mouseOver.Over || Instance.DraggedBlock == anchorBlock ||
                (anchorBlock.IsLocked && !includeLockedBlocks)) continue;

            return anchorBlock;
        }

        return null;
    }

    /// <summary>
    /// Checks if dragged block is over connector and inserts if so
    /// </summary>
    public static void CheckConnectorInsert()
    {
        if (!Instance.IsConnectorHovered) return;

        InsertAnchorBlockIntoChain();
    }

    #endregion

    /// <summary>
    /// Destroys loose strings, destroys all anchor blocks in main string, destroys anchor connectors
    /// </summary>
    public static void EmptyAnchorChains()
    {
        // destroy loose strings (ignore main string and anchor connector)
        List<GameObject> strings = new();
        foreach (Transform s in ReferenceManager.Instance.AnchorBlockChainContainer)
        {
            strings.Add(s.gameObject);
        }

        for (int i = 2; i < strings.Count; i++)
        {
            DestroyImmediate(strings[i]);
        }

        // destroy anchor blocks in main string (ignore start block and preview)
        List<GameObject> anchorBlocks = new();
        foreach (Transform anchorBlock in ReferenceManager.Instance.MainChainController.transform)
        {
            anchorBlocks.Add(anchorBlock.gameObject);
        }

        for (int i = 1; i < anchorBlocks.Count; i++)
        {
            // ignore preview object
            if (anchorBlocks[i].CompareTag("AnchorBlockPreview")) continue;

            DestroyImmediate(anchorBlocks[i]);
        }
    }

    /// <summary>
    /// Empties strings, converts anchor blocks from given anchor to UI and applies it to the scene
    /// </summary>
    /// <param name="anchor"><para>Anchor, from which the blocks are fetched</para>If nothing passed, selected anchor gets passed instead</param>
    public static void LoadAnchorBlocks(AnchorController anchor = null)
    {
        // check for null
        if (anchor == null)
        {
            if (AnchorManager.Instance.SelectedAnchor == null)
            {
                Debug.LogWarning("Tried to load anchor blocks, but both argument and selected anchor are null");
                return;
            }

            anchor = AnchorManager.Instance.SelectedAnchor;
        }

        EmptyAnchorChains();

        // create objects
        AnchorBlock[] blocks = anchor.Blocks.ToArray();

        foreach (AnchorBlock t in blocks)
        {
            t.CreateAnchorBlockObject();
        }

        AnchorManager.Instance.UpdateBlockListInSelectedAnchor();

        AnchorManager.Instance.CheckStartRotatingWarnings();
        AnchorManager.Instance.CheckStackOverflowWarnings();

        RectTransform stringController = (RectTransform)ReferenceManager.Instance.MainChainController.transform;
        LayoutRebuilder.ForceRebuildLayoutImmediate(stringController);
    }

    private static void UpdateSourceBlocksLayout()
    {
        // Get the reference to the container that holds the anchor block sources
        RectTransform sourceContainer = ReferenceManager.Instance.AnchorBlockSourceContainer;

        // Get all the AnchorBlockSource components that are children of the sourceContainer
        AnchorBlockSource[] sources = sourceContainer.GetComponentsInChildren<AnchorBlockSource>();

        // Iterate through each AnchorBlockSource component
        foreach (AnchorBlockSource source in sources)
        {
            AnchorBlockRebuilder rebuilder = source.GetComponent<AnchorBlockRebuilder>();
            rebuilder.RebuildLayout();
        }
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        // If the current object is not the instance, return
        if (Instance != this) return;

        // Call the method to update the layout of the source block
        UpdateSourceBlocksLayout();
    }

    private void LateUpdate() => HoveringLateUpdate();
    // Dbg.Text(HoveredBlockIndex);

    public void OnMainChainUpdate()
    {
    }
}