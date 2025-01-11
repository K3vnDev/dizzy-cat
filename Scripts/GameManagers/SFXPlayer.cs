using UnityEngine;
using UnityEngine.Audio;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Ins;

    [SerializeField] AudioMixerGroup audioMixerGroup;
    public float currentVolume = 85;

    [SerializeField] AudioClip[] buttonSounds = new AudioClip[4];
    public enum ButtonSound { Enter, Exit, Select, Play }
    [SerializeField] AnimationCurve curve;

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(this);
            return;
        }
        Ins = this;

        SetVolume(currentVolume);
    }

    /// <summary>
    /// Sets the sfx audio mixer volume from a 0 to 100 float
    /// </summary>
    public void SetVolume(float value)
    {
        currentVolume = value;

        float newAudioMixerVolume = (curve.Evaluate(currentVolume / 100) * 80) - 80;
        audioMixerGroup.audioMixer.SetFloat("Volume", newAudioMixerVolume);
    }

    public void PlaySound(AudioClip sound, float pitchRange)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixerGroup;

        float randomPitch = Random.Range(-pitchRange, pitchRange) + 1;
        audioSource.pitch = randomPitch;

        audioSource.PlayOneShot(sound);
        
        Destroy(audioSource, sound.length / randomPitch);
    }

    public void PlayButtonSound(ButtonSound sound, float pitchRange)
    {
        AudioClip selectedSound = buttonSounds[(int) sound];
        PlaySound(selectedSound, pitchRange);
    }
}
