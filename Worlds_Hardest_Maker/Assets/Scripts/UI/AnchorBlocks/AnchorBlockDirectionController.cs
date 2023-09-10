using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class AnchorBlockDirectionController : MonoBehaviour
{
    [SerializeField] [InitializationField] private Image directionImage;

    [SerializeField] [InitializationField] [PositiveValueOnly]
    private float animationDuration;

    private bool isClockwise = true;

    public bool IsClockwise
    {
        get => isClockwise;
        set
        {
            if (value != isClockwise) Toggle(false);
        }
    }

    [ButtonMethod]
    public void Toggle(bool animation = true)
    {
        isClockwise = !isClockwise;

        directionImage.rectTransform.DOKill();

        Vector3 currentRotation = directionImage.rectTransform.rotation.eulerAngles;

        if (animation)
        {
            directionImage.rectTransform.DORotate(
                new Vector3(currentRotation.x, isClockwise ? 360 : 180, currentRotation.z), animationDuration);
        }
        else
        {
            directionImage.rectTransform.rotation =
                Quaternion.Euler(currentRotation.x, isClockwise ? 0 : 180, currentRotation.z);
        }
    }
}