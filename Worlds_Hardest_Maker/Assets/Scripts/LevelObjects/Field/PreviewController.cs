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

    private FollowMouse followMouseComp;
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

    private void Start()
    {
        hasFollowMouseComp = TryGetComponent(out followMouseComp);

        previousEditMode = EditModeManager.Instance.CurrentEditMode;
    }

    private void Update()
    {
        EditMode currentEditMode = EditModeManager.Instance.CurrentEditMode;

        // update sprite if necessary
        bool hasEditModeChanged = previousEditMode != currentEditMode;
        bool hasPlayingChanged = previousPlaying != EditModeManager.Instance.Playing;
        if (CheckUpdateEveryFrame && (hasEditModeChanged || hasPlayingChanged)) UpdateSprite();


        if (!SelectionManager.Instance.Selecting && hasFollowMouseComp)
        {
            followMouseComp.WorldPosition = currentEditMode.IsFieldType() || currentEditMode == EditMode.Delete
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
    private bool CheckVisibility() => CheckVisibility(EditModeManager.Instance.CurrentEditMode);

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
            || AnchorBlockManager.Instance.DraggingBlock) return false;

        if (CopyManager.Instance.Pasting) return false;
        if (AnchorPositionInputEditManager.Instance.IsEditing) return false;

        // check if preview of prefab not allowed during filling
        if (SelectionManager.Instance.Selecting)
        {
            if (SelectionManager.NoFillPreviewModes.Contains(mode)) return false;
        }

        WorldPositionType positionMode = GetComponent<FollowMouse>().WorldPosition;

        Vector2 mousePos = positionMode switch
        {
            WorldPositionType.Any => MouseManager.Instance.MouseWorldPos,
            WorldPositionType.Grid => MouseManager.Instance.MouseWorldPosGrid,
            _ => MouseManager.Instance.MouseWorldPosMatrix,
        };

        // check coin placement
        if (mode != EditMode.Coin) return !KeyManager.KeyModes.Contains(mode) || KeyManager.CanPlace(mousePos);

        if (!CoinManager.CanPlace(mousePos)) return false;

        // check key placement + return
        return !KeyManager.KeyModes.Contains(mode) || KeyManager.CanPlace(mousePos);
    }

    /// <summary>
    ///     Updates sprite of preview to the current edit mode
    /// </summary>
    public void UpdateSprite()
    {
        SetSprite(EditModeManager.Instance.CurrentEditMode);

        previousPlaying = EditModeManager.Instance.Playing;
        previousEditMode = EditModeManager.Instance.CurrentEditMode;
    }

    public void SetSprite(EditMode editMode, bool updateRotation = false)
    {
        if (editMode == EditMode.Delete)
        {
            // defaultSprite for preview when deleting
            ApplyDefaultSprite();
            return;
        }

        GameObject currentPrefab = editMode.GetPrefab();
        bool hasCurrentPrefabPreviewController = currentPrefab.TryGetComponent(out PreviewSprite previewSprite);
        if (hasCurrentPrefabPreviewController &&
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

        Quaternion rotation = Quaternion.Euler(0, 0, EditModeManager.Instance.EditRotation);
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