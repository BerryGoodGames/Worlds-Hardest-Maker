using DG.Tweening;
using MyBox;
using UnityEngine;

/// <summary>
///     Controls placement, visibility and display of preview
///     <para>Attach to gameObject PlacementPreview and prefab FillPreview</para>
/// </summary>
public class PreviewController : MonoBehaviour
{
    private EditMode previousEditMode;
    private bool previousPlaying;

    [HideInInspector] public SpriteRenderer SpriteRenderer;

    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Color defaultColor;

    [Space] [SerializeField] [Range(0, 255)] private float alpha;

    [ReadOnly] public bool CheckUpdateEveryFrame = true;

    public bool ShowSpriteWhenPasting;

    public bool RotateToEditRotation = true;

    [Space] [SerializeField] private bool smoothRotation;
    [SerializeField] private float rotateDuration;

    protected FollowMouse FollowMouseComp;
    private bool hasFollowMouseComp;

    private bool ranAwake;
    private static readonly int visible = Animator.StringToHash("Visible");

    private void Awake() => Awake_();

    public void Awake_()
    {
        if (ranAwake) return;

        SpriteRenderer = GetComponent<SpriteRenderer>();

        SpriteRenderer.sprite = defaultSprite;
        SpriteRenderer.color = defaultColor;
        transform.localScale = new(1, 1);

        ranAwake = true;
    }

    protected virtual void Start()
    {
        hasFollowMouseComp = TryGetComponent(out FollowMouseComp);

        previousEditMode = EditModeManagerOther.Instance.CurrentEditMode;
    }

    private void Update()
    {
        EditMode currentEditMode = EditModeManagerOther.Instance.CurrentEditMode;

        // update sprite if necessary
        bool hasEditModeChanged = previousEditMode != currentEditMode;
        bool hasPlayingChanged = previousPlaying != EditModeManagerOther.Instance.Playing;
        if (CheckUpdateEveryFrame && (hasEditModeChanged || hasPlayingChanged)) UpdateSprite();


        if (!SelectionManager.Instance.Selecting && hasFollowMouseComp)
        {
            FollowMouseComp.WorldPosition = currentEditMode.Attributes.IsField || currentEditMode == EditModeManager.Delete
                ? WorldPositionType.Matrix
                : WorldPositionType.Grid;
        }

        // check visibility of preview
        if (TryGetComponent(out Animator anim)) anim.SetBool(visible, CheckVisibility());
    }

    /// <summary>
    ///     Checks if preview should currently be visible with current edit mode
    /// </summary>
    /// <returns></returns>
    private bool CheckVisibility() => CheckVisibility(EditModeManagerOther.Instance.CurrentEditMode);

    /// <summary>
    ///     Checks if preview should currently be visible at the moment
    /// </summary>
    /// <param name="mode">edit mode which needs to be checked</param>
    private bool CheckVisibility(EditMode mode)
    {
        if (MouseManager.Instance.IsUIHovered
            || KeyBinds.GetKeyBind("Editor_MoveEntity")
            || KeyBinds.GetKeyBind("Editor_Modify")
            || KeyBinds.GetKeyBind("Editor_DeleteEntity")
            || AnchorBlockManager.Instance.DraggingBlock
            || CopyManager.Instance.Pasting
            || AnchorPositionInputEditManager.Instance.IsEditing) return false;

        // check if preview of edit mode is not allowed during filling
        if (SelectionManager.Instance.Selecting)
        {
            if (!mode.ShowFillPreview) return false;
        }

        Vector2 mousePos = FollowMouseComp.WorldPosition switch
        {
            WorldPositionType.Any => MouseManager.Instance.MouseWorldPos,
            WorldPositionType.Grid => MouseManager.Instance.MouseWorldPosGrid,
            _ => MouseManager.Instance.MouseWorldPosMatrix,
        };

        // check coin placement
        if (mode == EditModeManager.Coin) return CoinManager.CanPlace(mousePos);

        // check key placement
        if (mode.Attributes.IsKey) return KeyManager.CanPlace(mousePos);

        return true;
    }

    /// <summary>
    ///     Updates sprite of preview to the current edit mode
    /// </summary>
    public void UpdateSprite()
    {
        SetSprite(EditModeManagerOther.Instance.CurrentEditMode);

        previousPlaying = EditModeManagerOther.Instance.Playing;
        previousEditMode = EditModeManagerOther.Instance.CurrentEditMode;
    }

    public void SetSprite(EditMode editMode, bool updateRotation = false)
    {
        if (editMode == EditModeManager.Delete)
        {
            // defaultSprite for preview when deleting
            ApplyDefaultSprite();
            return;
        }

        GameObject currentPrefab = editMode.Prefab;
        bool hasCurrentPrefabPreviewSpriteController = currentPrefab.TryGetComponent(out PreviewSprite previewSprite);
        if (hasCurrentPrefabPreviewSpriteController &&
            ((!SelectionManager.Instance.Selecting && !CopyManager.Instance.Pasting) || ShowSpriteWhenPasting))
        {
            // apply PreviewSprite settings if it has one
            SpriteRenderer.sprite = previewSprite.Sprite;
            SpriteRenderer.color = new(
                previewSprite.Color.r, previewSprite.Color.g, previewSprite.Color.b,
                alpha / 255f
            );

            transform.localScale = previewSprite.Scale;

            UpdateRotation(!previewSprite.Rotate);
            return;
        }

        // display sprite and apply scale of prefab if no PreviewSprite setting
        Vector2 scale = new();

        // get sprite and scale
        if (currentPrefab.TryGetComponent(out SpriteRenderer prefabRenderer)) scale = currentPrefab.transform.localScale;
        else
        {
            foreach (Transform child in currentPrefab.transform)
            {
                if (!child.TryGetComponent(out prefabRenderer)) continue;

                scale = child.localScale;
                break;
            }
        }

        // apply
        Color prefabColor = prefabRenderer.color;

        SpriteRenderer.sprite = prefabRenderer.sprite;
        SpriteRenderer.color = new(prefabColor.r, prefabColor.g, prefabColor.b, alpha / 255f);
        transform.localScale = scale;
        if (updateRotation) UpdateRotation(true);

        // for filling preview go to FillManager.cs
    }

    public void UpdateRotation(bool resetRotation = false, bool smooth = true)
    {
        if (resetRotation)
        {
            transform.localRotation = Quaternion.identity;
            return;
        }

        if (!RotateToEditRotation) return;

        Quaternion rotation = Quaternion.Euler(0, 0, EditModeManagerOther.Instance.EditRotation);
        if (smoothRotation && smooth)
        {
            transform.DOKill();
            transform.DORotateQuaternion(rotation, rotateDuration)
                .SetEase(Ease.OutCubic);
        }
        else transform.localRotation = rotation;
    }

    private void ApplyDefaultSprite()
    {
        SpriteRenderer.sprite = defaultSprite;
        SpriteRenderer.color = defaultColor;
        transform.localScale = new(1, 1);
    }
}