using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class InfobarResize : MonoBehaviour
{
    [FormerlySerializedAs("infobarHeight")] public float InfobarHeight;
    [FormerlySerializedAs("infoTexts")] public GameObject[] InfoTexts;

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
    }
}