using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTrapController : MonoBehaviour
{
    public GameObject sawBlade;
    private Animator anim;
    private bool isAnimation = false;
    private const string IDLE = "idle";
    private const string MOVEUP = "moveup";
    private const string MOVEDOWN = "movedown";
    private Vector3 rotationVector3 = new Vector3(-300, 0, 0);


    // Start is called before the first frame update
    void Start()
    {
        anim = sawBlade.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isAnimation)
        {
            sawBlade.transform.Rotate(rotationVector3 * Time.deltaTime);
        }
    }

    //private void disableOtherAnimations(Animator animator, )
    private void OnTriggerEnter(Collider other)
    {
        if (anim is not null)
        {
            isAnimation = true;
            runAnimation(MOVEUP);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (anim is not null)
        {
            
            isAnimation = false;
            runAnimation(MOVEDOWN);
            
        }
    }

    private void runAnimation(string animationName)
    {
        disableAnimationsOtherThan(animationName);
        anim.SetBool(animationName, true);
    }

    private void disableAnimationsOtherThan(string animationName)
    {
        foreach(AnimatorControllerParameter param in anim.parameters)
        {
            if (param.name != animationName)
            {
                anim.SetBool(param.name, false);
            }
        }
    }
}
