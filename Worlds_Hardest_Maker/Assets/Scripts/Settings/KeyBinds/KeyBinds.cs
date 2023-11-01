using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class KeyBinds
{
    // naming convention for key bind is: Category_CamelCaseName
    public static Dictionary<string, List<KeyCode[]>> KeyBindToKeyCode { get; } = new()
    {
        { "Movement_Up", new() { new[] { KeyCode.A, KeyCode.W }, new[] { KeyCode.UpArrow }, } },
        { "Movement_Right", new() { new[] { KeyCode.D }, new[] { KeyCode.RightArrow }, } },
        { "Movement_Down", new() { new[] { KeyCode.S }, new[] { KeyCode.DownArrow }, } },
        { "Movement_Left", new() { new[] { KeyCode.A }, new[] { KeyCode.LeftArrow }, } },
    };

    public static bool GetKeyBind(string keyBindName) => KeyBindToKeyCode[keyBindName].Any(combination => combination.All(Input.GetKey));

    public static bool GetKeyBindDown(string keyBindName) => KeyBindToKeyCode[keyBindName].Any(combination => combination.All(Input.GetKeyDown));

    public static bool GetKeyBindUp(string keyBindName) => KeyBindToKeyCode[keyBindName].Any(combination => combination.All(Input.GetKeyUp));

    public static void ResetKeyBind(string keyBindName) => KeyBindToKeyCode[keyBindName].Clear();

    public static void AddKeyCodesToKeyBind(string keyBindName, params KeyCode[][] keyCodes) => KeyBindToKeyCode[keyBindName].AddRange(keyCodes);

    public static bool HasKeyBindKeyCode(string keyBindName, KeyCode[] keyCode) => KeyBindToKeyCode[keyBindName].Contains(keyCode);

    public static List<KeyBind> GetAllKeyBinds()
    {
        List<KeyBind> keyBinds = new();

        foreach (KeyValuePair<string, List<KeyCode[]>> keyBindPair in KeyBindToKeyCode)
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