using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CItem : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text label;
    [SerializeField] private Color deselectedColor = Color.black;
    [SerializeField] private Color selectedColor = Color.white;
    private Toggle toggle;
    private bool prevIsOn;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    private void Start()
    {
        if (toggle.isOn) label.color = selectedColor;

        else label.color = deselectedColor;
    }

    private void Update()
    {
        if (toggle.isOn != prevIsOn)
        {
            if (toggle.isOn) label.color = selectedColor;

            else label.color = deselectedColor;
        }
    }

    private void LateUpdate()
    {
        prevIsOn = toggle.isOn;
    }

}