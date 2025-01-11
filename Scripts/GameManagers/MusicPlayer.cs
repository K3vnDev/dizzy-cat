using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Ins;
    [HideInInspector] public AudioSource audioSource;

    public AudioMixer audioMixer;
    [SerializeField] AnimationCurve curve;

    public float currentVolume = 70;
    readonly float LOW_VOLUME = 0.1f;

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(this);
            return;
        }
        Ins = this;

        audioSource = GetComponent<AudioSource>();
        SetVolume(currentVolume);
    }

    public void LowerVolume(bool value)
    {
        float newVolume = value ? LOW_VOLUME : 0.5f;
        audioSource.volume = newVolume;
    }

    /// <summary>
    /// Sets the music audio mixer volume from a 0 to 100 float
    /// </summary>
    public void SetVolume(float value)
    {
        currentVolume = value;

        float newMixerVolume = (curve.Evaluate(currentVolume / 100) * 80) - 80;
        audioMixer.SetFloat("Volume", newMixerVolume);
    }
}
