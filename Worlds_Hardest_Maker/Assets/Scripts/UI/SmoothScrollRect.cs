using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
///     Version of <see cref="ScrollRect" /> that supports smooth scrolling.
/// </summary>
public class SmoothScrollRect : ScrollRect, IPointerEnterHandler, IPointerExitHandler
{
    public bool SmoothScrolling { get; set; } = true;
    public float SmoothScrollTime { get; set; } = 0.12f;

    private const string MouseScrollWheelAxis = "Mouse ScrollWheel";
    private bool swallowMouseWheelScrolls = true;
    private bool isMouseOver;

    public override void OnScroll(PointerEventData data)
    {
        if (!IsActive()) return;

        // Eat the scroll so that we don't get a double scroll when the mouse is over an image
        if (IsMouseWheelRolling() && swallowMouseWheelScrolls) return;

        if (SmoothScrolling)
        {
            // Amplify the mousewheel so that it matches the scroll sensitivity.
            if (data.scrollDelta.y < -Mathf.Epsilon)
                data.scrollDelta = new Vector2(0f, -scrollSensitivity);
            else if (data.scrollDelta.y > Mathf.Epsilon)
                data.scrollDelta = new Vector2(0f, scrollSensitivity);

            Vector2 positionBefore = normalizedPosition;
            this.DOKill(true);
            base.OnScroll(data);
            Vector2 positionAfter = normalizedPosition;

            normalizedPosition = positionBefore;
            this.DONormalizedPos(positionAfter, SmoothScrollTime);
        }
        else
            base.OnScroll(data);
    }

    private void Update()
    {
        // Detect the mouse wheel and generate a scroll. This fixes the issue where Unity will prevent our ScrollRect
        // from receiving any mouse wheel messages if the mouse is over a raycast target (such as a button).
        if (!isMouseOver || !IsMouseWheelRolling()) return;

        float delta = Input.GetAxis(MouseScrollWheelAxis);

        PointerEventData pointerData = new(EventSystem.current)
        {
            scrollDelta = new Vector2(0f, delta)
        };

        swallowMouseWheelScrolls = false;
        OnScroll(pointerData);
        swallowMouseWheelScrolls = true;
    }

    private static bool IsMouseWheelRolling() => Input.GetAxis(MouseScrollWheelAxis) != 0;

    public void OnPointerEnter(PointerEventData eventData) => isMouseOver = true;

    public void OnPointerExit(PointerEventData eventData) => isMouseOver = false;
}