using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericBar : MonoBehaviour
{

    public Slider slider;

    public void SetMax(float maxValue, float initialValue)
    {
        slider.maxValue = maxValue;
        slider.value = initialValue;
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }
}
