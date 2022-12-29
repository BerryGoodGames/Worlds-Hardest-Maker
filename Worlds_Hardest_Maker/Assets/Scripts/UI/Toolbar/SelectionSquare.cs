using UnityEngine;
using UnityEngine.UI;

public class SelectionSquare : MonoBehaviour
{
    private Image image;
    public Sprite deselectedSprite;
    public Sprite selectedSprite;
    public Sprite subSelectedSprite;
    private RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        if (transform.parent.transform.parent != null && transform.parent.parent.CompareTag("OptionContainer"))
        {
            rt.sizeDelta = transform.parent.parent.GetComponent<RectTransform>().rect.size * 1.25f;
        }
        else
        {
            rt.sizeDelta = GetComponentInParent<RectTransform>().rect.size;
        }
    }

    public void Selected(bool selected)
    {
        image.sprite = selected ? selectedSprite : deselectedSprite;
    }

    public void SubSelected(bool subselected)
    {
        image.sprite = subselected ? subSelectedSprite : image.sprite;
    }
}