using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberInputOption : SettingOption
{
    [SerializeField] private float width = 250;
    public float Width { get { return width; } set { width = value; } }
    [Space]
    [SerializeField] private RectTransform numberInputObjRt;

    public override void Response()
    {
        base.Response();

        numberInputObjRt.sizeDelta = new(width, 0);
    }
}
