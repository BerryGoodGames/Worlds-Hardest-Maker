using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MenuManager))]
public class MenuManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ((MenuManager)target).ChangeMenuTab();
    }
}