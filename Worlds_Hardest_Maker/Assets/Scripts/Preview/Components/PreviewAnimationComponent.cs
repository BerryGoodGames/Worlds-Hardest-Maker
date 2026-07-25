using UnityEngine;
using VContainer;

/// <summary>
///     Handles animation state for preview visibility.
///     Uses animator to show/hide preview based on visibility rules.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PreviewAnimationComponent : MonoBehaviour
{
    private static readonly int VISIBLE = Animator.StringToHash("Visible");
    private Animator animator;
    private PreviewVisibilityRulesService visibilityService;

    [Inject]
    private void Construct(PreviewVisibilityRulesService visibilityService)
    {
        this.visibilityService = visibilityService;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (animator == null) return;

        EditMode currentEditMode = LevelSessionEditManager.Instance.CurrentEditMode;
        bool isVisible = visibilityService.IsPreviewVisible(currentEditMode);
        animator.SetBool(VISIBLE, isVisible);
    }
}