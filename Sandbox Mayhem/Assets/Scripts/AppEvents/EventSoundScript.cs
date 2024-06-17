using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EventSoundScript : MonoBehaviour
{
    // Note: this code is based on the Georgia Tech Video Game Design lectures in https://github.gatech.edu/IMTC/CS4455_M1_Support

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }
}
