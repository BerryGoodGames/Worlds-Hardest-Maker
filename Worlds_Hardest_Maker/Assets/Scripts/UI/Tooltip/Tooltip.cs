using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(MouseOverUI))]
public class Tooltip : MonoBehaviour
{
    public string text;
    public int fontSize = 20;
    public bool customTweenDelay = false;
    public float tweenDelay = 1.5f;
    private const float defaultTweenDelay = 1;
    private MouseOverUI mouseOver;
    private AlphaUITween fadeTween;
    private GameObject tooltip;
    private RectTransform tooltipRectTransform;
    private TMPro.TMP_Text tooltipText;

    private float hovered = 0;

    private void Awake()
    {
        if (!customTweenDelay)
            tweenDelay = defaultTweenDelay;
    }

    private void Start()
    {
        mouseOver = GetComponent<MouseOverUI>();

        tooltip = Instantiate(PrefabManager.Instance.Tooltip, Vector2.zero, Quaternion.identity, GameManager.Instance.TooltipCanvas.transform);
        fadeTween = tooltip.GetComponent<AlphaUITween>();

        tooltipRectTransform = tooltip.GetComponent<RectTransform>();
        tooltipText = tooltip.GetComponent<TooltipController>().Text;
        tooltipText.text = text;
        tooltipText.fontSize = fontSize;
        fadeTween.SetVisible(false);
    }

    private void Update()
    {
        if (mouseOver.over)
        {
            if (hovered > tweenDelay)
            {
                fadeTween.SetVisible(true);

                tooltip.transform.position = Input.mousePosition;

                float pivotX = tooltip.transform.position.x / Screen.width;
                tooltipRectTransform.pivot = new(pivotX, tooltipRectTransform.pivot.y);
            }

            hovered += Time.deltaTime;
        }
        else
        {
            fadeTween.SetVisible(false);
            hovered = 0;
        }
    }

    private void OnDisable()
    {
        fadeTween.SetVisible(false);
    }

    private void OnDestroy()
    {
        Destroy(tooltip);
    }

}
