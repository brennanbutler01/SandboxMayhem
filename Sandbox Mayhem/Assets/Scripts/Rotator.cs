using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotationVector = new Vector3(15, 30, 45);

    // Update is called once per frame
    void Update()
    {
        transform.Rotate (rotationVector * Time.deltaTime);
    }
}