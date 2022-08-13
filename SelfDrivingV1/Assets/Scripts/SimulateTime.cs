using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SimulateTime : MonoBehaviour
{
    [SerializeField] private Slider m_TimeScaleSlider;

    private const float c_DefaultScale = 1f;

    private void Start()
    {
        DefaultState(c_DefaultScale);

        m_TimeScaleSlider.onValueChanged.AddListener(delegate { 
            SetCurrentTimeScale(m_TimeScaleSlider.value);
            UpdateText(m_TimeScaleSlider.value);
        });
    }

    private void DefaultState(float c_DefaultScale)
    {
        SetCurrentTimeScale(c_DefaultScale);
        UpdateText(c_DefaultScale);
        m_TimeScaleSlider.value = c_DefaultScale;
    }

    private void OnDisable()
    {
        m_TimeScaleSlider.onValueChanged.RemoveAllListeners();
    }

    private void SetCurrentTimeScale(float val)
    {
        Time.timeScale = Mathf.Clamp(val, m_TimeScaleSlider.minValue, m_TimeScaleSlider.maxValue);
    }

    private void UpdateText(float val)
    {
        UiManager.instance.SetTimeScaleText(val.ToString());
    }
}
