using MyBox;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MouseOverUIRect))]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltipPrefab;

    [Space] [SerializeField] private bool customContainer;

    [SerializeField] [ConditionalField(nameof(customContainer))]
    private Transform container;

    public Transform Container
    {
        get => container;
        set
        {
            if (tooltip != null)
            {
                tooltip.transform.SetParent(value);
            }

            container = value;
        }
    }

    [Space] [SerializeField] private bool restrictInCanvas = true;

    [SerializeField] [ConditionalField(nameof(restrictInCanvas))]
    private bool customRestrictContainer;

    [SerializeField] [ConditionalField(nameof(customRestrictContainer), nameof(restrictInCanvas))]
    private RectTransform restrictContainer;

    [Separator] [SerializeField] private string text;

    [SerializeField] private int fontSize = 20;

    [SerializeField] private int offset = 10;

    [SerializeField] private bool customTweenDelay;

    [ConditionalField(nameof(customTweenDelay))] [SerializeField]
    private float tweenDelay = 1.5f;

    private const float DefaultTweenDelay = 1;
    private MouseOverUIRect mouseOver;
    private AlphaTween fadeTween;
    private GameObject tooltip;
    private RectTransform tooltipRectTransform;
    private TMP_Text tooltipText;

    private float hovered;

    private void Awake()
    {
        if (!customTweenDelay)
            tweenDelay = DefaultTweenDelay;
    }

    private void Start()
    {
        mouseOver = GetComponent<MouseOverUIRect>();

        tooltip = Instantiate(tooltipPrefab, Vector3.zero, Quaternion.identity,
            customContainer ? Container : ReferenceManager.Instance.TooltipCanvas.transform);

        UIRestrict restrict = tooltip.GetComponent<UIRestrict>();
        if (restrictInCanvas)
        {
            if (customRestrictContainer)
            {
                restrict.CustomRestrictContainer = true;
                restrict.RestrictContainer = restrictContainer;
            }
        }
        else Destroy(restrict);

        fadeTween = tooltip.GetComponent<AlphaTween>();

        tooltipRectTransform = tooltip.GetComponent<RectTransform>();
        tooltipText = tooltip.GetComponent<TooltipController>().Text;
        tooltipText.text = text.Replace("\\n", "\n");
        tooltipText.fontSize = fontSize;
        fadeTween.SetVisible(false);
    }

    private void Update()
    {
        if (mouseOver.Over)
        {
            if (hovered > tweenDelay)
            {
                fadeTween.SetVisible(true);

                tooltipRectTransform.position = Input.mousePosition + new Vector3(offset, -offset);
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