using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelectionSquare : MonoBehaviour
{
    private Image image;

    [FormerlySerializedAs("deselectedSprite")]
    public Sprite DeselectedSprite;

    [FormerlySerializedAs("selectedSprite")]
    public Sprite SelectedSprite;

    [FormerlySerializedAs("subSelectedSprite")]
    public Sprite SubSelectedSprite;

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
        image.sprite = selected ? SelectedSprite : DeselectedSprite;
    }

    public void SubSelected(bool subselected)
    {
        image.sprite = subselected ? SubSelectedSprite : image.sprite;
    }
}