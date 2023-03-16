using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(MouseOverUI))]
public class AnchorConnectorController : MonoBehaviour
{
    private MouseOverUI mouseOverUI;
    [FormerlySerializedAs("dummy")] public bool Dummy;


    private void Awake()
    {
        mouseOverUI = GetComponent<MouseOverUI>();
    }

    private void Update()
    {
        if (Dummy) return;

        // check if a block got released over this connector
        if (!AnchorBlockManager.DraggingBlock || !mouseOverUI.Over ||
            (!Input.GetMouseButtonUp(0) && !Input.GetMouseButtonUp(1))) return;

        // move dragged block to this string
        Transform draggedBlock = AnchorBlockManager.DraggedBlock.gameObject.transform;
        Transform parent = transform.parent;
        draggedBlock.SetParent(parent.parent);
        draggedBlock.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parent.parent);
        StartCoroutine(ExecuteOnSnap(AnchorBlockManager.DraggedBlock.BlockDragDropComp));

        // generate new connector if needed
        if (parent.parent.childCount - 1 != parent.childCount) return;

        Instantiate(PrefabManager.Instance.AnchorConnector, parent);
        if (!AnchorBlockManager.DraggedBlock.IsInsertable)
        {
            Dummy = true;
        }
    }

    private static IEnumerator ExecuteOnSnap(BlockDragDrop blockDragDrop)
    {
        yield return new WaitForEndOfFrame();
        blockDragDrop.OnSnap();
    }
}