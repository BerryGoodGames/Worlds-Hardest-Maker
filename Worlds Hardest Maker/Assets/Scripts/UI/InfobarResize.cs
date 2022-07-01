using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfobarResize : MonoBehaviour
{
    public float infobarHeight;
    public GameObject[] infoTexts;

    public void UpdateSize()
    {
        float height = infobarHeight;

        transform.GetComponent<RectTransform>().sizeDelta = new(0, height);

        RectTransform bgrt = transform.GetChild(0).GetComponent<RectTransform>();
        bgrt.sizeDelta = new(bgrt.rect.width, height + 200);

        foreach (GameObject text in infoTexts)
        {
            text.transform.localScale = new(height / 100, height / 100);
        }
    }
}
