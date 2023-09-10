using System.Globalization;
using MyBox;
using TMPro;
using UnityEngine;

public class AnchorBlockPositionInputController : MonoBehaviour
{
    [InitializationField] public TMP_InputField InputX;
    [InitializationField] public TMP_InputField InputY;

    public void OnValueChanged() => AnchorManager.Instance.UpdateSelectedAnchorLines();

    public void OnButtonClicked() => AnchorPositionInputEditManager.Instance.StartPositionInputEdit(this);

    public void SetPositionValues(float x, float y)
    {
        InputX.text = x.ToString();
        InputY.text = y.ToString();
    }

    public void SetPositionValues(Vector2 position) => SetPositionValues(position.x, position.y);

    public Vector2 GetPositionValues() =>
        new(
            float.Parse(InputX.text, CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(InputY.text, CultureInfo.InvariantCulture.NumberFormat)
        );
}