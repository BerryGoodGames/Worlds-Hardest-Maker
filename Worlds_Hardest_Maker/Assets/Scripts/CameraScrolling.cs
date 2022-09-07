using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// lets camera scroll at random dir or specified angle with specified speed
/// attach to main camera
/// </summary>
public class CameraScrolling : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private bool randomDir;
    [SerializeField] private float scrollDirAngle;
    private Vector2 dir;

    private void Awake()
    {
        if (randomDir)
        {
            // generate random unit vector
            float random = Random.Range(0f, 260f);
            dir = new(Mathf.Cos(random), Mathf.Sin(random));
        }
        else
        {
            dir = new((float)Mathf.Cos(scrollDirAngle * Mathf.PI / 180), (float)Mathf.Sin(scrollDirAngle * Mathf.PI / 180));
        }
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)dir * speed;
    }
}
