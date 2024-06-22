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
    public AudioClip raceCountdownAudio;
    public AudioClip raceMusicAudio;
    public AudioClip drivingOnDirtAudio;
    public AudioClip engineAudio;
    
    private AudioSource engineAudioSource;
    private AudioSource drivingOnDirtAudioSource; //Tracking

    private UnityAction<Vector3> collisionEventListener;
    private UnityAction<GameObject> raceCountdownEventListener;
    private UnityAction<GameObject> raceMusicEventListener;

    private void Awake()
    {
        collisionEventListener = new UnityAction<Vector3>(collisionEventHandler);
        raceCountdownEventListener = new UnityAction<GameObject>(raceCountdownEventHandler);
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

    private void OnEnable()
    {
        EventManager.StartListening<CarCollisionEvent, Vector3>(collisionEventListener);
        EventManager.StartListening<RaceCountdownEvent, GameObject>(raceCountdownEventListener);
        EventManager.StartListening<RaceMusicEvent, GameObject>(raceMusicEventListener);
    }

    private void OnDisable()
    {
        EventManager.StopListening<CarCollisionEvent, Vector3>(collisionEventListener);
        EventManager.StopListening<RaceCountdownEvent, GameObject>(raceCountdownEventListener);
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

    void raceCountdownEventHandler(GameObject go)
    {
        if (eventSoundScriptPrefabReference)
        {
            EventSoundScript snd = Instantiate(eventSoundScriptPrefabReference, go.transform);

            snd.audioSource.clip = this.raceCountdownAudio;
            snd.audioSource.minDistance = 5f;
            snd.audioSource.maxDistance = 100f;
            snd.audioSource.outputAudioMixerGroup = sfxMixer;
            snd.audioSource.Play();
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
