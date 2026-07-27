using UnityEngine;

[CreateAssetMenu(fileName = "NewToastTheme", menuName = "ScriptableObjects/ToastTheme")]
public class ToastTheme : ScriptableObject
{
    public Sprite Icon;
    public Color Color;
}