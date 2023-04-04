using System;
using UnityEditor;
using UnityEngine;

public abstract class AnchorBlockController : MonoBehaviour
{
    public bool IsInsertable = true;

    [SerializeField] private Vector2 duplicateOffset = new(-10, 10);

    [HideInInspector] public BlockDragDrop BlockDragDropComp;

    public abstract AnchorBlock GetAnchorBlock(AnchorController anchorController);

    private void Awake()
    {
        BlockDragDropComp = GetComponent<BlockDragDrop>();
    }

    private void Start()
    {
        if (!IsInsertable && TryGetComponent(out BlockDragDrop blockDragDrop))
            Destroy(blockDragDrop);
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    public void Duplicate()
    {
        Instantiate(gameObject, transform.position + (Vector3)duplicateOffset, Quaternion.identity, ReferenceManager.Instance.AnchorBlockStringContainer);
    }
}