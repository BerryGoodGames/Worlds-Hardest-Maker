using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PreviewController : MonoBehaviour
{
    private GameManager.EditMode previousEditMode;

    private bool previousPlaying;

    private SpriteRenderer spriteRenderer;
    public Sprite defaultSprite;
    public Color defaultColor;
    [Range(0, 255)] public int alpha;

    public bool updateEveryFrame = true;

    private void Start()
    {
        previousEditMode = GameManager.Instance.CurrentEditMode;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.color = defaultColor;
        transform.localScale = new(1, 1);
        UpdateSprite();
    }

    private void Update()
    {
        if (updateEveryFrame && (previousEditMode != GameManager.Instance.CurrentEditMode || previousPlaying != GameManager.Instance.Playing)) UpdateSprite();

        // check visibility of preview
        if (TryGetComponent(out Animator anim))
        {
            anim.SetBool("Visible", CheckVisibility());
        }
    }

    private bool CheckVisibility()
    {
        if (GameManager.Instance.UIHovered || 
            Input.GetKey(GameManager.Instance.BallDragKey) ||
            Input.GetKey(GameManager.Instance.EditSpeedKey) ||
            Input.GetKey(GameManager.Instance.EntityDeleteKey)) return false;

        GameObject hoveredField = FieldManager.GetField(GameManager.Instance.MousePosWorldSpaceRounded);
        FieldManager.FieldType? hoveredType = FieldManager.GetFieldType(hoveredField);

        // check player placement
        if (GameManager.Instance.CurrentEditMode == GameManager.EditMode.PLAYER)
        {
            if (hoveredField == null || !PlayerManager.StartFields.Contains((FieldManager.FieldType)hoveredType)) return false;
        }

        // check coin placement
        if(GameManager.Instance.CurrentEditMode == GameManager.EditMode.COIN)
        {
            if (hoveredField != null && CoinManager.CantPlaceFields.Contains((FieldManager.FieldType)hoveredType)) return false;
        }

        // check key placement
        if (KeyManager.KeyModes.Contains(GameManager.Instance.CurrentEditMode))
        {
            if (hoveredField != null && KeyManager.CantPlaceFields.Contains((FieldManager.FieldType)hoveredType)) return false;
        }
        return true;
    }

    /// <summary>
    /// updates sprite to the sprite of the current edit mode
    /// </summary>
    public void UpdateSprite()
    {
        if (GameManager.Instance.CurrentEditMode == GameManager.EditMode.DELETE_FIELD)
        {
            spriteRenderer.sprite = defaultSprite;
            spriteRenderer.color = defaultColor;
            transform.localScale = new(1, 1);
        }
        else
        {
            GameObject currentPrefab = GameManager.GetPrefabs()[GameManager.Instance.CurrentEditMode];
            if (currentPrefab.TryGetComponent(out PreviewSprite previewSprite) && (!GameManager.Instance.Filling || previewSprite.showWhenFilling))
            {
                spriteRenderer.sprite = previewSprite.sprite;
                spriteRenderer.color = previewSprite.color;
                spriteRenderer.color = new(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha / 255f);
                transform.localScale = previewSprite.scale;
            }
            else
            {
                Vector2 scale = new();
                if (currentPrefab.TryGetComponent(out SpriteRenderer prefabRenderer))
                {
                    prefabRenderer = currentPrefab.GetComponent<SpriteRenderer>();
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
                //if (currentPrefab.transform.childCount != 0)
                //{
                //    Transform prefabChild = currentPrefab.transform.GetChild(0);
                //    prefabRenderer = prefabChild.GetComponent<SpriteRenderer>();
                //    scale = prefabChild.localScale;
                //}
                //else
                //{
                //    prefabRenderer = currentPrefab.GetComponent<SpriteRenderer>();
                //    scale = currentPrefab.transform.localScale;
                //}
                spriteRenderer.sprite = prefabRenderer.sprite;
                spriteRenderer.color = prefabRenderer.color;
                spriteRenderer.color = new(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha / 255);
                transform.localScale = scale;
            }
        }
        previousPlaying = GameManager.Instance.Playing;
        previousEditMode = GameManager.Instance.CurrentEditMode;
    }
}
