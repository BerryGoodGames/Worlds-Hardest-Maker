using System.Collections.Generic;
using UnityEngine;

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
        foreach (Transform anchorConnector in ReferenceManager.Instance.MainStringController.transform.GetChild(0))
        {
            Destroy(anchorConnector.gameObject);
        }
    }

    private void CreateAnchorBlockObject(AnchorBlock anchorBlock, Transform parent, bool insertable = true)
    {
        switch (anchorBlock.ImplementedBlockType)
        {
            case AnchorBlock.Type.GO_TO:
                break;
            case AnchorBlock.Type.MOVE_TO:
                break;
            case AnchorBlock.Type.ROTATE:
                break;
            case AnchorBlock.Type.SET_ANGULAR_SPEED:
                break;
            case AnchorBlock.Type.SET_SPEED:
                break;
            case AnchorBlock.Type.TWEEN:
                break;
            case AnchorBlock.Type.WAIT:
                break;
            default:
                throw new(
                    $"Cannot create a {anchorBlock.ImplementedBlockType} type anchor block, because there wasn't one found");
        }
    }
}