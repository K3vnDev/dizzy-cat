using UnityEngine;
using UnityEngine.Audio;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Ins;

    [SerializeField] AudioMixerGroup audioMixer;
    public float audioMixerVol = 85;

    [SerializeField] AudioClip[] buttonSounds = new AudioClip[4];
    public enum ButtonSound { Enter, Exit, Select, Play }

    private void Awake()
    {
        if (Ins == null) Ins = this;
        else Destroy(gameObject);
    }

    public void PlaySound(AudioClip sound, float pitchRange)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixer;

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
