using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject lookTarget;
    public GameObject cameraTarget;
    private Vector3 offset;
    public float speed = 15f;
    
    void Start()
    {
        offset = transform.position - cameraTarget.transform.position;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, cameraTarget.transform.position, speed * Time.deltaTime);
        transform.LookAt(lookTarget.transform.position);
    }

}
