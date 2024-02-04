using System;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public static UndoManager Instance { get; private set; }

    public Stack<List<Data>> GameDataStack;

    private void Start()
    {
        LevelSessionEditManager.Instance.OnEditAction += PushCurrentGameData;
        LevelSessionManager.Instance.OnLevelLoaded += PushCurrentGameData;
    }

    private void Undo()
    {
        Stack<List<Data>> gameDataStack = new Stack<List<Data>>(new Stack<List<Data>>(GameDataStack));

        // remove current game data to get previous
        gameDataStack.Pop();

        List<Data> targetData = gameDataStack.Pop();
        GameManager.Instance.LoadLevelFromDataRaw(targetData);
    }

    public void PushCurrentGameData()
    {
        GameDataStack.Push(SaveSystem.SerializeCurrentLevel());
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
