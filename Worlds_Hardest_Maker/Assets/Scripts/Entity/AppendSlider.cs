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
    [SerializeField] private float min, max, step, startValue;
    public float Min { get { return min; } set { min = value; } }
    public float Max { get { return max; } set { max = value; } }
    public float Step { get { return step; } set { step = value; } }
    public float StartValue { get { return startValue; } set { startValue = value; } }
    [SerializeField] private GameObject sliderPrefab;
    public GameObject Slider { get; private set; }

    private void Awake()
    {
        Slider = Instantiate(sliderPrefab, Vector2.zero, Quaternion.identity, MGame.Instance.SliderContainer.transform);
        Slider settings = Slider.GetComponent<Slider>();
        settings.minValue = min / step;
        settings.maxValue = max / step;
        settings.value = startValue / step;
    }

    /// <returns>final value of slider</returns>
    public float GetValue()
    {
        return Slider.GetComponent<Slider>().value * step;
    }

    /// <returns>Slider component from slider</returns>
    public Slider GetSlider()
    {
        return Slider.GetComponent<Slider>();
    }

    /// <returns>slider object</returns>
    public GameObject GetSliderObject()
    {
        return Slider;
    }
}
