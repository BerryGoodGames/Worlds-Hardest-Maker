using System;
using MyBox;
using UnityEngine;

public class KeyDoorFieldController : MonoBehaviour, IResettable
{
    [SerializeField] [InitializationField] [MustBeAssigned] private Animator animator;

    [SerializeField] [InitializationField] [MustBeAssigned] private BoxCollider2D boxCollider;

    [Separator] [ReadOnly] public bool Unlocked;
    [ReadOnly] public KeyColor Color;

    private static readonly int unlockedString = Animator.StringToHash("Unlocked");

    public void SetLocked(bool locked)
    {
        Unlocked = !locked;

        boxCollider.enabled = locked;

        animator.SetBool(unlockedString, !locked);
    }

    private void Start() => ((IResettable)this).Subscribe();

    public void ResetState() => SetLocked(true);
    
    private void OnDestroy() => ((IResettable)this).Unsubscribe();
}