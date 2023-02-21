using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorConnectorController : MonoBehaviour
{
    private MouseOverUI mouseOverUI;

    private void Awake()
    {
        mouseOverUI = GetComponent<MouseOverUI>();
    }

    private void Update()
    {
        if (AnchorBlockManager.DraggingBlock && mouseOverUI.Over &&
            (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)))
        {
            // move dragged block to this string
            Transform draggedBlock = AnchorBlockManager.DraggedBlock.gameObject.transform;
            Transform parent = transform.parent;
            draggedBlock.SetParent(parent.parent);
            draggedBlock.SetSiblingIndex(transform.GetSiblingIndex() + 1);
            Instantiate(PrefabManager.Instance.AnchorConnector, parent);
        }
    }
}
