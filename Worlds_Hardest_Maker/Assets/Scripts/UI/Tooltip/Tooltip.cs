using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MouseOverUI))]
public class Tooltip : MonoBehaviour
{
    [FormerlySerializedAs("text")] public string Text;
    [FormerlySerializedAs("fontSize")] public int FontSize = 20;
    [FormerlySerializedAs("customTweenDelay")] public bool CustomTweenDelay;
    [FormerlySerializedAs("tweenDelay")] public float TweenDelay = 1.5f;
    private const float defaultTweenDelay = 1;
    private MouseOverUI mouseOver;
    private AlphaUITween fadeTween;
    private GameObject tooltip;
    private RectTransform tooltipRectTransform;
    private TMP_Text tooltipText;

    private float hovered;

    private void Awake()
    {
        if (!CustomTweenDelay)
            TweenDelay = defaultTweenDelay;
    }

    private void Start()
    {
        mouseOver = GetComponent<MouseOverUI>();

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

    private void OnDisable()
    {
        fadeTween.SetVisible(false);
    }

    private void OnDestroy()
    {
        Destroy(tooltip);
    }
}