using TMPro;
using UnityEngine;

public class AnchorBlockIndexInputController : MonoBehaviour
{
    [SerializeField] private TMP_InputField indexInput;

    public void OnButtonClick() => AnchorBlockIndexInputEditManager.Instance.StartIndexInputEdit(this);

    public void OnValueChanged() => AnchorManager.Instance.UpdateSelectedAnchorLines();

    public void SetIndexValue(int index) => indexInput.text = index.ToString();
}