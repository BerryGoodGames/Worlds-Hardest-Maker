using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
///     Controls placement, visibility and display of preview
///     <para>Attach to gameObject PlacementPreview</para>
/// </summary>
public class PreviewController : MonoBehaviour
{
    private EditMode previousEditMode;

    private bool previousPlaying;

    [FormerlySerializedAs("spriteRenderer")] [HideInInspector]
    public SpriteRenderer SpriteRenderer;

    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Color defaultColor;

    [FormerlySerializedAs("alpha")] [Range(0, 255)]
    public int Alpha;

    [FormerlySerializedAs("changeSpriteToCurrentEditMode")]
    public bool ChangeSpriteToCurrentEditMode = true;

    [FormerlySerializedAs("updateEveryFrame")]
    public bool UpdateEveryFrame = true;

    [FormerlySerializedAs("showSpriteWhenPasting")]
    public bool ShowSpriteWhenPasting;

    [FormerlySerializedAs("rotateToRotation")]
    public bool RotateToRotation = true;

    [SerializeField] private bool smoothRotation;
    [SerializeField] private float rotateDuration;

    private FollowMouse followMouseComp;
    private bool hasFollowMouseComp;

    private bool ranAwake;
    private static readonly int visible = Animator.StringToHash("Visible");

    private void Awake()
    {
        Awake_();
    }

    public void Awake_()
    {
        if (ranAwake) return;

        SpriteRenderer = GetComponent<SpriteRenderer>();

        SpriteRenderer.sprite = defaultSprite;
        SpriteRenderer.color = defaultColor;
        transform.localScale = new(1, 1);

        ranAwake = true;
    }

    private void Start()
    {
        followMouseComp = GetComponent<FollowMouse>();
        hasFollowMouseComp = followMouseComp != null;

        previousEditMode = EditModeManager.Instance.CurrentEditMode;
    }

    private void Update()
    {
        EditMode currentEditMode = EditModeManager.Instance.CurrentEditMode;

        if (UpdateEveryFrame &&
            (previousEditMode != currentEditMode || previousPlaying != EditModeManager.Instance.Playing))
            UpdateSprite();

        if (!SelectionManager.Instance.Selecting && hasFollowMouseComp)
            followMouseComp.WorldPosition = currentEditMode.IsFieldType() || currentEditMode == EditMode.DELETE_FIELD
                ? FollowMouse.WorldPositionType.MATRIX
                : FollowMouse.WorldPositionType.GRID;

        // check visibility of preview
        if (TryGetComponent(out Animator anim)) anim.SetBool(visible, CheckVisibility());
    }

    /// <summary>
    ///     Checks if preview should currently be visible with current edit mode
    /// </summary>
    /// <returns></returns>
    private bool CheckVisibility() => CheckVisibility(EditModeManager.Instance.CurrentEditMode);

    /// <summary>
    ///     Checks if preview should currently be visible at the moment
    /// </summary>
    /// <param name="mode">edit mode which needs to be checked</param>
    private bool CheckVisibility(EditMode mode)
    {
        if (MouseManager.Instance.IsUIHovered ||
            Input.GetKey(KeybindManager.Instance.EntityMoveKey) ||
            Input.GetKey(KeybindManager.Instance.EditSpeedKey) ||
            Input.GetKey(KeybindManager.Instance.EntityDeleteKey)) return false;

        if (CopyManager.Pasting) return false;

        // check if preview of prefab not allowed during filling
        if (SelectionManager.Instance.Selecting)
            if (SelectionManager.NoFillPreviewModes.Contains(mode))
                return false;

        FollowMouse.WorldPositionType positionMode = GetComponent<FollowMouse>().WorldPosition;

        Vector2 mousePos = positionMode switch
        {
            FollowMouse.WorldPositionType.ANY => MouseManager.Instance.MouseWorldPos,
            FollowMouse.WorldPositionType.GRID => MouseManager.Instance.MouseWorldPosGrid,
            _ => MouseManager.Instance.MouseWorldPosMatrix
        };

        // check coin placement
        if (mode != EditMode.COIN)
            return !KeyManager.KeyModes.Contains(mode) || KeyManager.CanPlace(mousePos.x, mousePos.y);

        if (!CoinManager.CanPlace(mousePos.x, mousePos.y)) return false;

        // check key placement + return
        return !KeyManager.KeyModes.Contains(mode) || KeyManager.CanPlace(mousePos.x, mousePos.y);
    }

    /// <summary>
    ///     Updates sprite to sprite of preview to the current edit mode
    /// </summary>
    public void UpdateSprite()
    {
        SetSprite(EditModeManager.Instance.CurrentEditMode);

        previousPlaying = EditModeManager.Instance.Playing;
        previousEditMode = EditModeManager.Instance.CurrentEditMode;
    }

    public void SetSprite(EditMode editMode, bool updateRotation = false)
    {
        if (editMode == EditMode.DELETE_FIELD)
        {
            // defaultSprite for preview when deleting
            SpriteRenderer.sprite = defaultSprite;
            SpriteRenderer.color = defaultColor;
            transform.localScale = new(1, 1);
            return;
        }

        GameObject currentPrefab = editMode.GetPrefab();
        if (currentPrefab.TryGetComponent(out PreviewSprite previewSprite) &&
            ((!SelectionManager.Instance.Selecting && !CopyManager.Pasting) || previewSprite.ShowWhenSelecting ||
             ShowSpriteWhenPasting))
        {
            // apply PreviewSprite settings if it has one
            SpriteRenderer.sprite = previewSprite.Sprite;
            SpriteRenderer.color = new(previewSprite.Color.r, previewSprite.Color.g, previewSprite.Color.b,
                Alpha / 255f);
            transform.localScale = previewSprite.Scale;

            UpdateRotation(!previewSprite.Rotate);
            return;
        }

        // display sprite and apply scale of prefab if no PreviewSprite setting
        Vector2 scale = new();

        // get sprite and scale
        if (currentPrefab.TryGetComponent(out SpriteRenderer prefabRenderer))
            scale = currentPrefab.transform.localScale;
        else
            foreach (Transform child in currentPrefab.transform)
            {
                if (!child.TryGetComponent(out prefabRenderer)) continue;

                scale = child.localScale;
                break;
            }

        // apply
        Color prefabColor = prefabRenderer.color;

        SpriteRenderer.sprite = prefabRenderer.sprite;
        SpriteRenderer.color = new(prefabColor.r, prefabColor.g, prefabColor.b, Alpha / 255f);
        transform.localScale = scale;
        if (updateRotation)
            UpdateRotation(true);

        // for filling preview go to FillManager.cs
    }

    public void UpdateRotation(bool resetRotation = false, bool smooth = true)
    {
        if (resetRotation)
        {
            transform.localRotation = Quaternion.identity;
            return;
        }

        if (!RotateToRotation) return;

        Quaternion rotation = Quaternion.Euler(0, 0, EditModeManager.Instance.EditRotation);
        if (smoothRotation && smooth)
        {
            transform.DOKill();
            transform.DORotateQuaternion(rotation, rotateDuration)
                .SetEase(Ease.OutCubic);
        }
        else
        {
            transform.localRotation = rotation;
        }
    }
}