using UnityEngine;

/// <summary>
///     script for consistent seamless background
///     attach to main camera
/// </summary>
public class LevelBackground : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private Transform container;
    [SerializeField] private float defaultMaxZoom;
    private Camera cam;
    private Vector2 prevPosition;
    private float height;
    private float width;

    private void Start()
    {
        cam = GetComponent<Camera>();

        CalcSize(TryGetComponent(out MapController mapController) ? mapController.MaxZoom : defaultMaxZoom);
    }

    private void Update()
    {
        Vector2 camPosition = cam.transform.position;

        if (prevPosition != camPosition)
        {
            container.position = new(Mathf.Floor(camPosition.x * 0.5f) * 2, Mathf.Floor(camPosition.y * 0.5f) * 2);
        }

        prevPosition = camPosition;
    }

    public void CalcSize(float zoom)
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        Vector2 containerPos = container.position;

        height = zoom;
        width = height * cam.aspect;
        for (float i = Mathf.Floor(-width + 1); i < Mathf.Ceil(width + 2); i++)
        {
            for (float j = Mathf.Floor(-height + 1); j < Mathf.Ceil(height + 2); j++)
            {
                if ((i + j) % 2 == 0) continue;
                Instantiate(tile, new(i + containerPos.x, j + containerPos.y), Quaternion.identity, container);
            }
        }
    }

    public void CalcSize()
    {
        CalcSize(cam.orthographicSize);
    }
}