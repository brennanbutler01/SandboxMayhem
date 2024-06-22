using UnityEngine;

public class CarController : MonoBehaviour {
    public AnimationCurve motorTorque = new AnimationCurve(
            new Keyframe(0, 1000), 
            new Keyframe(80, 2000), 
            new Keyframe(130, 0)
    );
    public Vector3 centerOfMassVector;
    public float maxSteerAngle = 30.0f;
    public float maxSpeed = 130;
    public float steeringAngleAtMaxSpeed = 15;
    public float brakeTorque = 1800;
    public float downforce = 1.0f;
    public bool isAccelerationEnabled = false; // So that the user can't accelerate before the countdown is finished
    public AudioEventManager audioEventManager;

    public float speed = 0;
    
    Rigidbody rigidBody;
    WheelControl[] wheels;

    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.centerOfMass += centerOfMassVector;
        wheels = GetComponentsInChildren<WheelControl>();
    }

    void FixedUpdate () {
        speed = transform.InverseTransformDirection(rigidBody.velocity).z * 3.6f;
        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.velocity);
        float speedFactor = Mathf.InverseLerp(0, maxSpeed, forwardSpeed);
        
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        
        bool isAccelerating = Mathf.Sign(verticalInput) == Mathf.Sign(forwardSpeed);

        foreach (var wheel in wheels)
        {
            if (wheel.steerable)
            {
                float steering = Mathf.Lerp(steeringAngleAtMaxSpeed, maxSteerAngle, speedFactor);
                wheel.WheelCollider.steerAngle = horizontalInput * steering;
            }
            
            if (isAccelerationEnabled && isAccelerating)
            {
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = verticalInput * motorTorque.Evaluate(speed);
                }
                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                wheel.WheelCollider.brakeTorque = verticalInput * brakeTorque * -1;
                wheel.WheelCollider.motorTorque = 0;
            }
        }
        rigidBody.AddForce(-1 * speed * downforce * transform.up);
        if (audioEventManager is not null)
        {
            audioEventManager.setEnginePitchAudio(speedFactor);
        }
    }
}
