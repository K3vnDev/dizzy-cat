using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayerSingleton : MonoBehaviour
{
    public static MusicPlayerSingleton Ins;
    public AudioSource audioSource;
    public AudioMixer audioMixer;
    [SerializeField] AnimationCurve curve;

    public float volume = 70;

    private void Awake()
    {
        if (Ins == null) Ins = this;
        else Destroy(gameObject);
    }

    public void ToggleVolume(bool value)
    {
        float mainVolume = (curve.Evaluate(volume / 100) * 80) - 80;

        float newVolume = value ? mainVolume - 15f : mainVolume;
        audioMixer.SetFloat("Volume", newVolume);
    }
}
