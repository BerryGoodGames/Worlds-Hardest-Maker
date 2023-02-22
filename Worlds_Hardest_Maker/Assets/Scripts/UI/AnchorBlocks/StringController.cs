using System.Collections.Generic;
using UnityEngine;

public class StringController : MonoBehaviour
{
    public Transform ConnectorContainer;

    public List<AnchorBlock> GetAnchorBlocks(AnchorController anchorController)
    {
        List<AnchorBlock> anchorBlocks = new();

        foreach (AnchorBlockController controller in GetComponentsInChildren<AnchorBlockController>())
        {
            anchorBlocks.Add(controller.GetAnchorBlock(anchorController));
        }

        return anchorBlocks;
    }
}