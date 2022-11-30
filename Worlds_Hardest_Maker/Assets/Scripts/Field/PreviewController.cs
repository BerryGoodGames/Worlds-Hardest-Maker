using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// controls placement, visibility and display of preview
/// attach to gameobject PlacementPreview
/// </summary>
public class PreviewController : MonoBehaviour
{
    private GameManager.EditMode previousEditMode;

    private bool previousPlaying;

    [HideInInspector] public SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Color defaultColor;
    [Range(0, 255)] public int alpha;
    public bool changeSpriteToCurrentEditMode = true;
    public bool updateEveryFrame = true;
    public bool followMouse;
    public bool showSpriteWhenPasting = false;
    public bool rotateToRotation = true;
    [SerializeField] private bool smoothRotation;
    [SerializeField] private float rotateDuration;

    private bool ranAwake = false;

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
        previousEditMode = GameManager.Instance.CurrentEditMode;
    }

    private void Update()
    {

        GameManager.EditMode currentEditMode = GameManager.Instance.CurrentEditMode;
        if (updateEveryFrame && (previousEditMode != currentEditMode || previousPlaying != GameManager.Instance.Playing)) UpdateSprite();

        if (!GameManager.Instance.Selecting && followMouse)
        {
            FollowMouse followMouse = GetComponent<FollowMouse>();
            followMouse.worldPosition = currentEditMode.IsFieldType() || currentEditMode == GameManager.EditMode.DELETE_FIELD ?
                FollowMouse.WorldPosition.MATRIX : FollowMouse.WorldPosition.GRID;
        }

        // check visibility of preview
        if (TryGetComponent(out Animator anim))
        {
            anim.SetBool("Visible", CheckVisibility());
        }
    }

    /// <summary>
    /// check if preview should be visible at the moment with current edit mode
    /// </summary>
    /// <returns></returns>
    private bool CheckVisibility()
    {
        return CheckVisibility(GameManager.Instance.CurrentEditMode);
    }
    /// <summary>
    /// check if preview should be visible at the moment
    /// </summary>
    /// <param name="mode">edit mode which needs to be checked</param>
    /// <returns></returns>
    private bool CheckVisibility(GameManager.EditMode mode)
    {
        if (GameManager.Instance.UIHovered ||
            Input.GetKey(GameManager.Instance.EntityMoveKey) ||
            Input.GetKey(GameManager.Instance.EditSpeedKey) ||
            Input.GetKey(GameManager.Instance.EntityDeleteKey)) return false;

        if (CopyManager.pasting) return false;

        // check if preview of prefab not allowed during filling
        if (GameManager.Instance.Selecting)
        {
            if (SelectionManager.NoFillPreviewModes.Contains(mode)) return false;
        }

        FollowMouse.WorldPosition positionMode = GetComponent<FollowMouse>().worldPosition;

        Vector2 mousePos = positionMode == FollowMouse.WorldPosition.ANY ? MouseManager.Instance.MouseWorldPos :
            positionMode == FollowMouse.WorldPosition.GRID ? MouseManager.Instance.MouseWorldPosGrid : MouseManager.Instance.MouseWorldPosMatrix;

        // check player placement
        if (mode == GameManager.EditMode.PLAYER)
        {
            if (!PlayerManager.CanPlace(mousePos.x, mousePos.y)) return false;
        }

        // check coin placement
        if (mode == GameManager.EditMode.COIN)
        {
            if (!CoinManager.CanPlace(mousePos.x, mousePos.y)) return false;
        }

        // check key placement
        if (KeyManager.KeyModes.Contains(mode))
        {
            if (!KeyManager.CanPlace(mousePos.x, mousePos.y)) return false;
        }
        return true;
    }

    /// <summary>
    /// updates sprite to the sprite of preview to the current edit mode
    /// </summary>
    public void UpdateSprite()
    {
        SetSprite(GameManager.Instance.CurrentEditMode);

        previousPlaying = GameManager.Instance.Playing;
        previousEditMode = GameManager.Instance.CurrentEditMode;
    }

    public void SetSprite(GameManager.EditMode editMode, bool updateRotation = false)
    {
        if (editMode == GameManager.EditMode.DELETE_FIELD)
        {
            // defaultSprite for preview when deleting
            spriteRenderer.sprite = defaultSprite;
            spriteRenderer.color = defaultColor;
            transform.localScale = new(1, 1);
        }
        else
        {
            GameObject currentPrefab = editMode.GetPrefab();
            if (currentPrefab.TryGetComponent(out PreviewSprite previewSprite) && (!GameManager.Instance.Selecting && !CopyManager.pasting || previewSprite.showWhenSelecting || showSpriteWhenPasting))
            {
                // apply PreviewSprite settings if it has one
                spriteRenderer.sprite = previewSprite.sprite;
                spriteRenderer.color = new(previewSprite.color.r, previewSprite.color.g, previewSprite.color.b, alpha / 255f);
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
                        if (child.TryGetComponent(out prefabRenderer))
                        {
                            scale = child.localScale;
                            break;
                        }
                    }
                }

                // apply
                spriteRenderer.sprite = prefabRenderer.sprite;
                spriteRenderer.color = new(prefabRenderer.color.r, prefabRenderer.color.g, prefabRenderer.color.b, alpha / 255f);
                transform.localScale = scale;
                if(updateRotation)
                    UpdateRotation(true);
            }
        }

        // for filling preview go to FillManager.cs


    }

    public void UpdateRotation(bool resetRotation = false, bool smooth = true)
    {
        if (!resetRotation)
        {
            if (!rotateToRotation) return;

            Quaternion rotation = Quaternion.Euler(0, 0, GameManager.Instance.EditRotation);
            if (smoothRotation && smooth)
            {
                transform.DOKill();
                transform.DORotateQuaternion(rotation, rotateDuration)
                    .SetEase(Ease.OutCubic);
            }
            else transform.localRotation = rotation;
        }
        else transform.localRotation = Quaternion.identity;
    }
}
