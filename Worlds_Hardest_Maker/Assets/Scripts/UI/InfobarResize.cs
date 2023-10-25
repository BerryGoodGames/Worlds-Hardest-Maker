using MyBox;
using TMPro;
using UnityEngine;

public class InfobarResize : MonoBehaviour
{
    public float InfobarHeight;
    public GameObject[] InfoTexts;

    [MustBeAssigned] [SerializeField] private CustomFitter fitter;

    [ButtonMethod]
    public void UpdateSize()
    {
        float height = InfobarHeight;

        ((RectTransform)transform).sizeDelta = new(0, height);

        RectTransform backgroundRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        backgroundRectTransform.sizeDelta = new(backgroundRectTransform.rect.width, height + 200);

        foreach (GameObject t in InfoTexts)
        {
            // text.transform.localScale = new(height / 100, height / 100);
            TMP_Text text = t.GetComponent<TMP_Text>();

            text.fontSize = InfobarHeight * 0.514f;
        }

        fitter.UpdateSize();
    }

    public void ExpandToEntireWidth()
    {
        RectTransform rt = (RectTransform)transform.GetChild(0);

        rt.sizeDelta = new(0, rt.sizeDelta.y);
        rt.anchoredPosition = new(0, rt.anchoredPosition.y);
    }
}