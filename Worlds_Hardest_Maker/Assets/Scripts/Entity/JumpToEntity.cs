using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToEntity : MonoBehaviour
{
    public const float deviation = 1000;
    public GameObject target;
    public bool smooth;
    [SerializeField] private bool cancelByRightClick = true;
    public float speed;
    private bool jumping = false;
    private Vector2 currentTarget;

    public void Jump(bool onlyIfTargetOffScreen = false)
    {
        if(target != null && (!onlyIfTargetOffScreen || (onlyIfTargetOffScreen && !target.GetComponent<Renderer>().isVisible)))
        {
            currentTarget = target.transform.position;

            if (smooth) jumping = true;
            else transform.position = new(currentTarget.x, currentTarget.y, transform.position.z);
        }
    }

    private void Update()
    {
        if(jumping && Input.GetMouseButton(1)) jumping = false;
    }

    private void FixedUpdate()
    {
        if (jumping)
        {
            Vector2 newPos = Vector2.Lerp(transform.position, currentTarget, Time.fixedDeltaTime * speed);
            transform.position = new(newPos.x, newPos.y, transform.position.z);
            
            if (Mathf.Round(transform.position.x * deviation) == Mathf.Round(currentTarget.x * deviation) &&
                Mathf.Round(transform.position.y * deviation) == Mathf.Round(currentTarget.y * deviation)) jumping = false;
        }
    }
}
