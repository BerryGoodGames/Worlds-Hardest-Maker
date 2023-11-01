using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class KeyBinds
{
    // naming convention for key bind is: Category_CamelCaseName
    private static readonly Dictionary<string, List<KeyCode[]>> keyBindToKeyCode = new()
    {
        { "Camera_Pan", new() {new[] {KeyCode.Mouse2, }, } },
        
        { "Movement_Up", new() { new[] { KeyCode.W, }, new[] { KeyCode.UpArrow, }, } },
        { "Movement_Right", new() { new[] { KeyCode.D, }, new[] { KeyCode.RightArrow, }, } },
        { "Movement_Down", new() { new[] { KeyCode.S, }, new[] { KeyCode.DownArrow, }, } },
        { "Movement_Left", new() { new[] { KeyCode.A, }, new[] { KeyCode.LeftArrow, }, } },
        
        {"Editor_Select", new() { new[] { KeyCode.Mouse1, }, }},
        {"Editor_Copy", new() { new[] { KeyCode.LeftControl, KeyCode.C, }, }},
        {"Editor_Paste", new() { new[] { KeyCode.LeftControl, KeyCode.V, }, }},
        {"Editor_Menu", new() { new[] { KeyCode.M, }, }},
        {"Editor_Save", new() { new[] { KeyCode.LeftControl, KeyCode.S, }, }},
        {"Editor_Load", new() { new[] { KeyCode.LeftControl, KeyCode.O, }, }},
        {"Editor_Modify", new() { new[] { KeyCode.LeftControl, }, }},
        {"Editor_MoveEntity", new() { new[] { KeyCode.LeftShift, }, }},
        {"Editor_DeleteEntity", new() { new[] { KeyCode.Delete, }, }},
        {"Editor_Rotate", new() { new[] { KeyCode.R, }, }},
        
        {"EditMode_Delete", new() { new[] { KeyCode.D, }, }},
        {"EditMode_Wall", new() { new[] { KeyCode.W, }, }},
        {"EditMode_Start", new() { new[] { KeyCode.S, }, }},
        {"EditMode_Goal", new() { new[] { KeyCode.G, }, }},
        {"EditMode_Checkpoint", new() { new[] { KeyCode.C, KeyCode.H, }, }},
        {"EditMode_Void", new() { new[] { KeyCode.V, }, }},
        {"EditMode_OneWayGate", new() { new[] { KeyCode.O, }, }},
        {"EditMode_Water", new() { new[] { KeyCode.W, KeyCode.A, }, }},
        {"EditMode_Ice", new() { new[] { KeyCode.I, }, }},
        {"EditMode_Player", new() { new[] { KeyCode.P, }, }},
        {"EditMode_Anchor", new() { new[] { KeyCode.A, }, }},
        {"EditMode_Ball", new() { new[] { KeyCode.B, }, }},
        {"EditMode_Coin", new() { new[] { KeyCode.C, }, }},
        {"EditMode_GrayKey", new() { new[] { KeyCode.K, }, }},
        {"EditMode_RedKey", new() { new[] { KeyCode.K, KeyCode.R, }, }},
        {"EditMode_GreenKey", new() { new[] { KeyCode.K, KeyCode.G, }, }},
        {"EditMode_BlueKey", new() { new[] { KeyCode.K, KeyCode.B, }, }},
        {"EditMode_YellowKey", new() { new[] { KeyCode.K, KeyCode.Y, }, }},
    };

    public static bool GetKeyBind(string keyBindName) => keyBindToKeyCode[keyBindName].Any(combination => combination.All(Input.GetKey));

    public static bool GetKeyBindDown(string keyBindName) => keyBindToKeyCode[keyBindName].Any(combination => combination.All(Input.GetKeyDown));

    public static bool GetKeyBindUp(string keyBindName) => keyBindToKeyCode[keyBindName].Any(combination => combination.All(Input.GetKeyUp));

    public static void ResetKeyBind(string keyBindName) => keyBindToKeyCode[keyBindName].Clear();

    public static void AddKeyCodesToKeyBind(string keyBindName, params KeyCode[][] keyCodes) => keyBindToKeyCode[keyBindName].AddRange(keyCodes);

    public static bool HasKeyBindKeyCode(string keyBindName, KeyCode[] keyCode) => keyBindToKeyCode[keyBindName].Contains(keyCode);

    public static List<KeyBind> GetAllKeyBinds()
    {
        List<KeyBind> keyBinds = new();

        foreach (KeyValuePair<string, List<KeyCode[]>> keyBindPair in keyBindToKeyCode)
        {
            KeyBind keyBind = new(keyBindPair.Key, keyBindPair.Value.ToArray());

            keyBinds.Add(keyBind);
        }

        return keyBinds;
    }
}

public struct KeyBind
{
    public KeyBind(string name, KeyCode[][] keyCodes)
    {
        Name = name;
        KeyCodes = keyCodes;
    }

    public string Name;
    public KeyCode[][] KeyCodes;

    public string Category
    {
        get
        {
            // check if there is a category
            if (!Name.Contains('_')) throw new($"There was no category in key bind {Name}");

            // return everything before the underscore
            return Name.Split('_')[0];
        }
    }

    public string FormattedName
    {
        get
        {
            // check if there is a category
            if (!Name.Contains('_')) throw new($"There was no category in key bind {Name}");

            // remove category
            string name = Name.Split('_')[1];

            // add spaces before capital letters
            for (int i = 1; i < name.Length; i++)
            {
                if (!char.IsUpper(Name[i])) continue;

                name = name.Insert(i, " ");
            }

            return name;
        }
    }

    public static implicit operator string(KeyBind keyBind) => keyBind.Name;
}