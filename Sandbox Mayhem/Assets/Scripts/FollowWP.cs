using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FollowWP : MonoBehaviour
{
    public GameObject[] waypoints;

    int firstIndex = 0;
    int overallIndex = 0;
    private CarController carController;

    public float speed = 0f;
    public float rotationSpeed = 2.0f;
    public float minimumSpeed = 13f;
    public float maxAngle = 20f;
    public float brakeForce = 0.9f;
    public float maxSpeed = 35f;
    public float accelerationForce = .025f;

    // Start is called before the first frame update
    void Start()
    {
        firstIndex = UnityEngine.Random.Range(0, 3);
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        Array.Sort(waypoints, determineLarger);
        carController = GetComponent<CarController>();
    }

    int determineLarger(GameObject nameX, GameObject nameY)
    {
        if (Int32.Parse(nameX.name.Substring(8)) < Int32.Parse(nameY.name.Substring(8)))
        {
            return -1;
        }
        return 1;
    }

    void FixedUpdate()
    {
        if (!carController.isMovementEnabled)
        {
            return;
        }

        //Debug.Log("heading for " + waypoints[firstIndex]);
        if (Vector3.Distance(this.transform.position, waypoints[firstIndex].transform.position) < 3)
        {
            overallIndex++;
            firstIndex = (overallIndex * 3) + UnityEngine.Random.Range(0, 3);
        }

        if (firstIndex >= waypoints.Length)
        {
            firstIndex = 0;
            overallIndex = 0;
        }

        //this.transform.LookAt(waypoints[currentWP].transform);

        Quaternion lookatWP = Quaternion.LookRotation(waypoints[firstIndex].transform.position - this.transform.position);

        Debug.Log("Angle: " + Quaternion.Angle(this.transform.rotation, lookatWP));
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
                speed = speed + accelerationForce;
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

        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }
}