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
    public AudioClip coinCollectionAudio;
    public AudioClip speedUpCollectionAudio;
    public AudioClip boostZoneAudio;
    public AudioClip floorTrapCollisionAudio;
    public AudioClip knifeSwingAudio;
    public AudioClip woodBreakAudio;
    
    private AudioSource engineAudioSource;
    private AudioSource drivingOnDirtAudioSource; //Tracking
    private AudioSource knifeSwingAudioSource;

    private UnityAction<Vector3> collisionEventListener;
    private UnityAction<GameObject> raceCountdownEventListener;
    private UnityAction<GameObject> raceMusicEventListener;
    private UnityAction<GameObject> coinCollectionEventListener;
    private UnityAction<GameObject> boostZoneEventListener;
    private UnityAction<GameObject> speedUpCollectionEventListener;
    private UnityAction<Vector3> floorTrapCollisionEventListener;
    private UnityAction<GameObject> boxBreakEventListener;

    //private UnityAction<GameObject> knifeSwingEventListener;

    private void Awake()
    {
        collisionEventListener = new UnityAction<Vector3>(collisionEventHandler);
        raceCountdownEventListener = new UnityAction<GameObject>(raceCountdownEventHandler);
        raceMusicEventListener = new UnityAction<GameObject>(raceMusicEventHandler);
        coinCollectionEventListener = new UnityAction<GameObject>(_coinCollectionEventHandler);
        floorTrapCollisionEventListener = new UnityAction<Vector3>(floorTrapCollisionEventHandler);
        speedUpCollectionEventListener = new UnityAction<GameObject>(_speedUpCollectionEventHandler);
        boostZoneEventListener = new UnityAction<GameObject>(_boostZoneEventHandler);
        boxBreakEventListener = new UnityAction<GameObject>(BoxBreakEventHandler);

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

        knifeSwingAudioSource = gameObject.AddComponent<AudioSource>();
        knifeSwingAudioSource.outputAudioMixerGroup = sfxMixer;
        knifeSwingAudioSource.clip = knifeSwingAudio;
    }

    private void OnEnable()
    {
        EventManager.StartListening<CarCollisionEvent, Vector3>(collisionEventListener);
        EventManager.StartListening<RaceCountdownEvent, GameObject>(raceCountdownEventListener);
        EventManager.StartListening<RaceMusicEvent, GameObject>(raceMusicEventListener);
        EventManager.StartListening<CoinCollectionEvent, GameObject>(coinCollectionEventListener);
        EventManager.StartListening<SpeedUpCollectionEvent, GameObject>(speedUpCollectionEventListener);
        EventManager.StartListening<SpeedBoostZoneEvent, GameObject>(boostZoneEventListener);
        EventManager.StartListening<FloorTrapCollisionEvent, Vector3>(floorTrapCollisionEventListener);
        EventManager.StartListening<BoxBreakEvent, GameObject>(boxBreakEventListener);
    }
    
    private void OnDisable()
    {
        EventManager.StopListening<CarCollisionEvent, Vector3>(collisionEventListener);
        EventManager.StopListening<RaceCountdownEvent, GameObject>(raceCountdownEventListener);
        EventManager.StopListening<RaceMusicEvent, GameObject>(raceMusicEventListener);
        EventManager.StopListening<CoinCollectionEvent, GameObject>(coinCollectionEventListener);
        EventManager.StopListening<SpeedUpCollectionEvent, GameObject>(speedUpCollectionEventListener);
        EventManager.StopListening<SpeedBoostZoneEvent, GameObject>(boostZoneEventListener);
        EventManager.StopListening<FloorTrapCollisionEvent, Vector3>(floorTrapCollisionEventListener);
        EventManager.StopListening<BoxBreakEvent, GameObject>(boxBreakEventListener);
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
    
    private void _coinCollectionEventHandler(GameObject go)
    {
        if (!eventSoundScriptPrefabReference) return;
        
        var snd = Instantiate(eventSoundScriptPrefabReference, go.transform.position, Quaternion.identity, null);
        
        snd.audioSource.clip = this.coinCollectionAudio;
        snd.audioSource.minDistance = 5f;
        snd.audioSource.maxDistance = 100f;
        snd.audioSource.outputAudioMixerGroup = sfxMixer;
        snd.audioSource.Play();
    }
    
    private void _speedUpCollectionEventHandler(GameObject go)
    {
        if (!eventSoundScriptPrefabReference) return;
        
        var snd = Instantiate(eventSoundScriptPrefabReference, go.transform.position, Quaternion.identity, null);
        
        snd.audioSource.clip = this.speedUpCollectionAudio;
        snd.audioSource.minDistance = 5f;
        snd.audioSource.maxDistance = 100f;
        snd.audioSource.outputAudioMixerGroup = sfxMixer;
        snd.audioSource.Play();
    }

    private void _boostZoneEventHandler(GameObject go)
    {
        if (!eventSoundScriptPrefabReference) return;

        var snd = Instantiate(eventSoundScriptPrefabReference, go.transform.position, Quaternion.identity, null);

        snd.audioSource.clip = this.boostZoneAudio;
        snd.audioSource.minDistance = 5f;
        snd.audioSource.maxDistance = 100f;
        snd.audioSource.outputAudioMixerGroup = sfxMixer;
        snd.audioSource.Play();
    }

    void floorTrapCollisionEventHandler(Vector3 worldPos)
    {
        
        if (eventSoundScriptPrefabReference)
        {
            EventSoundScript snd = Instantiate(eventSoundScriptPrefabReference, worldPos, Quaternion.identity, null);

            snd.audioSource.clip = this.floorTrapCollisionAudio;
            snd.audioSource.minDistance = 5f;
            snd.audioSource.maxDistance = 100f;
            snd.audioSource.outputAudioMixerGroup = sfxMixer;
            snd.audioSource.Play();
        }
    }

    public void playStopKnifeSwingAudtio(bool enable)
    {
        if (knifeSwingAudioSource is not null)
        {
            if (enable)
            {
                InvokeRepeating("playKnifeSwingAudio", 0f, 1.0f);
            }
            else if (!enable)
            {
                CancelInvoke("playKnifeSwingAudio");
            }
        }
    }

    private void playKnifeSwingAudio()
    {
        knifeSwingAudioSource.Play();
    }

    public void BoxBreakEventHandler(GameObject box)
    {
        if (eventSoundScriptPrefabReference)
        {
            EventSoundScript snd = Instantiate(eventSoundScriptPrefabReference, box.transform.position, Quaternion.identity, null);

            snd.audioSource.clip = woodBreakAudio;
            snd.audioSource.minDistance = 5f;
            snd.audioSource.maxDistance = 100f;
            snd.audioSource.outputAudioMixerGroup = sfxMixer;
            snd.audioSource.Play();
        }
    }
}
