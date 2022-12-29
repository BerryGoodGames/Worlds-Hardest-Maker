using System;
using UnityEngine;

public class JumpToEntity : MonoBehaviour
{
    public const float deviation = 1000;
    public GameObject target;
    public bool smooth;
    public float speed;
    [SerializeField] private bool cancelByRightClick = true;

    private bool jumping;
    private Vector2 currentTarget;

    public void Jump(bool onlyIfTargetOffScreen = false)
    {
        if (target == null) return;

        Renderer targetRenderer = target.GetComponent<Renderer>();

        if ((onlyIfTargetOffScreen && targetRenderer.isVisible) || target == null) return;

        currentTarget = target.transform.position;

        if (smooth) jumping = true;
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

        Vector2 newPos = Vector2.Lerp(transform.position, currentTarget, Time.fixedDeltaTime * speed);
        transform.position = new(newPos.x, newPos.y, transform.position.z);

        if (Math.Abs(Mathf.Round(transform.position.x * deviation) - Mathf.Round(currentTarget.x * deviation)) == 0 &&
            Math.Abs(Mathf.Round(transform.position.y * deviation) - Mathf.Round(currentTarget.y * deviation)) == 0)
            jumping = false;
    }
}