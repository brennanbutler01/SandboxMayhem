using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting.Dependencies.NCalc;
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
    // used for tracking consumable speed multiplier
    private float _consumableBoostFactor  = 1f;
    // used for tracking boost zone speed
    private float _boostZoneBoostFactor = 1f;
    private float _combinedSpeedFactor => _consumableBoostFactor * _boostZoneBoostFactor;
    
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
    public Material carBrakeLightWhenBrakingMaterial;
    public GameObject carBrakeLights;

    private Rigidbody rigidBody;
    private WheelControl[] wheels;
    private PlayerController playerController;
    private float drivingOnDirtSpeedPenalty =  6;
    private Material carOriginalBrakeLightMaterial;
    private Coroutine _speedBoostCoroutine;
    
    
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass += centerOfMassVector;
        wheels = GetComponentsInChildren<WheelControl>();
        playerController = GetComponent<PlayerController>();
        carOriginalBrakeLightMaterial = GameObject.Find("Taillights_glass_brakelights").GetComponent<MeshRenderer>().material;
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
        float absoluteSpeed = Math.Abs(forwardSpeed);
        
        //Input different from current direction
        bool braking = forwardSpeed * verticalInput < 0;

        //Steering calculations
        float adjustedMaxSpeed =  Mathf.Min(
                                     (maxSpeed - (isDrivingOnDirt ? 1 : 0) * drivingOnDirtSpeedPenalty) * _combinedSpeedFactor,
                                     maxSpeed * 2f  // This sets the absolute maximum speed to 200% of maxSpeed
                                 );
        float speedFactor = Mathf.InverseLerp(0, adjustedMaxSpeed, forwardSpeed);
        float steering = Mathf.Lerp(steeringAngleAtMaxSpeed, maxSteerAngle, speedFactor);
        float steerAngle = horizontalInput * steering;
        
        //Clamp speed between 0 and 100 for torque curve.
        float normalizedSpeed = Mathf.Clamp01(absoluteSpeed / adjustedMaxSpeed) * 100;
        
        //Apply wheel collider physics (torque and steer angle)
        foreach (var wheel in wheels)
        {
            if (braking)
            {
                wheel.wheelCollider.motorTorque = 0;
                wheel.wheelCollider.brakeTorque = brakeTorque * Math.Abs(verticalInput);
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
        if (IsGrounded() && absoluteSpeed > 3.5f)
        {
            OverrideSteeringPhysics(steerAngle, forwardSpeed);
        }

        //Speed in km/h
        speed = transform.InverseTransformDirection(rigidBody.velocity).z * 3.6f;
        
        //Apply downforce to press car against the track
        rigidBody.AddForce(-1 * Math.Abs(speed) * downforce * transform.up);
        
        if (audioEventManager is not null)
        {
            audioEventManager.setEnginePitchAudio(speedFactor);
        }

        //Brake lights
        if (verticalInput < 0)
        {
            // Brake lights on
            carBrakeLights.GetComponent<MeshRenderer>().material = carBrakeLightWhenBrakingMaterial;
        } else
        {
            // Brake lights off
            carBrakeLights.GetComponent<MeshRenderer>().material = carOriginalBrakeLightMaterial;
        }

    }

    private void OverrideSteeringPhysics(float steering, float forwardSpeed)
    {
        steering *= Math.Sign(forwardSpeed);
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

    public void triggerSpeedChange(float newSpeedPercentage=0.5f)
    {
        rigidBody.velocity *= newSpeedPercentage;
    }

    public void ApplyBoostZone(float speedModifier)
    {
        _boostZoneBoostFactor = speedModifier;
        if (!playerController.isHuman) triggerSpeedChange(Mathf.Clamp(_boostZoneBoostFactor, 1f, 1.03f));
    }

    public void RemoveBoostZone()
    {
        _boostZoneBoostFactor = 1f;
        if (!playerController.isHuman) triggerSpeedChange(1f);
    }
    
    public void ActivateConsumableSpeedBoost(float speedModifier, float? duration = 3f)
    {
        if (_speedBoostCoroutine  != null)
        { 
            StopCoroutine(_speedBoostCoroutine);
        }
        
        _consumableBoostFactor = speedModifier;
        triggerSpeedChange(speedModifier);
        _speedBoostCoroutine = StartCoroutine(ResetSpeedAfterConsumableBoost(duration ?? default));
    }

    // gradually reduces the speed at the end of the boost
    private IEnumerator ResetSpeedAfterConsumableBoost(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        triggerSpeedChange(1f / _consumableBoostFactor);
        _consumableBoostFactor = 1f;
        
        var elapsed = 0f;
        const float cooldownDuration = 2f;

        // count down and slowly get slower
        while (elapsed < cooldownDuration)
        {
            elapsed += Time.deltaTime;
            
            // lerp to smoothly slow down
            _consumableBoostFactor = Mathf.Lerp(_consumableBoostFactor, 1f, elapsed/cooldownDuration);
            
            yield return null;
        }

        triggerSpeedChange(1f);
        _consumableBoostFactor = 1f;
    }

    public bool isCarTilted()
    {
        bool result = false;
        
        //tilted sideways or upside down
        result = result || (Math.Abs(transform.rotation.z) >= 0.4f);

        //tilted front or back
        result = result || (Math.Abs(transform.rotation.x) >= 0.4f);

        return result;
    }

    public void resetCarPosition(Vector3 position)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(0, 90, 0);
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.velocity = Vector3.zero;
    }
}
