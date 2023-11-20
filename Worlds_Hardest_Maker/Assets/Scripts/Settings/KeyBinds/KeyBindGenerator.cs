using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class KeyBindGenerator : MonoBehaviour
{
    [Separator("References")] [SerializeField] private TMP_Text categoryHeader;
    [SerializeField] private KeyBindSetterController keyBindSetter;

    private void Start()
    {
        #if UNITY_EDITOR
        // change ctrl to shift in unity editor because of conflicts with the uniteh editor
        KeyBinds.ReplaceKeyCode(KeyCode.LeftControl, KeyCode.Tab);
        #endif
        GenerateKeyBindSetters();
    }

    private void GenerateKeyBindSetters()
    {
        // get categories and key binds
        List<KeyBind> keyBinds = KeyBinds.GetAllKeyBinds();

        string currentCategory = string.Empty;

        foreach (KeyBind keyBind in keyBinds)
        {
            if (keyBind.Category == "Hidden") continue;

            if (currentCategory != keyBind.Category)
            {
                currentCategory = keyBind.Category;
                TMP_Text header = Instantiate(categoryHeader, transform);
                header.text = keyBind.Category;
            }

            KeyBindSetterController setterController = Instantiate(keyBindSetter, transform);
            setterController.KeyBind = keyBind;
        }
    }
}