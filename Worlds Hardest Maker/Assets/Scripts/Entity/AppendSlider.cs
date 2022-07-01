using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script for e.g. balls or player
/// to add a slider following attached gameobject
/// and splitting gameobject and slider
/// </summary>
public class AppendSlider : MonoBehaviour
{
    // specific slider to append
    [SerializeField] private GameObject sliderPrefab;
    public float min, max, step, startValue;
    [HideInInspector] public GameObject slider;

    private void Awake()
    {
        slider = Instantiate(sliderPrefab, Vector2.zero, Quaternion.identity, GameManager.Instance.SliderContainer.transform);
        Slider settings = slider.GetComponent<Slider>();
        settings.minValue = min / step;
        settings.maxValue = max / step;
        settings.value = startValue / step;
    }

    /// <returns>final value of slider</returns>
    public float GetValue()
    {
        return slider.GetComponent<Slider>().value * step;
    }

    /// <returns>Slider component from slider</returns>
    public Slider GetSlider()
    {
        return slider.GetComponent<Slider>();
    }

    /// <returns>slider object</returns>
    public GameObject GetSliderObject()
    {
        return slider;
    }
}
