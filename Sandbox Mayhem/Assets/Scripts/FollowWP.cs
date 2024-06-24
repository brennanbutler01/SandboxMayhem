using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FollowWP : MonoBehaviour
{
    public GameObject[] waypoints;
    WheelControl[] wheelControls;

    int firstIndex = 0;
    int overallIndex = 0;
    int randIndex = 0;
    private CarController carController;
    private PlayerController humanController;
    private PlayerController aiController;

    public float speed = 0f;
    public float rotationSpeed = 2.0f;
    public float minimumSpeed = 10f;
    public float maxAngle = 0f;
    public float brakeForce = 0f;
    public float maxSpeed = 0f;
    public float accelerationForce = 0f;
    public Vector3 prevPosition;
    public float extraAccelerationForce = 0f;
    public float stateAccelerationBoost = 0f;
    public float climbSpeed = 0f;
    private RaycastHit objectHit;

    private const string normal = "normal";
    private const string losing = "losing";
    private const string winning = "winning";
    private string state = normal;

    // Start is called before the first frame update
    void Start()
    {
        // Randomly determine which waypoint to start driving towards
        firstIndex = UnityEngine.Random.Range(0, 3);

        // Get waypoints and sort in order
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        Array.Sort(waypoints, determineLarger);

        wheelControls = GetComponentsInChildren<WheelControl>();
        carController = GetComponent<CarController>();
        humanController = FindObjectOfType<HumanController>().gameObject.GetComponent<PlayerController>();
        aiController = GetComponent<PlayerController>();
    }

    // Determine AI position relative to human to set state accordingly
    void determinePosition()
    {
        //TODO set state
        if (humanController.currentLap == aiController.currentLap)
        {
            if (humanController.currentWaypoint > aiController.currentWaypoint)
            {
                state = losing;
                Debug.Log("AI is losing!");
            }
            else if (humanController.currentWaypoint < aiController.currentWaypoint)
            {
                state = winning;
                Debug.Log("AI is winning!");
            }
            else
            {
                state = normal;
                Debug.Log("It's too close to tell!");
            }
        }
        else if (humanController.currentLap > aiController.currentLap)
        {
            state = losing;
            Debug.Log("AI is losing!");
        }
        else if (humanController.currentLap < aiController.currentLap)
        {
            state = winning;
            Debug.Log("AI is winning!");
        }
    }

    // Determine order of waypoints
    int determineLarger(GameObject nameX, GameObject nameY)
    {
        if (Int32.Parse(nameX.name.Substring(8)) < Int32.Parse(nameY.name.Substring(8)))
        {
            return -1;
        }
        return 1;
    }

    // Apply basic torque to wheels so that they rotate
    void rotateWheels()
    {
        foreach (var wheel in wheelControls)
        {
            wheel.wheelCollider.motorTorque = 100f;
        }
    }

    void checkState()
    {
        switch (state)
        {
            case normal:
                stateAccelerationBoost = 0f;
                break;
            case losing:
                stateAccelerationBoost = 0.1f;
                break;
            case winning:
                stateAccelerationBoost = -0.1f;
                break;
            default:
                break;
        }
    }

    void FixedUpdate()
    {
        // Don't let cars accelerate until race begins
        if (!carController.isMovementEnabled)
        {
            return;
        }

        rotateWheels();
        determinePosition();

        // Determine if a waypoint is close enough to collect
        if (Vector3.Distance(this.transform.position, waypoints[firstIndex].transform.position) < 3)
        {
            prevPosition = waypoints[firstIndex].transform.position;
            overallIndex++;

            if (randIndex == 0)
            {
                randIndex = UnityEngine.Random.Range(0, 2);
            }
            else if (randIndex == 1)
            {
                randIndex = UnityEngine.Random.Range(0, 3);
            }
            else if (randIndex == 2)
            {
                randIndex = UnityEngine.Random.Range(1, 3);
            }

            firstIndex = (overallIndex * 3) + randIndex;
            if (firstIndex < waypoints.Length)
            {
                // Add extra speed if there is a significant elevation change between waypoints
                if ((prevPosition.y - waypoints[firstIndex].transform.position.y) < -1)
                {
                    extraAccelerationForce = 0.5f;
                    climbSpeed = 0.00f;
                }
                else
                {
                    extraAccelerationForce = 0f;
                    climbSpeed = 0f;
                }
            }
        }

        // If we've reached the end of the waypoints, reset to the beginning
        if (firstIndex >= waypoints.Length)
        {
            prevPosition = waypoints[waypoints.Length - 1].transform.position;
            firstIndex = 0;
            overallIndex = 0;
        }

        Quaternion lookatWP = Quaternion.LookRotation(waypoints[firstIndex].transform.position - this.transform.position);

        //TODO add check to prevent AI from freaking out if it misses a waypoint
        //could probably add another maximum angle that it doesn't even try to head towards if it's too extreme, just resets destination to the next one.

        checkState();

        // Slow down the car if the next waypoint is past a reasonable angle to turn
        if (Quaternion.Angle(this.transform.rotation, lookatWP) > maxAngle)
        {
            if (speed > minimumSpeed)
            {
                speed = speed - brakeForce;
            }
        }
        else
        {
            // Accelerate until max speed is reached
            if (speed < maxSpeed)
            {
                speed = speed + accelerationForce + extraAccelerationForce + stateAccelerationBoost;
            }
            // Add some speed for hill climbing
            float xAngle = (this.transform.rotation.x - lookatWP.x) * 100;
            if (xAngle < 3f)
            {
                speed = speed + .01f;
            }
            else if (xAngle < -3f)
            {
                speed = speed - .01f;
            }
        }

        // Rotate AI car
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookatWP, rotationSpeed * Time.deltaTime);

        this.transform.rotation.Set(this.transform.rotation.x, this.transform.rotation.y, 0, this.transform.rotation.w);

        // Check for possible collisions in front
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, fwd * 5, Color.green);

        if (Physics.Raycast(transform.position, fwd, out objectHit, 5))
        {
            //Do something if hit object is a car
            if (objectHit.collider.name.Contains("Car"))
            {
                if (speed > minimumSpeed)
                {
                    speed = speed - brakeForce;
                }
            }
        }

        // Apply speed to AI car
        this.transform.Translate(0, climbSpeed, speed * Time.deltaTime);
    }
}