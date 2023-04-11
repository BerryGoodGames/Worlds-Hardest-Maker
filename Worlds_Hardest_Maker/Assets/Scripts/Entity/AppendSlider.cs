using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     Script for e.g. balls or player
///     to add a slider following attached gameObject
///     and splitting gameObject and slider
/// </summary>
public class AppendSlider : MonoBehaviour
{
    // specific slider to append
    [SerializeField] private float min, max, step, startValue;
    [SerializeField] private GameObject sliderPrefab;

    public float Min
    {
        get => min;
        set => min = value;
    }

    public float Max
    {
        get => max;
        set => max = value;
    }

    public float Step
    {
        get => step;
        set => step = value;
    }

    public float StartValue
    {
        get => startValue;
        set => startValue = value;
    }

    public GameObject Slider { get; private set; }

    private void Awake()
    {
        Slider = Instantiate(sliderPrefab, Vector2.zero, Quaternion.identity,
            ReferenceManager.Instance.SliderContainer);
        Slider settings = Slider.GetComponent<Slider>();
        settings.minValue = min / step;
        settings.maxValue = max / step;
        settings.value = startValue / step;
    }

    /// <returns>final value of slider</returns>
    public float GetValue() => Slider.GetComponent<Slider>().value * step;

    /// <returns>Slider component from slider</returns>
    public Slider GetSlider() => Slider.GetComponent<Slider>();

    /// <returns>slider object</returns>
    public GameObject GetSliderObject() => Slider;
}