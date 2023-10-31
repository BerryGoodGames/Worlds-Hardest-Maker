using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class ButtonVerticalArrowTween : MonoBehaviour
{
    [SerializeField] [InitializationField] private Image directionImage;

    [SerializeField] [InitializationField] [PositiveValueOnly] private float animationDuration;

    private bool isUp;

    public bool IsUp
    {
        get => isUp;
        set
        {
            if (value != isUp) Toggle(false);
        }
    }

    [ButtonMethod]
    public void Toggle(bool animation = true)
    {
        isUp = !isUp;

        directionImage.rectTransform.DOKill();

        Vector3 currentRotation = directionImage.rectTransform.rotation.eulerAngles;

        if (animation)
        {
            // this alternates the arrow even though it shouldn't, i have no idea wtf it's 3am just let me sleep unity
            directionImage.rectTransform.DORotate(
                new Vector3(180, currentRotation.y, currentRotation.z), animationDuration
            );
        }
        else
        {
            directionImage.rectTransform.rotation =
                Quaternion.Euler(isUp ? 0 : 180, currentRotation.y, currentRotation.z);
        }
    }
}