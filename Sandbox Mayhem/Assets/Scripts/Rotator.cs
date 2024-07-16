using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 rotationVector = new Vector3(15, 30, 45);
    public bool isLocal = true; //Local vs global reference rotation

    // Update is called once per frame
    void Update()
    {
        if (isLocal)
        {
            transform.Rotate(rotationVector * Time.deltaTime);
        } else
        {
            transform.Rotate(rotationVector * Time.deltaTime, Space.World);
        }
        
    }
}