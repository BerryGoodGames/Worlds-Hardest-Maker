using MyBox;
using UnityEngine;

public class SyncTransform : MonoBehaviour
{
    [InitializationField] public Transform Source;

    [Separator] [SerializeField] [InitializationField]
    private bool ignorePosition;

    [SerializeField] [InitializationField] private bool ignoreRotation;
    [SerializeField] [InitializationField] private bool ignoreScale;

    private void Update()
    {
        // Synchronize the target transform with the source transform
        if (!ignorePosition) transform.position = Source.position;
        if (!ignoreRotation) transform.rotation = Source.rotation;
        if (!ignoreScale) transform.localScale = Source.localScale;
    }
}