using System.Collections;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldRotation : MonoBehaviour
{
    // hippety hoppety
    public float Duration;
    public Vector3 RotateAngle = new(0, 0, -90);
    private bool rotating;
    [SerializeField] private bool disableCollision;
    [SerializeField] [ConditionalField(nameof(disableCollision))] [MustBeAssigned] private BoxCollider2D boxCollider;

    private FieldController controller;

    private static readonly int rotateString = Animator.StringToHash("Rotate");

    private IEnumerator Rotate(Vector3 angles, float d)
    {
        rotating = true;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(angles) * startRotation;

        for (float t = 0; t < d; t += Time.deltaTime)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, t / d);
            yield return null;
        }

        transform.rotation = endRotation;
        rotating = false;

        if (disableCollision) boxCollider.isTrigger = false;
    }


    public void StartRotation()
    {
        if (rotating || EventSystem.current.IsPointerOverGameObject()) return;

        if (disableCollision) boxCollider.isTrigger = true;

        Animator anim = GetComponent<Animator>();
        anim.SetTrigger(rotateString);

        StartCoroutine(Rotate(RotateAngle, Duration));
    }

    private void OnMouseUpAsButton()
    {
        if (SelectionManager.Instance.Selecting || CopyManager.Instance.Pasting || LevelSessionEditManager.Instance.Playing) return;

        if (LevelSessionEditManager.Instance.CurrentEditMode != controller.FieldMode) return;

        StartRotation();
    }

    private void Awake() => controller = GetComponent<FieldController>();
}