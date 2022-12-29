using TMPro;
using UnityEngine;

[RequireComponent(typeof(MouseOverUI))]
public class Tooltip : MonoBehaviour
{
    public string text;
    public int fontSize = 20;
    public bool customTweenDelay;
    public float tweenDelay = 1.5f;
    private const float defaultTweenDelay = 1;
    private MouseOverUI mouseOver;
    private AlphaUITween fadeTween;
    private GameObject tooltip;
    private RectTransform tooltipRectTransform;
    private TMP_Text tooltipText;

    private float hovered;

    private void Awake()
    {
        if (!customTweenDelay)
            tweenDelay = defaultTweenDelay;
    }

    private void Start()
    {
        mouseOver = GetComponent<MouseOverUI>();

        tooltip = Instantiate(PrefabManager.Instance.tooltip, Vector2.zero, Quaternion.identity,
            ReferenceManager.Instance.tooltipCanvas.transform);
        fadeTween = tooltip.GetComponent<AlphaUITween>();

        tooltipRectTransform = tooltip.GetComponent<RectTransform>();
        tooltipText = tooltip.GetComponent<TooltipController>().text;
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