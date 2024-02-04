using System;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public static UndoManager Instance { get; private set; }

    public Stack<List<Data>> GameDataStack;

    private void Start()
    {
        LevelSessionEditManager.Instance.OnEditAction += () =>
        {
            GameDataStack.Push(SaveSystem.SerializeCurrentLevel());
        };
    }

    private void Undo()
    {
        print("Hello");
        Stack<List<Data>> gameDataStack = GameDataStack;
        gameDataStack.Clear();
        
        print("");
    }

    private void Update()
    {
        if (KeyBinds.GetKeyBindDown("Editor_Undo")) Undo();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        GameDataStack = new();
    }
}
