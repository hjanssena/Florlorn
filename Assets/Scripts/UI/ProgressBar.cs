using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetMax(float max)
    {
        slider.maxValue = max;
    }

    public void ChangeProgress(float newProgress)
    {
        slider.value = newProgress;
    }
}
