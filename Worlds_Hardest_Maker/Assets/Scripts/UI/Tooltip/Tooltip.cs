using MyBox;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class Tooltip : MonoBehaviour
{
    public string Text;
    public int FontSize = 20;

    public bool CustomTweenDelay;

    [ConditionalField(nameof(CustomTweenDelay))]
    public float TweenDelay = 1.5f;

    private const float DefaultTweenDelay = 1;
    private MouseOverUIRect mouseOver;
    private AlphaUITween fadeTween;
    private GameObject tooltip;
    private RectTransform tooltipRectTransform;
    private TMP_Text tooltipText;

    private float hovered;

    private void Awake()
    {
        if (!CustomTweenDelay)
            TweenDelay = DefaultTweenDelay;
    }

    private void Start()
    {
        mouseOver = GetComponent<MouseOverUIRect>();

        tooltip = Instantiate(PrefabManager.Instance.Tooltip, Vector2.zero, Quaternion.identity,
            ReferenceManager.Instance.TooltipCanvas.transform);
        fadeTween = tooltip.GetComponent<AlphaUITween>();

        tooltipRectTransform = tooltip.GetComponent<RectTransform>();
        tooltipText = tooltip.GetComponent<TooltipController>().Text;
        tooltipText.text = Text;
        tooltipText.fontSize = FontSize;
        fadeTween.SetVisible(false);
    }

    private void Update()
    {
        if (mouseOver.Over)
        {
            if (hovered > TweenDelay)
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

    private void OnDisable() => fadeTween.SetVisible(false);

    private void OnDestroy() => Destroy(tooltip);
}