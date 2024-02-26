using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    //these are variable that help us check when a new song/ambient is played, check to see if it is already playing. If not, delete the previous.
    private AudioSource lastPlayedMusic;
    private AudioSource lastPlayedAmbient;

    private bool wasUnderwater = false;

    [Header("Mixers")]
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] public AudioMixerGroup mixerSFX;
    [SerializeField] public AudioMixerGroup mixerMusic;
    [SerializeField] public AudioMixerGroup mixerAmbience;

    [Header("Exposed Parameter Strings")]
    public string lowPassMusicParameter = "LowPassMusic";
    public string lowPassSFXParameter = "LowPassSFX";

    [Header("Music")]
    [SerializeField] public AudioClip peacefulMusic;
    [SerializeField] public AudioClip actionMusic;
    [SerializeField] public AudioClip failureMusic;

    [Header("RodFX Clips")]
    [SerializeField] public AudioClip rodReel;
    [SerializeField] public AudioClip rodCast;
    [SerializeField] public AudioClip rodLineBreak;
    [SerializeField] public AudioClip rodBobberSplash;
    [SerializeField] public AudioClip rodEquip;

    [Header("FishFX Clips")]
    [SerializeField] public AudioClip fishHooked;
    [SerializeField] public AudioClip fishOut;
    [SerializeField] public AudioClip fishFanfare;

    [Header("Ambient")]
    [SerializeField] public AudioClip waterRunningAmbient;
    [SerializeField] public AudioClip waterUnderAmbient;

    [Header("WaterFX Clips")]
    [SerializeField] public AudioClip waterUnderwaterOneShot1;
    [SerializeField] public AudioClip waterUnderwaterOneShot2;
    


    // Dictionary to store active sounds by AudioClip
    private Dictionary<AudioClip, GameObject> activeSounds = new Dictionary<AudioClip, GameObject>();
    private Dictionary<AudioClip, GameObject> activeMusic = new Dictionary<AudioClip, GameObject>();
    private Dictionary<AudioClip, GameObject> activeAmbience = new Dictionary<AudioClip, GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (CameraManager.Instance != null)
        {
            bool isUnderwater = CameraManager.Instance.isUnderWater;

            if (isUnderwater && !wasUnderwater)
            {
                EnableUnderwaterAudio();
            }
            else if (!isUnderwater && wasUnderwater)
            {

                DisableUnderwaterAudio();
            }

            wasUnderwater = isUnderwater;
        }
    }

    //---------------------------------------------METHODS--------
    void PlaySound(AudioClip sound)
    {
        // Check if the sound is already playing
        if (activeSounds.ContainsKey(sound) && activeSounds[sound] != null)
        {
            // Sound is already playing, reuse the existing GameObject
            return;
        }

        GameObject soundGameObject = new GameObject(sound.name);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixerSFX;
        audioSource.PlayOneShot(sound);

        // Store the sound in the dictionary
        activeSounds[sound] = soundGameObject;

        Destroy(soundGameObject, sound.length);
    }
    void PlayMusic(AudioClip sound, bool isLooping)
    {
        // Check if the sound is already playing
        if (lastPlayedMusic != null)
        {
            if (lastPlayedMusic.clip == sound)
            {
                // The same sound is already playing, do nothing
                return;
            }
            // Different sound is playing, stop the existing source and destroy the GameObject
            lastPlayedMusic.Stop();
            Destroy(lastPlayedMusic.gameObject);
        }

        GameObject soundGameObject = new GameObject(sound.name);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixerMusic;
        audioSource.loop = isLooping;
        audioSource.clip = sound;
        audioSource.Play();

        // Store the sound and its AudioSource in the dictionary
        activeMusic[sound] = soundGameObject;
        lastPlayedMusic = audioSource;
    }

    void PlayAmbience(AudioClip sound, bool isLooping)
    {
        // Check if the sound is already playing
        if (lastPlayedAmbient != null)
        {
            // Ambient is already playing, check if it's the same sound

            if (lastPlayedAmbient.clip == sound)
            {
                // The same sound is already playing, do nothing
                return;
            }
            // Different sound is playing, stop the existing source and destroy the GameObject

            lastPlayedAmbient.Stop();
            Destroy(lastPlayedAmbient.gameObject);
        }

        GameObject soundGameObject = new GameObject(sound.name);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = mixerAmbience;
        audioSource.loop = isLooping;
        audioSource.clip = sound;
        audioSource.Play();

        // Store the sound in the dictionary
        activeAmbience[sound] = soundGameObject;
        lastPlayedAmbient = audioSource;
    }

    //---------------------------------------------------SFX------
    public void RodEquip()
    {
        PlaySound(rodEquip);
    }
    public void RodReel()
    {
        PlaySound(rodReel);
    }
    public void RodCast()
    {
        PlaySound(rodCast);
    }
    public void RodLineBreak()
    {
        PlaySound(rodLineBreak);
    }
    public void RodBobberSplash()
    {
        PlaySound(rodBobberSplash);
    }
    public void FishHooked()
    {
        PlaySound(fishHooked);
    }
    public void FishOut()
    {
        PlaySound(fishOut);
    }
    public void RunningWaterLoop()
    {
        PlaySound(waterRunningAmbient);
    }
    public void UnderwaterOneShot1()
    {
        PlaySound(waterUnderwaterOneShot1);
    }
    public void UnderwaterOneShot2()
    {
        PlaySound(waterUnderwaterOneShot2);
    }
    public void UnderwaterOneShot3()
    {
        PlaySound(waterUnderAmbient);
    }
    //---------------------------------------------MUSIC----------
    public void MusicPeaceful()
    {
        PlayMusic(peacefulMusic,true);
    }
    public void MusicAction()
    {
        PlayMusic(actionMusic, true);
    }
    public void MusicFailure()
    {
        PlayMusic(failureMusic, true);
    }
    public void MusicFishCaught()
    {
        PlayMusic(fishFanfare, false);
    }
    //---------------------------------------------AMBIENT--------
    public void AmbienceDock()
    {
        PlayAmbience(waterRunningAmbient, true);
    }
    public void AmbienceUnderWater()
    {
        PlayAmbience(waterUnderAmbient, true);
    }
    //---------------------------------------------FILTERS--------
    public void EnableLowPassMusic()
    {
        audioMixer.SetFloat(lowPassMusicParameter, 700f); // Adjust the value as needed
    }
    
    public void DisableLowPassMusic()
    {
        audioMixer.SetFloat(lowPassMusicParameter, 22000f); // Adjust the value as needed
    }
    public void EnableLowPassSFX()
    {
        audioMixer.SetFloat(lowPassSFXParameter, 2300f); // Adjust the value as needed
    }
    
    public void DisableLowPassSFX()
    {
        audioMixer.SetFloat(lowPassSFXParameter, 22000f); // Adjust the value as needed
    }

    //----------------------------------------------EVENTS--------
    public void EnableUnderwaterAudio()
    {
        FishOut();
        EnableLowPassMusic();
        EnableLowPassSFX();
        AmbienceUnderWater();

        
    }
    public void DisableUnderwaterAudio()
    {
        FishOut();
        DisableLowPassMusic();
        DisableLowPassSFX();
        AmbienceDock();
    }
}
