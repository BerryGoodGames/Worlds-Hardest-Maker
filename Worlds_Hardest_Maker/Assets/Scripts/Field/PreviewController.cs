using DG.Tweening;
using UnityEngine;

/// <summary>
///     controls placement, visibility and display of preview
///     attach to gameObject PlacementPreview
/// </summary>
public class PreviewController : MonoBehaviour
{
    private EditMode previousEditMode;

    private bool previousPlaying;

    [HideInInspector] public SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Color defaultColor;
    [Range(0, 255)] public int alpha;
    public bool changeSpriteToCurrentEditMode = true;
    public bool updateEveryFrame = true;
    public bool followMouse;
    public bool showSpriteWhenPasting;
    public bool rotateToRotation = true;
    [SerializeField] private bool smoothRotation;
    [SerializeField] private float rotateDuration;

    private FollowMouse followMouseComp;

    private bool ranAwake;
    private static readonly int visible = Animator.StringToHash("Visible");

    private void Awake()
    {
        Awake_();
    }

    public void Awake_()
    {
        if (ranAwake) return;

        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.color = defaultColor;
        transform.localScale = new(1, 1);

        ranAwake = true;
    }

    private void Start()
    {
        followMouseComp = GetComponent<FollowMouse>();

        previousEditMode = EditModeManager.Instance.CurrentEditMode;
    }

    private void Update()
    {
        EditMode currentEditMode = EditModeManager.Instance.CurrentEditMode;

        if (updateEveryFrame &&
            (previousEditMode != currentEditMode || previousPlaying != EditModeManager.Instance.Playing)) UpdateSprite();

        if (!SelectionManager.Instance.Selecting && followMouse)
        {
            followMouseComp.worldPosition = currentEditMode.IsFieldType() || currentEditMode == EditMode.DELETE_FIELD
                ? FollowMouse.WorldPosition.MATRIX
                : FollowMouse.WorldPosition.GRID;
        }

        // check visibility of preview
        if (TryGetComponent(out Animator anim))
        {
            anim.SetBool(visible, CheckVisibility());
        }
    }

    /// <summary>
    ///     check if preview should be visible at the moment with current edit mode
    /// </summary>
    /// <returns></returns>
    private bool CheckVisibility()
    {
        return CheckVisibility(EditModeManager.Instance.CurrentEditMode);
    }

    /// <summary>
    ///     check if preview should be visible at the moment
    /// </summary>
    /// <param name="mode">edit mode which needs to be checked</param>
    /// <returns></returns>
    private bool CheckVisibility(EditMode mode)
    {
        if (UIManager.Instance.UIHovered ||
            Input.GetKey(KeybindManager.Instance.entityMoveKey) ||
            Input.GetKey(KeybindManager.Instance.editSpeedKey) ||
            Input.GetKey(KeybindManager.Instance.entityDeleteKey)) return false;

        if (CopyManager.pasting) return false;

        // check if preview of prefab not allowed during filling
        if (SelectionManager.Instance.Selecting)
        {
            if (SelectionManager.noFillPreviewModes.Contains(mode)) return false;
        }

        FollowMouse.WorldPosition positionMode = GetComponent<FollowMouse>().worldPosition;

        Vector2 mousePos = positionMode switch
        {
            FollowMouse.WorldPosition.ANY => MouseManager.Instance.MouseWorldPos,
            FollowMouse.WorldPosition.GRID => MouseManager.Instance.MouseWorldPosGrid,
            _ => MouseManager.Instance.MouseWorldPosMatrix
        };

        // check coin placement
        if (mode != EditMode.COIN)
            return !KeyManager.keyModes.Contains(mode) || KeyManager.CanPlace(mousePos.x, mousePos.y);

        if (!CoinManager.CanPlace(mousePos.x, mousePos.y)) return false;

        // check key placement + return
        return !KeyManager.keyModes.Contains(mode) || KeyManager.CanPlace(mousePos.x, mousePos.y);
    }

    /// <summary>
    ///     updates sprite to the sprite of preview to the current edit mode
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
            spriteRenderer.sprite = defaultSprite;
            spriteRenderer.color = defaultColor;
            transform.localScale = new(1, 1);
        }
        else
        {
            GameObject currentPrefab = editMode.GetPrefab();
            if (currentPrefab.TryGetComponent(out PreviewSprite previewSprite) &&
                ((!SelectionManager.Instance.Selecting && !CopyManager.pasting) || previewSprite.showWhenSelecting ||
                 showSpriteWhenPasting))
            {
                // apply PreviewSprite settings if it has one
                spriteRenderer.sprite = previewSprite.sprite;
                spriteRenderer.color = new(previewSprite.color.r, previewSprite.color.g, previewSprite.color.b,
                    alpha / 255f);
                transform.localScale = previewSprite.scale;

                UpdateRotation(!previewSprite.rotate);
            }
            else
            {
                // display sprite and apply scale of prefab if no PreviewSprite setting
                Vector2 scale = new();

                // get sprite and scale
                if (currentPrefab.TryGetComponent(out SpriteRenderer prefabRenderer))
                {
                    scale = currentPrefab.transform.localScale;
                }
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

                spriteRenderer.sprite = prefabRenderer.sprite;
                spriteRenderer.color = new(prefabColor.r, prefabColor.g, prefabColor.b, alpha / 255f);
                transform.localScale = scale;
                if (updateRotation)
                    UpdateRotation(true);
            }
        }

        // for filling preview go to FillManager.cs
    }

    public void UpdateRotation(bool resetRotation = false, bool smooth = true)
    {
        if (resetRotation)
        {
            transform.localRotation = Quaternion.identity;
            return;
        }

        if (!rotateToRotation) return;

        Quaternion rotation = Quaternion.Euler(0, 0, EditModeManager.Instance.EditRotation);
        if (smoothRotation && smooth)
        {
            transform.DOKill();
            transform.DORotateQuaternion(rotation, rotateDuration)
                .SetEase(Ease.OutCubic);
        }
        else transform.localRotation = rotation;
    }
}