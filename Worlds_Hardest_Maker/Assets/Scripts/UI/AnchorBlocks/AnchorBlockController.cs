using UnityEngine;
using UnityEngine.Serialization;

public abstract class AnchorBlockController : MonoBehaviour
{
    [FormerlySerializedAs("IsInsertable")] public bool Movable = true;

    [SerializeField] private readonly Vector2 duplicateOffset = new(-10, 10);

    [HideInInspector] public BlockDragDrop BlockDragDropComp;

    public abstract AnchorBlock GetAnchorBlock(AnchorController anchorController);

    private void Awake()
    {
        BlockDragDropComp = GetComponent<BlockDragDrop>();
    }

    private void Start()
    {
        if (!Movable && TryGetComponent(out BlockDragDrop blockDragDrop))
            Destroy(blockDragDrop);
        if (!Movable && TryGetComponent(out AnchorBlockQuickMenu quickMenu)) quickMenu.Active = false;
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    public void Duplicate()
    {
        Instantiate(gameObject, transform.position + (Vector3)duplicateOffset, Quaternion.identity,
            ReferenceManager.Instance.AnchorBlockStringContainer);
    }
}