using System;
using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindSetterController : MonoBehaviour
{
    [HideInInspector] public KeyBind KeyBind;

    [Separator("References")] [SerializeField] private TMP_Text keyBindName;
    [SerializeField] private RectTransform displayContainer;

    private void AddKeyCode(KeyCode keyCode)
    {
        if (KeyBinds.HasKeyBindKeyCode(KeyBind, keyCode)) return;

        KeyBinds.AddKeyCodesToKeyBind(KeyBind, keyCode);

        InstantiateKeyCodeDisplay(keyCode);

        print($"Successfully added key code {keyCode} to {keyBindName.text}");
    }

    public void AddKeyCode(string keyCodeName)
    {
        #region charToKeycode

        //NOTE: This is only a DICTIONARY with MOST character to keycode bindings... it is NOT a working cs file
        //ITS USEFUL: when you are reading in your control scheme from a file

        //NOTE: some characters SHOULD map to multiple keycodes (but this is impossible)
        //since this is a dictionary, only 1 character is bound to 1 keycode
        //EX: * from the keyboard will be read the same as * from the keypad... because they produce the same character in a text file

        Dictionary<char, KeyCode> charToKeycode = new()
        {
            //-------------------------LOGICAL mappings-------------------------

            //Lower Case Letters
            { 'a', KeyCode.A },
            { 'b', KeyCode.B },
            { 'c', KeyCode.C },
            { 'd', KeyCode.D },
            { 'e', KeyCode.E },
            { 'f', KeyCode.F },
            { 'g', KeyCode.G },
            { 'h', KeyCode.H },
            { 'i', KeyCode.I },
            { 'j', KeyCode.J },
            { 'k', KeyCode.K },
            { 'l', KeyCode.L },
            { 'm', KeyCode.M },
            { 'n', KeyCode.N },
            { 'o', KeyCode.O },
            { 'p', KeyCode.P },
            { 'q', KeyCode.Q },
            { 'r', KeyCode.R },
            { 's', KeyCode.S },
            { 't', KeyCode.T },
            { 'u', KeyCode.U },
            { 'v', KeyCode.V },
            { 'w', KeyCode.W },
            { 'x', KeyCode.X },
            { 'y', KeyCode.Y },
            { 'z', KeyCode.Z },

            //KeyPad Numbers
            { '1', KeyCode.Keypad1 },
            { '2', KeyCode.Keypad2 },
            { '3', KeyCode.Keypad3 },
            { '4', KeyCode.Keypad4 },
            { '5', KeyCode.Keypad5 },
            { '6', KeyCode.Keypad6 },
            { '7', KeyCode.Keypad7 },
            { '8', KeyCode.Keypad8 },
            { '9', KeyCode.Keypad9 },
            { '0', KeyCode.Keypad0 },

            //Other Symbols
            { '!', KeyCode.Exclaim }, //1
            { '"', KeyCode.DoubleQuote },
            { '#', KeyCode.Hash }, //3
            { '$', KeyCode.Dollar }, //4
            { '&', KeyCode.Ampersand }, //7
            { '\'', KeyCode.Quote }, //remember the special forward slash rule... this isnt wrong
            { '(', KeyCode.LeftParen }, //9
            { ')', KeyCode.RightParen }, //0
            { '*', KeyCode.Asterisk }, //8
            { '+', KeyCode.Plus },
            { ',', KeyCode.Comma },
            { '-', KeyCode.Minus },
            { '.', KeyCode.Period },
            { '/', KeyCode.Slash },
            { ':', KeyCode.Colon },
            { ';', KeyCode.Semicolon },
            { '<', KeyCode.Less },
            { '=', KeyCode.Equals },
            { '>', KeyCode.Greater },
            { '?', KeyCode.Question },
            { '@', KeyCode.At }, //2
            { '[', KeyCode.LeftBracket },
            { '\\', KeyCode.Backslash }, //remember the special forward slash rule... this isnt wrong
            { ']', KeyCode.RightBracket },
            { '^', KeyCode.Caret }, //6
            { '_', KeyCode.Underscore },
            { '`', KeyCode.BackQuote },

            //-------------------------NON-LOGICAL mappings-------------------------

            //NOTE: all of these can easily be remapped to something that perhaps you find more useful

            //---Mappings where the logical keycode was taken up by its counter part in either (the regular keybaord) or the (keypad)

            //Alpha Numbers
            //NOTE: we are using the UPPER CASE LETTERS Q -> P because they are nearest to the Alpha Numbers
            { 'Q', KeyCode.Alpha1 },
            { 'W', KeyCode.Alpha2 },
            { 'E', KeyCode.Alpha3 },
            { 'R', KeyCode.Alpha4 },
            { 'T', KeyCode.Alpha5 },
            { 'Y', KeyCode.Alpha6 },
            { 'U', KeyCode.Alpha7 },
            { 'I', KeyCode.Alpha8 },
            { 'O', KeyCode.Alpha9 },
            { 'P', KeyCode.Alpha0 },

            //INACTIVE since I am using these characters else where
            { 'A', KeyCode.KeypadPeriod },
            { 'B', KeyCode.KeypadDivide },
            { 'C', KeyCode.KeypadMultiply },
            { 'D', KeyCode.KeypadMinus },
            { 'F', KeyCode.KeypadPlus },
            { 'G', KeyCode.KeypadEquals },

            //-------------------------CHARACTER KEYS with NO KEYCODE-------------------------

            //NOTE: you can map these to any of the OPEN KEYCODES below

            /*
            //Upper Case Letters (16)
            {'H', -},
            {'J', -},
            {'K', -},
            {'L', -},
            {'M', -},
            {'N', -},
            {'S', -},
            {'V', -},
            {'X', -},
            {'Z', -}
            */

            //-------------------------KEYCODES with NO CHARACER KEY-------------------------

            //-----KeyCodes without Logical Mappings
            //-Anything above "KeyCode.Space" in Unity's Documentation (9 KeyCodes)
            //-Anything between "KeyCode.UpArrow" and "KeyCode.F15" in Unity's Documentation (24 KeyCodes)
            //-Anything Below "KeyCode.Numlock" in Unity's Documentation [(28 KeyCodes) + (9 * 20 = 180 JoyStickCodes) = 208 KeyCodes]

            //-------------------------other-------------------------

            //-----KeyCodes that are inaccesible for some reason
            //{'~', KeyCode.tilde},
            //{'{', KeyCode.LeftCurlyBrace}, 
            //{'}', KeyCode.RightCurlyBrace}, 
            //{'|', KeyCode.Line},   
            //{'%', KeyCode.percent},
        };

        #endregion

        if (keyCodeName.Length > 1)
        {
            Debug.LogWarning("This keyCode has a name which is longer than 1 character huh weird whatever we just ignored it");
            return;
        }

        char keyCodeChar = keyCodeName[0];

        try { AddKeyCode(charToKeycode[keyCodeChar]); }
        catch { print($"Couldn't parse {keyCodeChar} into KeyCode, ignored"); }
    }

    public void OnAddButtonClick()
    {
        MenuManager.Instance.IsAddingKeyBind = true;
        MenuManager.Instance.AddingKeyBindSetter = this;

        ReferenceManager.Instance.KeybindBlocker.SetVisible(true);
        ReferenceManager.Instance.KeybindBlockerText.text = keyBindName.text;
    }

    public void OnClearButtonClick()
    {
        KeyBind.KeyCodes = Array.Empty<KeyCode>();
        KeyBinds.ResetKeyBind(KeyBind);

        foreach (RectTransform keyCodeDisplay in displayContainer) { Destroy(keyCodeDisplay.gameObject); }
    }

    public static void CancelAddingKeyBind()
    {
        MenuManager.Instance.IsAddingKeyBind = false;
        MenuManager.Instance.AddingKeyBindSetter = default;
        ReferenceManager.Instance.KeybindBlocker.SetVisible(false);
    }

    private void Start()
    {
        keyBindName.text = KeyBind.FormattedName;

        SetupInitKeyCodes();
    }

    private void SetupInitKeyCodes() => KeyBind.KeyCodes.ForEach(InstantiateKeyCodeDisplay);

    private void InstantiateKeyCodeDisplay(KeyCode keyCode)
    {
        // instantiate key code display
        KeyCodeDisplay keyCodeDisplay = Instantiate(PrefabManager.Instance.KeyCodeDisplay, displayContainer);
        keyCodeDisplay.SetKeyCodeSprite(keyCode);

        // rebuild
        LayoutRebuilder.ForceRebuildLayoutImmediate(displayContainer);
    }
}