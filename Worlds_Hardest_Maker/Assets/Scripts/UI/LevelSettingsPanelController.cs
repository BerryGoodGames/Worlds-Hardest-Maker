using UnityEngine;

public class LevelSettingsPanelController : MonoBehaviour
{
    [SerializeField] private PanelTween levelSettingsPanelTween;
    [SerializeField] private PanelTween levelSettingsButtonPanelTween;

    public void ToggleTween()
    {
        if (levelSettingsButtonPanelTween.Open) levelSettingsPanelTween.Toggle();
    }
}