using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

struct Behavior
{
    public float speed;
    public float rotationSpeed;
    public float minimumSpeed;
    public float maxAngle;
    public float brakeForce;
    public float maxSpeed;
    public float accelerationForce;
}

public class FollowWP : MonoBehaviour
{
    public GameObject[] waypoints;
    private SortedDictionary<string, Behavior> behaviors;

    private GameObject[] wheels;
    WheelControl[] wheelControls;

    int firstIndex = 0;
    int overallIndex = 0;
    private CarController carController;
    int randIndex = 0;

    public float speed = 0f;
    public float rotationSpeed = 2.0f;
    public float minimumSpeed = 13f;
    public float maxAngle = 20f;
    public float brakeForce = 0.9f;
    public float maxSpeed = 35f;
    public float accelerationForce = .1f;
    public Vector3 prevPosition;
    public float extraAccelerationForce = 0f;
    public float climbSpeed = 0f;
    private float rotateAmount = 0f;
    private RaycastHit objectHit;

    // Start is called before the first frame update
    void Start()
    {
        firstIndex = UnityEngine.Random.Range(0, 3);
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        wheels = GameObject.FindGameObjectsWithTag("Wheel");
        Array.Sort(waypoints, determineLarger);
        setBehaviors();
        wheelControls = GetComponentsInChildren<WheelControl>();
        carController = GetComponent<CarController>();
    }

    void determinePosition()
    {
        CarController[] cars = FindObjectsOfType<CarController>();
        PlayerController humanController = cars[1].gameObject.GetComponent<PlayerController>();
        PlayerController aiController = GetComponent<PlayerController>();

        if (humanController.currentLap == aiController.currentLap)
        {
            if (humanController.currentWaypoint > aiController.currentWaypoint)
            {
                Debug.Log("AI is losing!");
            }
            else if (humanController.currentWaypoint < aiController.currentWaypoint)
            {
                Debug.Log("AI is winning!");
            }
            else
            {
                Debug.Log("It's too close to tell!");
            }
        }
        else if (humanController.currentLap > aiController.currentLap)
        {
            Debug.Log("AI is losing!");
        }
        else if (humanController.currentLap < aiController.currentLap)
        {
            Debug.Log("AI is winning!");
        }
    }

    int determineLarger(GameObject nameX, GameObject nameY)
    {
        if (Int32.Parse(nameX.name.Substring(8)) < Int32.Parse(nameY.name.Substring(8)))
        {
            return -1;
        }
        return 1;
    }

    void setBehaviors()
    {
        behaviors = new SortedDictionary<string, Behavior>();

        Behavior aggressive = new Behavior();
        aggressive.accelerationForce = .1f;
        aggressive.brakeForce = .9f;
        aggressive.maxSpeed = 35f;
        aggressive.maxAngle = 20f;

        behaviors.Add("aggressive", aggressive);

        Behavior passive = new Behavior();
        passive.accelerationForce = .5f;
        passive.brakeForce = 1f;
        passive.maxSpeed = 30f;
        passive.maxAngle = 22f;

        behaviors.Add("passive", passive);

        Behavior intermediate = new Behavior();
        intermediate.accelerationForce = .75f;
        intermediate.brakeForce = .95f;
        intermediate.maxSpeed = 32.5f;
        intermediate.maxAngle = 21f;

        behaviors.Add("intermediate", intermediate);
    }

    void rotateWheels()
    {
        foreach (var wheel in wheelControls)
        {
            if (wheel.motorized)
            {
                wheel.WheelCollider.motorTorque = 100f;
            }
        }
    }

    void FixedUpdate()
    {
        if (!carController.isMovementEnabled)
        {
            return;
        }


        rotateWheels();
        determinePosition();
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
                if ((prevPosition.y - waypoints[firstIndex].transform.position.y) < -1)
                {
                    extraAccelerationForce = 0.3f;
                    climbSpeed = 0.05f;
                }
                else
                {
                    extraAccelerationForce = 0f;
                    climbSpeed = 0f;
                }
            }
        }

        if (firstIndex >= waypoints.Length)
        {
            prevPosition = waypoints[waypoints.Length - 1].transform.position;
            firstIndex = 0;
            overallIndex = 0;
        }

        //this.transform.LookAt(waypoints[currentWP].transform);

        Quaternion lookatWP = Quaternion.LookRotation(waypoints[firstIndex].transform.position - this.transform.position);

        //Debug.Log("Angle: " + Quaternion.Angle(this.transform.rotation, lookatWP));
        //Debug.Log("X angle diff: " + ;
        if (Quaternion.Angle(this.transform.rotation, lookatWP) > maxAngle)
        {
            if (speed > minimumSpeed)
            {
                speed = speed - brakeForce;
            }
        }
        else
        {
            if (speed < maxSpeed)
            {
                speed = speed + accelerationForce + extraAccelerationForce;
            }
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
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookatWP, rotationSpeed * Time.deltaTime);

        this.transform.rotation.Set(this.transform.rotation.x, this.transform.rotation.y, 0, this.transform.rotation.w);


        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, fwd * 5, Color.green);

        if (Physics.Raycast(transform.position, fwd, out objectHit, 5))
        {
            //do something if hit object ie
            if (objectHit.collider.name.Contains("Car"))
            {
                if (speed > minimumSpeed)
                {
                    Debug.Log("Slowing down!");
                    speed = speed - brakeForce;
                }
            }
        }

        this.transform.Translate(0, climbSpeed, speed * Time.deltaTime);
    }
}