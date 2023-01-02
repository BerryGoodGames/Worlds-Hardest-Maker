using TMPro;
using UnityEngine;

public class InfobarResize : MonoBehaviour
{
    public float infobarHeight;
    public GameObject[] infoTexts;

    public void UpdateSize()
    {
        float height = infobarHeight;

        transform.GetComponent<RectTransform>().sizeDelta = new(0, height);

        RectTransform backgroundRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        backgroundRectTransform.sizeDelta = new(backgroundRectTransform.rect.width, height + 200);

        foreach (GameObject t in infoTexts)
        {
            // text.transform.localScale = new(height / 100, height / 100);
            TMP_Text text = t.GetComponent<TMP_Text>();

            text.fontSize = infobarHeight * 0.514f;
        }
    }
}