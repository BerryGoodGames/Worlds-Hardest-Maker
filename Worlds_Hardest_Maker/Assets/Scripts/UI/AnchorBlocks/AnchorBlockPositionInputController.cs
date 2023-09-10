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

    public void SetPositionValues(Vector2 position)
    {
        // subtract anchor origin point
        position -= (Vector2)AnchorManager.Instance.SelectedAnchor.transform.position;

        InputX.text = position.x.ToString();
        InputY.text = position.y.ToString();
    }

    public Vector2 GetPositionValues() =>
        new(
            float.Parse(InputX.text, CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(InputY.text, CultureInfo.InvariantCulture.NumberFormat)
        );
}