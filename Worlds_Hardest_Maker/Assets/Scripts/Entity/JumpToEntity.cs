using System;
using UnityEngine;
using UnityEngine.Serialization;

public class JumpToEntity : MonoBehaviour
{
    public const float Deviation = 1000;
    [FormerlySerializedAs("target")] public GameObject Target;
    [FormerlySerializedAs("smooth")] public bool Smooth;
    [FormerlySerializedAs("speed")] public float Speed;
    [SerializeField] private readonly bool cancelByRightClick = true;

    private bool jumping;
    private Vector2 currentTarget;

    public void Jump(bool onlyIfTargetOffScreen = false)
    {
        if (Target == null) return;

        Renderer targetRenderer = Target.GetComponent<Renderer>();

        if ((onlyIfTargetOffScreen && targetRenderer.isVisible) || Target == null) return;

        currentTarget = Target.transform.position;

        if (Smooth)
        {
            jumping = true;
        }
        else
        {
            Transform t = transform;
            t.position = new(currentTarget.x, currentTarget.y, t.position.z);
        }
    }

    private void Update()
    {
        if (cancelByRightClick && jumping && Input.GetMouseButton(KeybindManager.Instance.PanMouseButton))
            jumping = false;
    }

    private void FixedUpdate()
    {
        if (!jumping) return;

        Vector2 newPos = Vector2.Lerp(transform.position, currentTarget, Time.fixedDeltaTime * Speed);
        transform.position = new(newPos.x, newPos.y, transform.position.z);

        if (Math.Abs(Mathf.Round(transform.position.x * Deviation) - Mathf.Round(currentTarget.x * Deviation)) == 0 &&
            Math.Abs(Mathf.Round(transform.position.y * Deviation) - Mathf.Round(currentTarget.y * Deviation)) == 0)
            jumping = false;
    }
}