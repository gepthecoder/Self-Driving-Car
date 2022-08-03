using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("C A R  A I  C O R E")]
    [Space(10)]
    [Range(-1, 1)]
    [SerializeField] private float m_Acceleration;
    [Range(-1, 1)]
    [SerializeField] private float m_TurningVal;

    [SerializeField] private float m_TimeSinceStart = 0f; // checking for idle cars / usless cars -> Reset

    [Header("Fitness -> How Far? How Fast? Whats Valuable? Gives importance to each factor. Example: If the two cars go the exact same distance, how do we differentiate which one is better?")]
    [SerializeField] private float m_OverallFitness;
    [Tooltip("How important is the DISTANCE to the fitness function? Distance * this.multi")]
    [SerializeField] private float m_DistanceMultiplier = 1.4f;
    [Tooltip("How important is the SPEED to the fitness function?")]
    [SerializeField] private float m_AvarageSpeedMulti = 0.2f;

    private Vector3 m_StartPosition; private Vector3 m_StartRotation;
    private Vector3 m_LastPosition;

    private float m_TotalDistanceTravelled;
    private float m_AvarageSpeed;

    // Distance Between The Origin Ray To The Position The Ray Hit The Wall
    private float a_Sensor, b_Sensor, c_Sensor;

    private void Awake()
    {
        m_StartPosition = transform.position;
        m_StartRotation = transform.eulerAngles;
    }

    private void Reset()
    {
        m_TimeSinceStart = 0f;
        m_TotalDistanceTravelled = 0f;
        m_AvarageSpeed = 0f;
        m_OverallFitness = 0f;

        m_LastPosition = m_StartPosition;
        transform.position = m_StartPosition;
        transform.eulerAngles = m_StartRotation;
    }





}
