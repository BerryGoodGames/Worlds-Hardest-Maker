using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color deselectedColor = Color.black;
    [SerializeField] private Color selectedColor = Color.white;
    private Toggle toggle;
    private bool prevIsOn;

    private void Awake() => toggle = GetComponent<Toggle>();

    private void Start() => label.color = toggle.isOn ? selectedColor : deselectedColor;

    private void Update()
    {
        if (toggle.isOn == prevIsOn) return;

        label.color = toggle.isOn ? selectedColor : deselectedColor;
    }

    private void LateUpdate() => prevIsOn = toggle.isOn;
}