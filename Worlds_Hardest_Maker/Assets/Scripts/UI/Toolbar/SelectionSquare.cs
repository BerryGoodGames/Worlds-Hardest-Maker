using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SelectionSquare : MonoBehaviour
{
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Sprite deselectedSprite;
    [SerializeField] private Sprite subSelectedSprite;

    private Image image;

    private void Awake() => image = GetComponent<Image>();

    public void SetSelected(bool selected) => image.sprite = selected ? selectedSprite : deselectedSprite;

    public void SetSubSelected(bool subSelected) => image.sprite = subSelected ? subSelectedSprite : image.sprite;
}