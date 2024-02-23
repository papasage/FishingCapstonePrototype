using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] public AudioClip rodReel;
    [SerializeField] public AudioClip rodCast;
    [SerializeField] public AudioClip rodLineBreak;
    [SerializeField] public AudioClip rodBobberSplash;
    [SerializeField] public AudioClip rodEquip;

    [SerializeField] public AudioClip fishHooked;
    [SerializeField] public AudioClip fishOut;
    [SerializeField] public AudioClip fishFanfare;

    [SerializeField] public AudioClip waterRunningAmbient;
    [SerializeField] public AudioClip waterUnderwaterOneShot1;
    [SerializeField] public AudioClip waterUnderwaterOneShot2;
    [SerializeField] public AudioClip waterUnderwaterOneShot3;

    // Dictionary to store active sounds by AudioClip
    private Dictionary<AudioClip, GameObject> activeSounds = new Dictionary<AudioClip, GameObject>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

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
        audioSource.PlayOneShot(sound);

        // Store the sound in the dictionary
        activeSounds[sound] = soundGameObject;

        Destroy(soundGameObject, sound.length);
    }

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
    public void FishFanfare()
    {
        PlaySound(fishFanfare);
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
        PlaySound(waterUnderwaterOneShot3);
    }
}
