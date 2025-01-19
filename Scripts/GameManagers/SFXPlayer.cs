using UnityEngine;
using UnityEngine.Audio;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer I;

    [Range (0, 100)] public int currentVolume = 85;
    [SerializeField] AudioMixerGroup audioMixerGroup;

    [SerializeField] AudioClip[] sounds;
    public enum Sound { Enter, Exit, Select, Play, Rotate, Meow, Eating, Key, Lock }

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this);
            return;
        }
        I = this;
    }

    private void Start()
    {
        SetVolume(currentVolume);
    }

    public void SetVolume(int value)
    {
        currentVolume = value;
        float newVolume = Utils.ParseVolume(value);
        audioMixerGroup.audioMixer.SetFloat("Volume", newVolume);
    }

    public void PlaySound(AudioClip sound, float pitchRange = 0)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixerGroup;

        float randomPitch = 1;

        if (pitchRange != 0)
        {
            randomPitch = Random.Range(-pitchRange, pitchRange) + 1;
            audioSource.pitch = randomPitch;
        }

        audioSource.PlayOneShot(sound);
        
        Destroy(audioSource, sound.length / randomPitch);
    }

    public void PlaySound(Sound sound, float pitchRange = 0)
    {
        AudioClip selectedSound = sounds[(int) sound];
        PlaySound(selectedSound, pitchRange);
    }
}
