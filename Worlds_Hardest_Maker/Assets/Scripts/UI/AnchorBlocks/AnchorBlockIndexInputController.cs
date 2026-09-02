using TMPro;
using UnityEngine;
using VContainer;

public class AnchorBlockIndexInputController : MonoBehaviour
{
    [SerializeField] private TMP_InputField indexInput;

    // TODO: check if injected
    [Inject] private IAnchorManager anchorManager;
    
    public void OnButtonClick() => AnchorBlockIndexInputEditManager.Instance.StartIndexInputEdit(this);
    
    public void OnValueChanged() => anchorManager.UpdateSelectedAnchorLines();
    
    public void SetIndexValue(int index) => indexInput.text = index.ToString();
}