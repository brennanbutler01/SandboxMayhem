using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RobotLightsController : MonoBehaviour
{
    private Transform rotatingLights;
    // Start is called before the first frame update
    void Start()
    {
        SetActive(false);
        rotatingLights = transform.Find("Head");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            rotatingLights.Rotate(transform.up, 180 * Time.deltaTime);
        }
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
