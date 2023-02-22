using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnchorBlockController : MonoBehaviour
{
    public bool IsInsertable = true;
    [HideInInspector] public BlockDragDrop BlockDragDropComp;

    public abstract AnchorBlock GetAnchorBlock(AnchorController anchorController);

    private void Awake()
    {
        BlockDragDropComp = GetComponent<BlockDragDrop>();
    }
}
