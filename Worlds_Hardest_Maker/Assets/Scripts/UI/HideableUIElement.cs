using MyBox;
using UnityEngine;

[RequireComponent(typeof(PanelTween))]
public class HideableUIElement : MonoBehaviour, IHideableUI
{
    private PanelTween tween;

    [field: SerializeField] [field: InitializationField] public bool IsHidden { get; private set; }

    public void SetHidden(bool hidden)
    {
        IsHidden = hidden;
        tween.SetOpen(!IsHidden);
    }

    private void Awake()
    {
        tween = GetComponent<PanelTween>();
    }

    private void Start()
    {
        tween.SetOpen(!IsHidden, true);
    }
}