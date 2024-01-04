using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SelectionSquare : MonoBehaviour
{
    [FormerlySerializedAs("SelectedSprite")] [SerializeField] private Sprite selectedSprite;
    [FormerlySerializedAs("DeselectedSprite")] [SerializeField] private Sprite deselectedSprite;
    [FormerlySerializedAs("SubSelectedSprite")] [SerializeField] private Sprite subSelectedSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetSelected(bool selected) => image.sprite = selected ? selectedSprite : deselectedSprite;

    public void SetSubSelected(bool subSelected) => image.sprite = subSelected ? subSelectedSprite : image.sprite;
}