using MyBox;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class Tooltip : MonoBehaviour
{
    public string Text;
    public int FontSize = 20;
    public int Offset = 10;

    public bool CustomTweenDelay;

    [ConditionalField(nameof(CustomTweenDelay))]
    public float TweenDelay = 1.5f;

    private const float DefaultTweenDelay = 1;
    private MouseOverUIRect mouseOver;
    private AlphaTween fadeTween;
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

        tooltip = Instantiate(PrefabManager.Instance.Tooltip, Vector3.zero, Quaternion.identity,
            ReferenceManager.Instance.TooltipCanvas.transform);
        fadeTween = tooltip.GetComponent<AlphaTween>();

        tooltipRectTransform = tooltip.GetComponent<RectTransform>();
        tooltipText = tooltip.GetComponent<TooltipController>().Text;
        tooltipText.text = Text.Replace("\\n", "\n");
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

                tooltipRectTransform.position = Input.mousePosition + new Vector3(Offset, -Offset);
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