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
    [SerializeField] private float m_Steering;

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
    private float right_Sensor, left_Sensor, forward_Sensor;

    private Vector3 m_Input;

    private const int c_NormalizedFactor = 20;

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

    public void OnCollisionEnter(Collision collision)
    {
        Reset();
    }

    public void MoveCar(float vertical, float horizontal)
    {
        // Position & Direction
        m_Input = Vector3.Lerp(Vector3.zero, new Vector3(0,0,vertical*11.4f), .02f);
        m_Input = transform.TransformDirection(m_Input);
        transform.position += m_Input;

        // Rotation
        transform.eulerAngles += new Vector3(0, (horizontal * 90) * .02f, 0);
    }

    private void SensorObservance()
    {
        Vector3 rightDiagonal = transform.forward + transform.right;
        Vector3 forward = transform.forward;
        Vector3 leftDiagonal = transform.forward - transform.right;

        Ray ray = new Ray(transform.position, rightDiagonal);
        RaycastHit rayHit;

        if(Physics.Raycast(ray, out rayHit)) {
            right_Sensor = rayHit.distance / c_NormalizedFactor; // NORMALs
            print($"Right: {right_Sensor}");
        }

        ray.direction = forward;
        if (Physics.Raycast(ray, out rayHit))
        {
            forward_Sensor = rayHit.distance / c_NormalizedFactor; // NORMALs
            print($"Forward: {forward_Sensor}");
        }

        ray.direction = leftDiagonal;
        if (Physics.Raycast(ray, out rayHit))
        {
            left_Sensor = rayHit.distance / c_NormalizedFactor; // NORMALs
            print($"Left: {left_Sensor }");
        }
    }


}
