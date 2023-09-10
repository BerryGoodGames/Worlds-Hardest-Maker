using System.Collections.Generic;
using UnityEngine;

public class ChainController : MonoBehaviour
{
    [HideInInspector] public List<AnchorBlockController> Children;

    /// <summary>
    /// Collects its <see cref="AnchorBlockController"/>s in a list and converts them into list <see cref="AnchorBlock"/>s
    /// </summary>
    public List<AnchorBlock> GetAnchorBlocks(AnchorController anchorController)
    {
        if (Children == null) UpdateChildrenArray();

        List<AnchorBlock> anchorBlocks = new();

        foreach (AnchorBlockController controller in Children!)
        {
            anchorBlocks.Add(controller.GetAnchorBlock(anchorController));
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
            if (child.TryGetComponent(out AnchorBlockController component))
            {
                Children.Add(component);
            }
        }
    }
}