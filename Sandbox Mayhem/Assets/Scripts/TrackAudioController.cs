using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackAudioController : MonoBehaviour
{
    private AudioSource raceStartAudioSource;
    private AudioSource raceMusicAudioSource;
    private string RACE_START_SUBSTRING = "gonzo-racestart";
    private string RACE_MUSIC_SUBSTRING = "gonzo-racemusic";
    private bool raceIsStillStarting = true;


    // Start is called before the first frame update
    void Start()
    {
        foreach (AudioSource comp in GetComponents<AudioSource>())
        {
            if (comp.clip.name.Contains(RACE_START_SUBSTRING))
            {
                raceStartAudioSource = comp;
            }
            else if (comp.clip.name.Contains(RACE_MUSIC_SUBSTRING))
            {
                raceMusicAudioSource = comp;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Starting the race background music if the race start audio stopped playing
        if (raceIsStillStarting)
        {
            if ((raceStartAudioSource is not null) && (raceMusicAudioSource is not null)) 
            {
                if (!raceStartAudioSource.isPlaying)
                {
                    raceIsStillStarting = false;
                    raceMusicAudioSource.Play();
                }
            }
        }
    }
}
