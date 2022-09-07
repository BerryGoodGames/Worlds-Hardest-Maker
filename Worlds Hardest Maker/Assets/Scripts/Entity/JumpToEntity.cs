using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpToEntity : MonoBehaviour
{
    public const float deviation = 1000;
    public GameObject target;
    public bool smooth;
    public float speed;
    private bool jumping = false;

    public void Jump()
    {
        if(target != null)
        {
            Vector2 pos = target.transform.position;
            if (smooth) jumping = true;
            else transform.position = new(pos.x, pos.y, transform.position.z);
        }
    }

    private void FixedUpdate()
    {
        if (jumping)
        {
            Vector2 pos = target.transform.position;
            Vector2 newPos = Vector2.Lerp(transform.position, target.transform.position, Time.fixedDeltaTime * speed);
            transform.position = new(newPos.x, newPos.y, transform.position.z);
            
            if (Mathf.Round(transform.position.x * deviation) == Mathf.Round(pos.x * deviation) &&
                Mathf.Round(transform.position.y * deviation) == Mathf.Round(pos.y * deviation)) jumping = false;
        }
    }
}
