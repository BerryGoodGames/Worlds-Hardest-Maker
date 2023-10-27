using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using MyBox;
using TMPro;
using UnityEngine;

public class KeyBindSetterController : MonoBehaviour
{
    [HideInInspector] public KeyBind KeyBind;

    [Separator("References")] 
    [SerializeField] private TMP_Text keyBindName;
    [SerializeField] private Transform displayContainer;
    
    public void AddKeyCode(KeyCode keyCode)
    {
        KeyBinds.AddKeyCodesToKeyBind(KeyBind, keyCode);
    }

    private void Start()
    {
        keyBindName.text = KeyBind.FormattedName;
    }
}
