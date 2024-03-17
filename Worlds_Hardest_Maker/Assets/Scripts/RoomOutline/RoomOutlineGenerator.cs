using System;
using MyBox;
using UnityEngine;

public class RoomOutlineGenerator : MonoBehaviour
{
    private const int ROOM_WIDTH = 23;
    private const int ROOM_HEIGHT = 13;

    [SerializeField] [InitializationField] [MustBeAssigned] private RoomOutline roomOutlinePrefab;
    [SerializeField] [InitializationField] [MustBeAssigned] private Camera camera;
    
    private Vector2 prevPosition;
    
    private void Start()
    {
        if (!LevelSessionManager.Instance.IsEdit)
        {
            Destroy(this);
            return;
        }

        float zoom = camera.GetComponent<MapController>().ZoomLimits.Max;
        CalcSize(zoom);
    }

    private void Update()
    {
        Vector2 camPosition = camera.transform.position;
        if (prevPosition != (Vector2)camera.transform.position)
        {
            transform.position = new(
                Mathf.Round(camPosition.x / ROOM_WIDTH) * ROOM_WIDTH, 
                Mathf.Round(camPosition.y / ROOM_HEIGHT) * ROOM_HEIGHT
            );
        }

        prevPosition = camPosition;
    }

    private void CalcSize(float zoom)
    {
        Transform t = transform;
        foreach (Transform child in t) Destroy(child.gameObject);

        float height = zoom;
        float width = height * camera.aspect;

        float minX = (Mathf.Ceil(-width / ROOM_WIDTH) - 1) * ROOM_WIDTH;
        float maxX = (Mathf.Ceil(width / ROOM_WIDTH) + 1) * ROOM_WIDTH;
        float minY = (Mathf.Ceil(-height / ROOM_HEIGHT) - 1) * ROOM_HEIGHT;
        float maxY = (Mathf.Ceil(height / ROOM_HEIGHT) + 1) * ROOM_HEIGHT;
        
        for (float i = minX; i < maxX; i += ROOM_WIDTH)
        {
            for (float j = minY; j < maxY; j += ROOM_HEIGHT)
            {
                RoomOutline outline = Instantiate(
                    roomOutlinePrefab, new Vector3(i, j) + t.position,
                    Quaternion.identity, t
                );

                outline.SetDimensions(ROOM_WIDTH, ROOM_HEIGHT);
            }
        }
    }
}
