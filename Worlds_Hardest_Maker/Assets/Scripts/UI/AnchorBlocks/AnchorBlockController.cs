using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorBlockController : MonoBehaviour
{
    public bool IsInsertable = true;
    [HideInInspector] public BlockDragDrop BlockDragDropComp;

    private void Awake()
    {
        BlockDragDropComp = GetComponent<BlockDragDrop>();
    }
}
