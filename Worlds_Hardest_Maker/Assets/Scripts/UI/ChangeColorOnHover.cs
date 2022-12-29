using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeColorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Color defaultColor;
    private Image image;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hoverColor;

    private void Start()
    {
        // try to get image/sprite renderer component
        TryGetComponent(out image);
        TryGetComponent(out spriteRenderer);

        // get default color
        if (image != null)
            defaultColor = image.color;

        else if (spriteRenderer != null)
            defaultColor = spriteRenderer.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // try to color the image/sprite to the hover color
        if (image != null)
            image.color = hoverColor;
        else if (spriteRenderer != null)
            spriteRenderer.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // try to color the image/sprite to the default color
        if (image != null)
            image.color = defaultColor;
        else if (spriteRenderer != null)
            spriteRenderer.color = defaultColor;
    }
}