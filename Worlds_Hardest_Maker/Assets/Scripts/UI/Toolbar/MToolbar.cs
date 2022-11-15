using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MToolbar : MonoBehaviour
{
    public GameObject toolbarBackground;
    public GameObject infobarBackground;

    public static GameObject[] tools;
    void Start()
    {
        tools = GameObject.FindGameObjectsWithTag("Tool");

        // editmode wall when starting
        transform.GetChild(0).GetComponent<Tool>().SwitchGameMode();
    }

    public static void DeselectAll()
    {
        for(int i = 0; i < tools.Length; i++)
        {
            Tool tool = tools[i].GetComponent<Tool>();
            tool.Selected(false);
        }
    }
}
