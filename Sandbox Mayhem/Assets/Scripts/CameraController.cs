using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CameraController : MonoBehaviour
{
    public Transform finishLookTarget;
    public Transform car;
    public Transform cameraTarget;
    public Transform lookTarget;
    public float speed = 15f;

    private float rotation;

    void Start()
    {
        rotation = 0;
    }

    void FixedUpdate()
    {
        if (rotation > 1)
        {
            var angle = rotation * Time.fixedDeltaTime * 2.5f;
            
            lookTarget.position = Vector3.Lerp(lookTarget.position, finishLookTarget.position, Time.fixedDeltaTime * 0.5f);
            cameraTarget.transform.RotateAround(car.position, Vector3.up, angle);
            lookTarget.transform.RotateAround(car.position, Vector3.up, angle);
            
            rotation -= angle;
        }
        transform.position = Vector3.Lerp(transform.position, cameraTarget.position, speed * Time.fixedDeltaTime);
        transform.LookAt(lookTarget.position);
    }

    public void RotateCamera()
    {
        rotation = 200;
    }
}
