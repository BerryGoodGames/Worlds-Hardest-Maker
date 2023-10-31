using System;
using MyBox;
using UnityEngine;

/// <summary>
///     Detects collision logic of rect transform and the mouse
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class MouseOverUIRect : MonoBehaviour
{
    public bool Over { get; private set; }

    [Space] [ReadOnly] [SerializeField] private bool over;

    [Space] [SerializeField] private bool printDbg;

    private RectTransform rt;

    private Rect rtConverted;
    private float width;
    private float height;

    public Action OnHovered = () => { };
    public Action OnUnhovered = () => { };

    private void Start() => rt = GetComponent<RectTransform>();

    public void UpdateSize()
    {
        rtConverted = UnitPixelUtils.RectTransformToScreenSpace(rt);
        width = rtConverted.width;
        height = rtConverted.height;
    }

    private void Update()
    {
        UpdateSize();

        Vector2 position = rt.position;
        Vector2 pivot = rt.pivot;

        Set(
            Input.mousePosition.x > position.x - width * pivot.x &&
            Input.mousePosition.x < position.x + width * (1 - pivot.x) &&
            Input.mousePosition.y > position.y - height * pivot.y &&
            Input.mousePosition.y < position.y + height * (1 - pivot.y)
        );
    }

    private void OnDisable() => Set(false);

    private void Set(bool over)
    {
        this.over = over;

        if (over)
        {
            if (!Over)
            {
                OnHovered.Invoke();

                if (printDbg) print($"{name} (Rect): Pointer enter");
            }

            Over = true;
        }
        else
        {
            if (Over)
            {
                OnUnhovered.Invoke();

                if (printDbg) print($"{name} (Rect): Pointer exit");
            }

            Over = false;
        }
    }
}