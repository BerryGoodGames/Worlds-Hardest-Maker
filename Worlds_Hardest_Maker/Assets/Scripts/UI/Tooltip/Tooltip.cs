using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MouseOverUIRect))]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltipPrefab;

    [FormerlySerializedAs("useCustomContainer")] [SerializeField]
    private bool customContainer;

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

    [SerializeField] private bool restrictInCanvas = true;

    [FormerlySerializedAs("Text")] [Separator] [SerializeField]
    private string text;

    [FormerlySerializedAs("FontSize")] [SerializeField]
    private int fontSize = 20;

    [FormerlySerializedAs("Offset")] [SerializeField]
    private int offset = 10;

    [FormerlySerializedAs("CustomTweenDelay")] [SerializeField]
    private bool customTweenDelay;

    [FormerlySerializedAs("TweenDelay")] [ConditionalField(nameof(customTweenDelay))] [SerializeField]
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

        if (!restrictInCanvas) Destroy(tooltip.GetComponent<UIRestrict>());

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