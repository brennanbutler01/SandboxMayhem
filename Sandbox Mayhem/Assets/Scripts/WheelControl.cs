using UnityEngine;

public class WheelControl : MonoBehaviour
{
    public Transform wheelModel;

    [HideInInspector] public WheelCollider wheelCollider;

    public bool steerable;

    Vector3 position;
    Quaternion rotation;

    private void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    void Update()
    {
        wheelCollider.GetWorldPose(out position, out rotation);
        wheelModel.transform.position = position;
        wheelModel.transform.rotation = rotation;
    }
}