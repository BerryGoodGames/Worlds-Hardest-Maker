using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private string text;
    [SerializeField] private int fontSize = 20;
    private MouseOverUI mouseOver;
    private GameObject tooltip;
    private RectTransform tooltipRectTransform;
    private TMPro.TMP_Text tooltipText;

    private void Start()
    {
        if (!TryGetComponent(out mouseOver))
        {
            mouseOver = gameObject.AddComponent<MouseOverUI>();
        }

        tooltip = Instantiate(GameManager.Instance.Tooltip, Vector2.zero, Quaternion.identity, GameManager.Instance.TooltipCanvas.transform);
        tooltipRectTransform = tooltip.GetComponent<RectTransform>();
        tooltipText = tooltip.GetComponent<TooltipController>().Text;
        tooltipText.text = text;
        tooltipText.fontSize = fontSize;
        tooltip.SetActive(false);
    }

    private void Update()
    {
        if (mouseOver.over)
        {
            tooltip.SetActive(true);
            tooltip.transform.position = Input.mousePosition;

            float pivotX = tooltip.transform.position.x / Screen.width;
            tooltipRectTransform.pivot = new(pivotX, tooltipRectTransform.pivot.y);
        }
        else
        {
            tooltip.SetActive(false);
        }
    }

    private void OnDisable()
    {
        tooltip.SetActive(false);
    }

    private void OnDestroy()
    {
        Destroy(tooltip);
    }

}
