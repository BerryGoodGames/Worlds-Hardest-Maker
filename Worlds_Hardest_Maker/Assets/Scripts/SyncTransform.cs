using MyBox;
using UnityEngine;

public class SyncTransform : MonoBehaviour
{
    [InitializationField] public Transform Source;

    [Separator]
    [SerializeField] [InitializationField] private bool syncPosition = true;
    [SerializeField] [InitializationField] private bool syncRotation = true;
    [SerializeField] [InitializationField] private bool syncScale = true;

    private void Update()
    {
        // Synchronize the target transform with the source transform
        if (syncPosition) transform.position = Source.position;
        if (syncRotation) transform.rotation = Source.rotation;
        if (syncScale) transform.localScale = Source.localScale;
    }
}