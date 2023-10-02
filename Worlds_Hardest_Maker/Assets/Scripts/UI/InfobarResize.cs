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

        transform.GetComponent<RectTransform>().sizeDelta = new(0, height);

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
}