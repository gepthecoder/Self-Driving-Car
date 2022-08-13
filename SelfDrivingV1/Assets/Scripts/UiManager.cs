using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;

    [SerializeField] private Text m_CurrentGenerationText;
    [SerializeField] private Text m_CurrentGenomeText;

    [SerializeField] private Text m_SteeringText;
    [SerializeField] private Text m_AccelerationText;

    [SerializeField] private Text m_OverallFitnessText;
    [SerializeField] private Text m_SpeedText;

    [SerializeField] private Text m_TimeScaleText;

    private void Awake()
    {
        instance = this;
    }

    public void SetCurrentGenerationText(string val)
    {
        m_CurrentGenerationText.text = $"Current Generation: {val}";
    }

    public void SetCurrentGenomeText(string val)
    {
        m_CurrentGenomeText.text = $"Current Genome: {val}";
    }

    public void SetSteeringText(string val)
    {
        m_SteeringText.text = $"Steering: {val}";
    }

    public void SetAccelerationText(string val)
    {
        m_AccelerationText.text = $"Acceleration: {val}";
    }

    public void SetOverallFitnessText(string val)
    {
        m_OverallFitnessText.text = $"Fitness: {val}";
    }

    public void SetSpeedText(string val)
    {
        m_SpeedText.text = $"Avg Speed: {val}";
    }

    public void SetTimeScaleText(string val)
    {
        m_TimeScaleText.text = $"Time Scale: {val}";
    }

}
