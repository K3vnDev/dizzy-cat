using UnityEngine;
using UnityEngine.Audio;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer I;

    [SerializeField] AudioMixerGroup audioMixerGroup;
    public float currentVolume = 85;

    [SerializeField] AudioClip[] buttonSounds = new AudioClip[4];
    public enum ButtonSound { Enter, Exit, Select, Play }
    [SerializeField] AnimationCurve curve;

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

    public void SetVolume(float value)
    {
        currentVolume = value;
        float newVolume = Utils.ParseVolume(value, curve);
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

    public void PlaySound(ButtonSound sound, float pitchRange = 0)
    {
        AudioClip selectedSound = buttonSounds[(int) sound];
        PlaySound(selectedSound, pitchRange);
    }
}
