using System;
using System.Linq;
using UnityEngine;

public class CarController : MonoBehaviour {
    // Torque
    public AnimationCurve motorTorque = new(
            new Keyframe(0, 1000), 
            new Keyframe(80, 2000), 
            new Keyframe(130, 0)
    );
    public float brakeTorque = 4000;

    //Speed
    public float speed = 0;
    public float maxSpeed = 30;
    
    //Steering
    public float maxSteerAngle = 30;
    public float maxAngularVelocity = 1.2f;
    public float angularVelocitySpeed = 20;
    public float steeringAngleAtMaxSpeed = 15;
    public float steeringSpeed = 2.3f;

    public Vector3 centerOfMassVector;
    public float downforce = 1.0f;
    public bool isMovementEnabled = true;
    public bool isDrivingOnDirt = false;
    public AudioEventManager audioEventManager;

    private Rigidbody rigidBody;
    private WheelControl[] wheels;
    private PlayerController playerController;
    private float drivingOnDirtSpeedPenalty =  6;
    
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass += centerOfMassVector;
        wheels = GetComponentsInChildren<WheelControl>();
        playerController = GetComponent<PlayerController>();
    }

    void FixedUpdate ()
    {
        if (!playerController.isHuman || !isMovementEnabled)
        {
            return;
        }

        //Inputs
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);
        bool movingForward = forwardSpeed >= 0;
        bool braking = movingForward && verticalInput < 0;

        //Steering calculations
        float adjustedMaxSpeed = maxSpeed- (isDrivingOnDirt ? 1 : 0) * drivingOnDirtSpeedPenalty;
        float speedFactor = Mathf.InverseLerp(0, adjustedMaxSpeed, forwardSpeed);
        float steering = Mathf.Lerp(steeringAngleAtMaxSpeed, maxSteerAngle, speedFactor);
        float steerAngle = horizontalInput * steering;
        
        //Clamp speed between 0 and 100 for torque curve.
        float normalizedSpeed = Mathf.Clamp01(Math.Abs(forwardSpeed) / adjustedMaxSpeed) * 100;
        
        //Apply wheel collider physics (torque and steer angle)
        foreach (var wheel in wheels)
        {
            if (braking)
            {
                wheel.wheelCollider.motorTorque = 0;
                wheel.wheelCollider.brakeTorque = brakeTorque * verticalInput * -1;
            }
            else
            {
                wheel.wheelCollider.motorTorque = motorTorque.Evaluate(normalizedSpeed) * verticalInput;
                wheel.wheelCollider.brakeTorque = 0;
            }
            if (wheel.steerable)
            {
                wheel.wheelCollider.steerAngle = steerAngle;
            }
        }
        
        //Limit velocity. Adjust if we want to add boosts or allow higher speeds at ramps/slopes.
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, adjustedMaxSpeed);
        
        //Override steering to make it feel more arcade-like.
        //Only if grounded to avoid unexpected behavior, and car is moving so that it doesn't rotate in place.
        if (IsGrounded() && forwardSpeed > 5)
        {
            OverrideSteeringPhysics(steerAngle);
        }

        //Speed in km/h
        speed = transform.InverseTransformDirection(rigidBody.velocity).z * 3.6f;
        
        //Apply downforce
        rigidBody.AddForce(-1 * speed * downforce * transform.up);

        if (audioEventManager is not null)
        {
            audioEventManager.setEnginePitchAudio(speedFactor);
        }
    }

    private void OverrideSteeringPhysics(float steering)
    {
        Vector3 currentAngularVelocity = rigidBody.angularVelocity;
        float targetVelocity = steering * maxAngularVelocity / maxSteerAngle;
        currentAngularVelocity.y = Mathf.MoveTowards(currentAngularVelocity.y, targetVelocity, Time.fixedDeltaTime * angularVelocitySpeed);

        // Manually assign velocities after steering.
        rigidBody.angularVelocity = currentAngularVelocity;
        rigidBody.velocity = Quaternion.AngleAxis(steering * Time.fixedDeltaTime * steeringSpeed, transform.up) * rigidBody.velocity;
    }

    private bool IsGrounded()
    {
        return wheels.All(wheel => wheel.wheelCollider.isGrounded && wheel.wheelCollider.GetGroundHit(out var hit));
    }
}
