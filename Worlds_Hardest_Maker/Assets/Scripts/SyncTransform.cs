using MyBox;
using UnityEngine;

public class SyncTransform : MonoBehaviour
{
    [InitializationField] public Transform Source;

    private void Update()
    {
        // Synchronize the target transform with the source transform
        transform.position = Source.position;
        transform.rotation = Source.rotation;
        transform.localScale = Source.localScale;
    }
}