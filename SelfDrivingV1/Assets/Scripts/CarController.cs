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

    [Tooltip("Sensor Value Output Needs To Be Between 0-1")]
    [SerializeField] private int m_NormalizedFactor = 20;

    [SerializeField] private float m_TimeSinceStart = 0f; // checking for idle cars / usless cars -> Reset

    [Header("Fitness -> How Far? How Fast? Whats Valuable? Gives importance to each factor. Example: If the two cars go the exact same distance, how do we differentiate which one is better?")]
    [SerializeField] private float m_OverallFitness;
    [Tooltip("How important is the DISTANCE to the fitness function? Distance * this.multi")]
    [SerializeField] private float m_DistanceMultiplier = 1.4f;
    [Tooltip("How important is the SPEED to the fitness function?")]
    [SerializeField] private float m_AvarageSpeedMulti = 0.2f;
    [Tooltip("How important it is to stay in the middle of the track?")]
    [SerializeField] private float m_SensorMulti = 0.1f;

    private Vector3 m_StartPosition; private Vector3 m_StartRotation;
    private Vector3 m_LastPosition;

    private float m_TotalDistanceTravelled;
    private float m_AvarageSpeed;

    // Distance Between The Origin Ray To The Position The Ray Hit The Wall
    private float right_Sensor, left_Sensor, forward_Sensor;

    private Vector3 m_Input;

    private void Awake()
    {
        m_StartPosition = transform.position;
        m_StartRotation = transform.eulerAngles;
    }

    private void FixedUpdate()
    {
        SensorObservance();
        m_LastPosition = transform.position;

        // TODO: assign these two parameters via Neural Net
        MoveCar(m_Acceleration, m_Steering);

        m_TimeSinceStart += Time.deltaTime;

        CalculateFitnessModel();

        // TOOD: reset A & S
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

    private void CalculateFitnessModel()
    {
        // every frame we set last position to our current position
        // NOT exact distance BUT a metric to compare each car with eachother
        m_TotalDistanceTravelled += Vector3.Distance(transform.position, m_LastPosition);
        m_AvarageSpeed = m_TotalDistanceTravelled / m_TimeSinceStart;


        m_OverallFitness = (m_TotalDistanceTravelled * m_DistanceMultiplier)    + 
                            (m_AvarageSpeed * m_AvarageSpeedMulti)              + 
                                (((right_Sensor + forward_Sensor + left_Sensor) / 3) * m_SensorMulti);

        // DUMB / SMART ??
        if (m_TimeSinceStart > 20 && m_OverallFitness < 40) { // NOT DOIN ANYTHING
            Reset();
        }

        if(m_OverallFitness >= 1000) { // DID TO GOOD (at least 3 Laps)    
            // TODO: Save network to a JSON
            Reset(); 
        }
    }


    private void SensorObservance()
    {
        Vector3 rightDiagonal = transform.forward + transform.right;
        Vector3 forward = transform.forward;
        Vector3 leftDiagonal = transform.forward - transform.right;

        Ray ray = new Ray(transform.position, rightDiagonal);
        RaycastHit rayHit;

        if(Physics.Raycast(ray, out rayHit)) {
            right_Sensor = rayHit.distance / m_NormalizedFactor; // NORMALs
            print($"Right: {right_Sensor}");
            Debug.DrawLine(ray.origin, rayHit.point, Color.red);
        }

        ray.direction = forward;
        if (Physics.Raycast(ray, out rayHit))
        {
            forward_Sensor = rayHit.distance / m_NormalizedFactor; // NORMALs
            print($"Forward: {forward_Sensor}");
            Debug.DrawLine(ray.origin, rayHit.point, Color.red);
        }

        ray.direction = leftDiagonal;
        if (Physics.Raycast(ray, out rayHit))
        {
            left_Sensor = rayHit.distance / m_NormalizedFactor; // NORMALs
            print($"Left: {left_Sensor }");
            Debug.DrawLine(ray.origin, rayHit.point, Color.red);
        }
    }
}
