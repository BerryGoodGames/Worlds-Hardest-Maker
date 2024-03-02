using System;
using MyBox;
using UnityEngine;

public class LevelSessionSettingsSetup : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private SettingsManager settingsManager;
    [SerializeField] [InitializationField] [MustBeAssigned] private ToolbarSizing toolbarSpacing;
    [SerializeField] [InitializationField] [MustBeAssigned] private InfobarResize infobarPlayResize;
    [SerializeField] [InitializationField] [MustBeAssigned] private InfobarResize infobarEditResize;

    private void SetToolbarSize(float size)
    {
        toolbarSpacing.ToolbarHeight = size;
        toolbarSpacing.UpdateSize();
    }

    private void SetInfobarSize(float size)
    {
        infobarPlayResize.InfobarHeight = size;
        infobarEditResize.InfobarHeight = size;
        infobarPlayResize.UpdateSize();
        infobarEditResize.UpdateSize();
    }

    private void Start()
    {
        settingsManager.LoadPrefs();
    }

    private void Awake()
    {
        settingsManager.OnSetToolbarSize += SetToolbarSize;
        settingsManager.OnSetInfobarSize += SetInfobarSize;
    }

    private void OnDestroy()
    {
        settingsManager.OnSetInfobarSize -= SetToolbarSize;
        settingsManager.OnSetInfobarSize -= SetInfobarSize;
    }
}
