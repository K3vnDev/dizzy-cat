using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayerSingleton : MonoBehaviour
{
    public static MusicPlayerSingleton Ins;
    public AudioSource audioSource;
    public AudioMixer audioMixer;
    public float audioMixerVol = 70;
    public AnimationCurve curve;

    private void Awake()
    {
        if (Ins == null) Ins = this;
        else Destroy(gameObject);
    }

    public void ToggleVolume(bool value)
    {
        float mainVolume = (curve.Evaluate(audioMixerVol / 100) * 80) - 80;

        float volume = value ? mainVolume - 15f : mainVolume;
        audioMixer.SetFloat("Volume", volume);
    }
}
