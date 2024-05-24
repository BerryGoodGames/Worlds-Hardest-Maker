using System.Collections.Generic;
using MyBox;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class KeyCodeDisplay : MonoBehaviour
{
    [Separator("References")] [SerializeField] [InitializationField] [Required] private Transform keyCodeImageContainer;
    [SerializeField] [InitializationField] [Required] private Image keyCodeImage;
    
    [SerializeField] [InitializationField] [Required] private Transform separator;
    
    
    #region Key Code Sprites
    
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite key0;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite key1;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite key2;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite key3;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite key4;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite key5;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite key6;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite key7;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite key8;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite key9;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyA;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyAlt;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyArrowDown;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyArrowLeft;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyArrowRight;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyArrowUp;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyAsterisk;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyB;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyBackspace;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyBracketLeft;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyBracketRight;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyC;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyCapsLock;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyCommand;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyCtrl;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyD;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyDel;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyE;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyEnd;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyEnterTall;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyEsc;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF1;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF2;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF3;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF4;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF5;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF6;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF7;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF8;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF9;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF10;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF11;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF12;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyF;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyG;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyH;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyHome;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyI;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyInsert;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyJ;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyK;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyL;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyM;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyMarkLeft;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyMarkRight;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyMinus;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyMouseLeft;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyMouseMiddle;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyMouseRight;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyN;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyNumLock;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyO;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyP;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyPageDown;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyPageUp;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyPlus;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyPlusTall;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyPrint;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyQ;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyQuestion;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyQuote;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyR;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyS;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keySemicolon;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyShiftAlt;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyShift;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keySlash;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keySpace;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyT;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyTab;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyTilda;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyU;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyV;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyW;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyWin;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyX;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyY;
    [MyBox.Foldout("Key Code Spites")] [SerializeField] private Sprite keyZ;
    
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
            { KeyCode.Mouse2, keyMouseMiddle },
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
        foreach (Transform child in keyCodeImageContainer) Destroy(child.gameObject);
        
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
            
            if (keyCodeToSprite.TryGetValue(keyCode, out Sprite sprite)) image.sprite = sprite;
            else Debug.LogWarning($"There is no sprite for the key code \"{keyCode}\"");
        }
    }
}