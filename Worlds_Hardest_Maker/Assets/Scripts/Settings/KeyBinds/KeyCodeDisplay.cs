using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

public class KeyCodeDisplay : MonoBehaviour
{
    [Separator("References")]
    [SerializeField] [InitializationField] [MustBeAssigned] private Transform keyCodeImageContainer;
    [SerializeField] [InitializationField] [MustBeAssigned] private Image keyCodeImage;

    [SerializeField] [InitializationField] [MustBeAssigned] private Transform separator;
    

    #region Key Code Sprites

    [Foldout("Key Code Spites")] [SerializeField] private Sprite key0;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite key1;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite key2;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite key3;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite key4;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite key5;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite key6;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite key7;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite key8;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite key9;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyA;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyAlt;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyArrowDown;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyArrowLeft;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyArrowRight;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyArrowUp;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyAsterisk;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyB;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyBackspace;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyBracketLeft;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyBracketRight;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyC;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyCapsLock;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyCommand;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyCtrl;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyD;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyDel;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyE;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyEnd;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyEnter;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyEnterTall;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyEsc;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF1;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF2;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF3;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF4;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF5;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF6;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF7;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF8;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF9;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF10;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF11;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF12;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyF;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyG;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyH;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyHome;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyI;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyInsert;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyJ;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyK;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyL;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyM;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyMarkLeft;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyMarkRight;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyMinus;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyMouseLeft;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyMouseMiddle;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyMouseRight;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyN;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyNumLock;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyO;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyP;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyPageDown;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyPageUp;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyPlus;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyPlusTall;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyPrint;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyQ;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyQuestion;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyQuote;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyR;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyS;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keySemicolon;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyShiftAlt;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyShift;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keySlash;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keySpace;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyT;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyTab;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyTilda;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyU;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyV;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyW;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyWin;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyX;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyY;
    [Foldout("Key Code Spites")] [SerializeField] private Sprite keyZ;

    #endregion

    private Dictionary<KeyCode, Sprite> keyCodeToSprite;

    private void Awake()
    {
        keyCodeToSprite = new()
        {
            #region Key Code to Sprite

            { KeyCode.Alpha0, key0 },
            { KeyCode.Alpha1, key1 },
            { KeyCode.Alpha2, key2 },
            { KeyCode.Alpha3, key3 },
            { KeyCode.Alpha4, key4 },
            { KeyCode.Alpha5, key5 },
            { KeyCode.Alpha6, key6 },
            { KeyCode.Alpha7, key7 },
            { KeyCode.Alpha8, key8 },
            { KeyCode.Alpha9, key9 },
            { KeyCode.A, keyA },
            { KeyCode.LeftAlt, keyAlt },
            { KeyCode.RightAlt, keyAlt },
            { KeyCode.DownArrow, keyArrowDown },
            { KeyCode.LeftArrow, keyArrowLeft },
            { KeyCode.RightArrow, keyArrowRight },
            { KeyCode.UpArrow, keyArrowUp },
            { KeyCode.Asterisk, keyAsterisk },
            { KeyCode.B, keyB },
            { KeyCode.Backspace, keyBackspace },
            { KeyCode.LeftBracket, keyBracketLeft },
            { KeyCode.RightBracket, keyBracketRight },
            { KeyCode.C, keyC },
            { KeyCode.CapsLock, keyCapsLock },
            { KeyCode.LeftCommand, keyCommand },
            { KeyCode.RightCommand, keyCommand },
            { KeyCode.LeftControl, keyCtrl },
            { KeyCode.RightControl, keyCtrl },
            { KeyCode.D, keyD },
            { KeyCode.Delete, keyDel },
            { KeyCode.E, keyE },
            { KeyCode.End, keyEnd },
            { KeyCode.KeypadEnter, keyEnterTall },
            { KeyCode.Escape, keyEsc },
            { KeyCode.F1, keyF1 },
            { KeyCode.F2, keyF2 },
            { KeyCode.F3, keyF3 },
            { KeyCode.F4, keyF4 },
            { KeyCode.F5, keyF5 },
            { KeyCode.F6, keyF6 },
            { KeyCode.F7, keyF7 },
            { KeyCode.F8, keyF8 },
            { KeyCode.F9, keyF9 },
            { KeyCode.F10, keyF10 },
            { KeyCode.F11, keyF11 },
            { KeyCode.F12, keyF12 },
            { KeyCode.F, keyF },
            { KeyCode.G, keyG },
            { KeyCode.H, keyH },
            { KeyCode.Home, keyHome },
            { KeyCode.I, keyI },
            { KeyCode.Insert, keyInsert },
            { KeyCode.J, keyJ },
            { KeyCode.K, keyK },
            { KeyCode.L, keyL },
            { KeyCode.M, keyM },
            { KeyCode.Less, keyMarkLeft },
            { KeyCode.Greater, keyMarkRight },
            { KeyCode.Minus, keyMinus },
            { KeyCode.Mouse0, keyMouseLeft },
            { KeyCode.Mouse1, keyMouseRight },
            { KeyCode.Mouse3, keyMouseMiddle },
            { KeyCode.N, keyN },
            { KeyCode.Numlock, keyNumLock },
            { KeyCode.O, keyO },
            { KeyCode.P, keyP },
            { KeyCode.PageDown, keyPageDown },
            { KeyCode.PageUp, keyPageUp },
            { KeyCode.Plus, keyPlus },
            { KeyCode.KeypadPlus, keyPlusTall },
            { KeyCode.Print, keyPrint },
            { KeyCode.Q, keyQ },
            { KeyCode.Question, keyQuestion },
            { KeyCode.Quote, keyQuote },
            { KeyCode.R, keyR },
            { KeyCode.S, keyS },
            { KeyCode.Semicolon, keySemicolon },
            { KeyCode.LeftShift, keyShiftAlt },
            { KeyCode.RightShift, keyShift },
            { KeyCode.Slash, keySlash },
            { KeyCode.Space, keySpace },
            { KeyCode.T, keyT },
            { KeyCode.Tab, keyTab },
            { KeyCode.Tilde, keyTilda },
            { KeyCode.U, keyU },
            { KeyCode.V, keyV },
            { KeyCode.W, keyW },
            { KeyCode.LeftWindows, keyWin },
            { KeyCode.RightWindows, keyWin },
            { KeyCode.X, keyX },
            { KeyCode.Y, keyY },
            { KeyCode.Z, keyZ },

            #endregion
        };
    }

    public void SetKeyCodeSprite(KeyCode[] keyCodes)
    {
        if (keyCodes.Length <= 0) throw new("There has to be at least one key code");
        
        // destroy the children MUGUHUAHAHAHAGAAGAGGAGAAGAGAGAGGAGAGAGAHAHAHAHAHAHAHHASJHHAHASHYHHAHHAJHHAHAHA
        foreach (Transform child in keyCodeImageContainer)
        {
            Destroy(child.gameObject);
        }
        
        // create first image
        CreateKeyCodeImage(keyCodes[0]);
        
        // create other images with separator
        for (int i = 1; i < keyCodes.Length; i++)
        {
            Instantiate(separator, keyCodeImageContainer);
            CreateKeyCodeImage(keyCodes[i]);
        }

        return;

        void CreateKeyCodeImage(KeyCode keyCode)
        {
            Image image = Instantiate(keyCodeImage, keyCodeImageContainer);
            image.sprite = keyCodeToSprite[keyCode];
        }
    }
}