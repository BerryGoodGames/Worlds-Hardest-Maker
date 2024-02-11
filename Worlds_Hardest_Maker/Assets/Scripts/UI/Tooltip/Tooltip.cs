using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MouseOverUIRect))]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltipPrefab;

    [FormerlySerializedAs("customContainer")] [Space] public bool CustomContainer;

    [FormerlySerializedAs("container")] [ConditionalField(nameof(CustomContainer))] public Transform Container;

    [Space] [SerializeField] private bool restrictInCanvas = true;

    [FormerlySerializedAs("CustomRestrictContainer")] [SerializeField] [ConditionalField(nameof(restrictInCanvas))] private bool customRestrictContainer;

    [SerializeField] [ConditionalField(nameof(customRestrictContainer), nameof(restrictInCanvas))] private RectTransform restrictContainer;

    [Separator] [SerializeField] private string text;

    [SerializeField] private int fontSize = 20;

    [SerializeField] private int offset = 10;

    [SerializeField] private bool customTweenDelay;

    [ConditionalField(nameof(customTweenDelay))] [SerializeField] private float tweenDelay = 1.5f;

    private const float defaultTweenDelay = 1;
    private MouseOverUIRect mouseOver;
    private AlphaTween fadeTween;
    private GameObject tooltip;
    private RectTransform tooltipRectTransform;
    private TMP_Text tooltipText;

    private float hovered;

    private void Awake()
    {
        if (!customTweenDelay) tweenDelay = defaultTweenDelay;
    }

    private void Start()
    {
        mouseOver = GetComponent<MouseOverUIRect>();

        tooltip = Instantiate(
            tooltipPrefab, Vector3.zero, Quaternion.identity,
            CustomContainer ? Container : ReferenceManager.Instance.TooltipCanvas.transform
        );

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

    public void SetContainer(Transform container)
    {
        CustomContainer = true;
        Container = container;
        
        if (tooltip == null) return;
        
        tooltip.transform.SetParent(container);
    }

    private void OnDisable()
    {
        if (fadeTween != null) fadeTween.SetVisible(false);
    }

    private void OnDestroy() => Destroy(tooltip);
}