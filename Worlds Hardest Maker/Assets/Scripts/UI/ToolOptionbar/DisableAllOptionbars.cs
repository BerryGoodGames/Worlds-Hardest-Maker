using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAllOptionbars : MonoBehaviour
{
    public void DisableOptionbars()
    {
        GameManager.DisableAllOptionbars();
    }
    public void EnableOptionbars()
    {
        GameManager.EnableAllOptionbars();
    }
}
