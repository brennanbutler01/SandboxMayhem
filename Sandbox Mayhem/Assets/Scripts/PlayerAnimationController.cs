using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public GameObject steeringWheel;
    public GameObject defaultSmile;
    public GameObject happySmile;

    private Animator animator;
    private float smoothness = 50;
    private float maxSteeringAngle = 30;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetFloat("Steering", 0);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float currentSteering = animator.GetFloat("Steering");
        float steering = Mathf.Lerp(currentSteering, horizontalInput, Time.deltaTime * smoothness);

        steeringWheel.transform.localRotation = Quaternion.Euler(- horizontalInput * maxSteeringAngle, 0, 0);
        animator.SetFloat("Steering", steering);
    }

    public void Victory()
    {
        defaultSmile.SetActive(false);
        happySmile.SetActive(true);
        
        animator.SetBool("Won", true);
    }

    public void Defeat()
    {
        animator.SetBool("Lost", true);
    }
}