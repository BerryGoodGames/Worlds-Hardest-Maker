using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script for consistent seemless background
/// attach to main camera
/// </summary>
public class BackgroundLoop : MonoBehaviour
{
    public GameObject tile;
    public Transform container;
    private Camera cam;
    private Vector3 prevPosition;
    private float height;
    private float width;

    private void Start()
    {
        cam = GetComponent<Camera>();
        CalcSize(GetComponent<MapController>().maxZoom);
    }

    private void Update()
    {
        if(prevPosition != cam.transform.position)
        {
            container.position = new(Mathf.Floor(cam.transform.position.x / 2) * 2, Mathf.Floor(cam.transform.position.y / 2) * 2);
        }
        prevPosition = cam.transform.position;
    }

    public void CalcSize(float zoom)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        height = zoom;
        width = height * cam.aspect;
        for (float i = Mathf.Floor(-width + 1); i < Mathf.Ceil(width + 2); i++)
        {
            for (float j = Mathf.Floor(-height + 1); j < Mathf.Ceil(height + 2); j++)
            {
                if ((i + j) % 2 == 0) continue;
                Instantiate(tile, new(i + container.position.x, j + container.position.y), Quaternion.identity, container);
            }
        }
    }
    public void CalcSize()
    {
        CalcSize(cam.orthographicSize);
    }
}
