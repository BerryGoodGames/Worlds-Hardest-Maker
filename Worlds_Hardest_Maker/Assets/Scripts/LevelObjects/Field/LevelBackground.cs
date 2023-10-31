using UnityEngine;

/// <summary>
///     Consistent seamless background
///     <para>Attach to main camera</para>
/// </summary>
public class LevelBackground : MonoBehaviour
{
    [SerializeField] private GameObject backgroundTile;
    [SerializeField] private Transform container;
    [SerializeField] private float defaultMaxZoom;
    [SerializeField] private Vector2 tileSize = Vector2.one;
    private Camera cam;
    private Vector2 prevPosition;
    private float height;
    private float width;

    private void Start()
    {
        cam = GetComponent<Camera>();

        CalcSize(TryGetComponent(out MapController mapController) ? mapController.ZoomLimits.Max : defaultMaxZoom);
    }

    private void Update()
    {
        Vector2 camPosition = cam.transform.position;

        if (prevPosition != camPosition) container.position = new(Mathf.Floor(camPosition.x * 0.5f) * 2, Mathf.Floor(camPosition.y * 0.5f) * 2);

        prevPosition = camPosition;
    }

    public void CalcSize(float zoom)
    {
        foreach (Transform child in container) { Destroy(child.gameObject); }

        Vector2 containerPos = container.position;

        height = zoom;
        width = height * cam.aspect;
        for (float i = Mathf.Floor(-width + 1); i < Mathf.Ceil(width + 2); i += tileSize.x)
        {
            for (float j = Mathf.Floor(-height + 1); j < Mathf.Ceil(height + 2); j += tileSize.y)
            {
                // TODO: inconsistent if tileSize isn't (0, 0)
                int mx = Mathf.RoundToInt((i - Mathf.Floor(-width + 1)) / tileSize.x);
                int my = Mathf.RoundToInt((j - Mathf.Floor(-height + 1)) / tileSize.y);
                if ((mx + my) % 2 == 0) continue;
                GameObject tile = Instantiate(
                    backgroundTile, new(i + containerPos.x, j + containerPos.y),
                    Quaternion.identity, container
                );

                tile.transform.localScale = tileSize;
            }
        }
    }

    public void CalcSize() => CalcSize(cam.orthographicSize);
}