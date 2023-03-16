using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockManager : MonoBehaviour
{
    public static bool DraggingBlock;
    public static AnchorBlockController DraggedBlock;

    public static void LoadAnchorBlocks(AnchorController anchor)
    {
        // destroy loose strings
        List<GameObject> strings = new();
        foreach (Transform s in ReferenceManager.Instance.AnchorBlockStringContainer)
        {
            strings.Add(s.gameObject);
        }

        for (int i = 1; i < strings.Count; i++)
        {
            Destroy(strings[i]);
        }

        // destroy anchor blocks in main string
        List<GameObject> anchorBlocks = new();
        foreach (Transform anchorBlock in ReferenceManager.Instance.MainStringController.transform)
        {
            anchorBlocks.Add(anchorBlock.gameObject);
        }

        for (int i = 2; i < anchorBlocks.Count; i++)
        {
            Destroy(anchorBlocks[i]);
        }

        // destroy anchor connectors
        List<GameObject> anchorConnectors = new();
        foreach (Transform anchorConnector in ReferenceManager.Instance.MainStringController.transform.GetChild(0))
        {
            anchorConnectors.Add(anchorConnector.gameObject);
        }

        for (int i = 2; i < anchorConnectors.Count; i++)
        {
            Destroy(anchorConnectors[i]);
        }

        // create objects
        AnchorBlock[] blocks = AnchorManager.Instance.SelectedAnchor.Blocks.ToArray();

        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].CreateAnchorBlockObject(i > 2);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(ReferenceManager.Instance.MainStringController
            .GetComponent<RectTransform>());
    }
}