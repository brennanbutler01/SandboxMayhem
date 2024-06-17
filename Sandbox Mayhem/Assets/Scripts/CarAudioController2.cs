using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAudioController2 : MonoBehaviour
{
    private Rigidbody rb;
    private bool triggeredOffroad = false;
    private string TERRAIN_OBJECT_NAME_STRING = "Terrain";
    public AudioEventManager audioEventManager;

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.impulse.magnitude > 5000f)
        {
            //we'll just use the first contact point for simplicity
            EventManager.TriggerEvent<CarCollisionEvent, Vector3>(collision.contacts[0].point);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        if (isOffRoad() && rb.velocity.magnitude > 2) // Adding a minimum velocity for better experience
        {
            if (!triggeredOffroad)
            {
                audioEventManager.playStopDrivingOnDirtAudio(true);
                triggeredOffroad = true;
            }
        }
        else
        {
            triggeredOffroad = false;
            // stop playing dirt audio
            audioEventManager.playStopDrivingOnDirtAudio(false);
        }
    }

    private bool isOffRoad()
    {
        bool result = false;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.name == TERRAIN_OBJECT_NAME_STRING)
            {
                result = true;
            }
        }

        return result;
    }
}
