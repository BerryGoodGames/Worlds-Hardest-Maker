using System;
using System.Collections.Generic;
using UnityEngine;

public partial class AnchorManager : MonoBehaviour
{
    public static AnchorManager Instance { get; private set; }

    private static readonly int selected = Animator.StringToHash("Selected");
    private static readonly int playing = Animator.StringToHash("Playing");

    /// <summary>
    /// Enables LevelSettingsPanel and disables AnchorEditorPanel (or the other way around)
    /// </summary>
    /// <param name="enableLevelSettingsPanel"></param>
    public static void AlternatePanels(bool enableLevelSettingsPanel)
    {
        if (enableLevelSettingsPanel)
        {
            ReferenceManager.Instance.AnchorEditorButtonPanelTween.Set(false);
            ReferenceManager.Instance.AnchorEditorPanelTween.Set(false);

            ReferenceManager.Instance.LevelSettingsButtonPanelTween.Set(true);
        }
        else
        {
            ReferenceManager.Instance.AnchorEditorButtonPanelTween.Set(true);

            ReferenceManager.Instance.LevelSettingsButtonPanelTween.Set(false);
            ReferenceManager.Instance.LevelSettingsPanelTween.Set(false);
        }
    }

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
        
        UpdateWarnings();
    }

    public void UpdateWarnings()
    {   
        // check for unexecutable start rotating blocks
        bool canStartRotateWork = false;

        LinkedListNode<AnchorBlock> currentNode = SelectedAnchor.Blocks.First;

        for (int i = 0; i < SelectedAnchor.Blocks.Count; i++)
        {
            if (currentNode == null)
            {
                Debug.LogWarning("the node should not be null here");
                break;
            }

            AnchorBlock block = currentNode.Value;

            switch (block.ImplementedBlockType)
            {
                case AnchorBlock.Type.SetRotation:
                {
                    // update if start rotating blocks can work
                    SetRotationBlock setRotationBlock = (SetRotationBlock)block;
                    canStartRotateWork = setRotationBlock.GetUnit() != SetRotationBlock.Unit.Time;
                    break;
                }
                // update if it can't rotate
                case AnchorBlock.Type.StartRotating:
                    ReferenceManager.Instance.MainChainController.Children[i].SetWarning(!canStartRotateWork);
                    break;
            }

            currentNode = currentNode.Next;
        }
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