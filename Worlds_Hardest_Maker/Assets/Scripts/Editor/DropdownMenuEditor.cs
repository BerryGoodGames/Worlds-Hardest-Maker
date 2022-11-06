using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DropdownMenu))]
public class DropdownMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DropdownMenu menu = (DropdownMenu)target;

        base.OnInspectorGUI();

        // calculate line sizes + arrow scale
        float mLineSize = menu.originalLineSize / menu.originalHeight;
        float mBaseLineSize = menu.originalBaseLineSize / menu.originalHeight;
        float mArrowScl = menu.originalArrowScl / menu.originalHeight;
        float mArrowDist = menu.originalArrowDist / menu.originalHeight;

        float height = menu.height;

        float newLineSize = mLineSize * height;
        float newBaseLineSize = mBaseLineSize * height;

        BackgroundLineSize lineSizeController = menu.bgLineSize;

        lineSizeController.SetLineSize(newLineSize);
        lineSizeController.SetLineSizeBottom(newBaseLineSize);

        menu.arrowRt.localScale = height * mArrowScl * Vector2.one;
        menu.arrowRt.anchoredPosition = new(mArrowDist * height, menu.arrowRt.anchoredPosition.y);

        menu.rt.sizeDelta = new(menu.rt.rect.width, height);
    }
}
