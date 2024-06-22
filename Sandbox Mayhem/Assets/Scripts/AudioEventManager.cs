using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioEventManager : MonoBehaviour
{
    //public AudioMixer mixer;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    public EventSoundScript eventSoundScriptPrefabReference;
    public AudioClip collisionAudio;
    public AudioClip raceStartAudio;
    public AudioClip raceMusicAudio;
    public AudioClip drivingOnDirtAudio;
    public AudioClip engineAudio;

    private bool countdownInProgress = false;
    private AudioSource raceStartAudioSource; // To track if the race start sound finished playing
    private AudioSource engineAudioSource;
    private AudioSource drivingOnDirtAudioSource; //Tracking

    private UnityAction<Vector3> collisionEventListener;
    private UnityAction<GameObject> raceStartEventListener;
    private UnityAction<GameObject> raceMusicEventListener;

    private void Awake()
    {
        collisionEventListener = new UnityAction<Vector3>(collisionEventHandler);
        raceStartEventListener = new UnityAction<GameObject>(raceStartEventHandler);
        raceMusicEventListener = new UnityAction<GameObject>(raceMusicEventHandler);

        //float masterVolume = PlayerPrefs.GetFloat("MasterVol");
        //mixer.SetFloat("MasterVol", masterVolume);

        //float musicVolume = PlayerPrefs.GetFloat("MusicVol");
        //mixer.SetFloat("MusicVol", musicVolume);

        //float sfxVolume = PlayerPrefs.GetFloat("SFXLabel");
        //mixer.SetFloat("SFXVol", sfxVolume);
    }

    private void Start()
    {
        // Playing the race start countdown (as one time event)
        EventManager.TriggerEvent<RaceStartEvent, GameObject>(gameObject);

        // Preparing references for continuous audio or that requires stopping early
        drivingOnDirtAudioSource = gameObject.AddComponent<AudioSource>();
        drivingOnDirtAudioSource.clip = drivingOnDirtAudio;
        drivingOnDirtAudioSource.outputAudioMixerGroup = sfxMixer;

        engineAudioSource = gameObject.AddComponent<AudioSource>();
        engineAudioSource.outputAudioMixerGroup = sfxMixer;
        engineAudioSource.clip = engineAudio;
        engineAudioSource.loop = true;
        engineAudioSource.Play();
    }

    private void Update()
    {
        // Starting the race music if the countdown started and the raceStart sound clip stopped playing (was destroyed)
        if (countdownInProgress && (raceStartAudioSource == null))
        {
            EventManager.TriggerEvent<RaceMusicEvent, GameObject>(gameObject);
            countdownInProgress = false;
        }
    }
    private void OnEnable()
    {
        EventManager.StartListening<CarCollisionEvent, Vector3>(collisionEventListener);
        EventManager.StartListening<RaceStartEvent, GameObject>(raceStartEventListener);
        EventManager.StartListening<RaceMusicEvent, GameObject>(raceMusicEventListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening<CarCollisionEvent, Vector3>(collisionEventListener);
        EventManager.StopListening<RaceStartEvent, GameObject>(raceStartEventListener);
        EventManager.StopListening<RaceMusicEvent, GameObject>(raceMusicEventListener);
    }

    void collisionEventHandler(Vector3 worldPos)
    {
        //AudioSource.PlayClipAtPoint(this.fenceCollisionAudio, worldPos, 1f);

        if (eventSoundScriptPrefabReference)
        {
            
            EventSoundScript snd = Instantiate(eventSoundScriptPrefabReference, worldPos, Quaternion.identity, null);
            
            snd.audioSource.clip = this.collisionAudio;
            snd.audioSource.minDistance = 5f;
            snd.audioSource.maxDistance = 100f;
            snd.audioSource.outputAudioMixerGroup = sfxMixer;
            snd.audioSource.Play();
        }
    }

    void raceStartEventHandler(GameObject go)
    {
        if (eventSoundScriptPrefabReference)
        {
            EventSoundScript snd = Instantiate(eventSoundScriptPrefabReference, go.transform);

            snd.audioSource.clip = this.raceStartAudio;
            snd.audioSource.minDistance = 5f;
            snd.audioSource.maxDistance = 100f;
            snd.audioSource.outputAudioMixerGroup = sfxMixer;
            snd.audioSource.Play();
            
            countdownInProgress = true;

            // raceStartAudioSource holds the raceStart audio so that we detect if the countdown stopped playing to start playing the background music
            raceStartAudioSource = snd.audioSource;
            raceStartAudioSource.outputAudioMixerGroup = sfxMixer;
        }
    }

    void raceMusicEventHandler(GameObject go)
    {
        if (eventSoundScriptPrefabReference)
        {
            EventSoundScript snd = Instantiate(eventSoundScriptPrefabReference, go.transform);

            snd.audioSource.clip = this.raceMusicAudio;
            snd.audioSource.minDistance = 5f;
            snd.audioSource.maxDistance = 100f;
            snd.audioSource.loop = true;
            snd.audioSource.outputAudioMixerGroup = musicMixer;
            //snd.audioSource.volume = 0.4f;
            snd.audioSource.Play();
        }
    }

    public void playStopDrivingOnDirtAudio(bool enable)
    {

        if (drivingOnDirtAudioSource is not null)
        {
            if (enable && !drivingOnDirtAudioSource.isPlaying)
            {
                drivingOnDirtAudioSource.Play();
            }
            else if (!enable && drivingOnDirtAudioSource.isPlaying)
            {
                drivingOnDirtAudioSource.Stop();
            }
        }
    }

    public void setEnginePitchAudio(float pitch)
    {
        // input "pitch" between 0 and 1.

        const float ENGINE_PITCH_MIN = 0.5f;
        const float ENGINE_PITCH_MAX = 3.0f;
        if (engineAudioSource is not null)
        {
            engineAudioSource.pitch = ENGINE_PITCH_MIN + (ENGINE_PITCH_MAX - ENGINE_PITCH_MIN)*pitch;
        }
    }
}
