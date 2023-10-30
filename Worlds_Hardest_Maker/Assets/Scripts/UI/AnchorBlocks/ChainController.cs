using System.Collections.Generic;
using UnityEngine;

public class ChainController : MonoBehaviour
{
    [HideInInspector] public List<AnchorBlockController> Children;

    /// <summary>
    ///     Collects <see cref="AnchorBlockController" />s from UI in a list and converts them into list
    ///     <see cref="AnchorBlock" />s
    /// </summary>
    public List<AnchorBlock> GetAnchorBlocks(AnchorController anchorController)
    {
        if (Children == null) UpdateChildrenArray();

        List<AnchorBlock> anchorBlocks = new();

        foreach (AnchorBlockController controller in Children!)
        {
            AnchorBlock anchorBlock = controller.GetAnchorBlock(anchorController);
            anchorBlock.Controller = controller;
            anchorBlock.Controller.Block = anchorBlock;
            anchorBlocks.Add(anchorBlock);
        }

        return anchorBlocks;
    }

    public AnchorBlockController GetAnchorBlockByChainIndex(int stringIndex) => Children[stringIndex - 1];

    public void UpdateChildrenArray()
    {
        // children = GetComponentsInChildren<AnchorBlockController>();
        Children = new();

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out AnchorBlockController component)) Children.Add(component);
        }
    }
}