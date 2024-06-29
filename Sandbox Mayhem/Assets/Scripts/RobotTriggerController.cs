using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class RobotTriggerController : MonoBehaviour
{
    public GameObject robot;
    public AudioClip robotAudio;
    public AudioMixerGroup sfxMixer;
    
    private Animator anim;
    private Vector3 targetPos;
    private Vector3 initialPos;
    private RobotState state;
    
    private AudioSource robotAudioSource;

    private enum RobotState
    {
        Waiting,
        Following,
        Returning
    }
    
    void Awake()
    {
        targetPos = initialPos = robot.transform.position;
        anim = GetComponentInParent<Animator>();
        state = RobotState.Waiting;
        
        robotAudioSource = gameObject.AddComponent<AudioSource>();
        robotAudioSource.clip = robotAudio;
        robotAudioSource.loop = true;
        robotAudioSource.minDistance = 5f;
        robotAudioSource.maxDistance = 100f;
        robotAudioSource.outputAudioMixerGroup = sfxMixer;
    }

    private void Update()
    {
        if (robot.transform.position == targetPos && state != RobotState.Waiting)
        {
            state = RobotState.Waiting;
            anim.SetBool("PlayerNear", false);
        }
        else
        {
            float speed = state == RobotState.Following ? 10f : 20;
            robot.transform.position = Vector3.MoveTowards(robot.transform.position, targetPos, speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isHumanPlayer(other))
        {
            targetPos = other.transform.position;
            robot.transform.LookAt(targetPos);
            anim.SetBool("PlayerNear", true);
            state = RobotState.Following;
            robotAudioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isHumanPlayer(other))
        {
            targetPos = initialPos;
            robot.transform.LookAt(targetPos);
            anim.SetBool("PlayerNear", false);
            state = RobotState.Returning;
            robotAudioSource.Stop();
        }
    }

    private bool isHumanPlayer(Collider col)
    {
        PlayerController controller;

        return col.CompareTag("Player") &&
               (controller = col.GetComponent<PlayerController>()) &&
               controller.isHuman;
    }
}
