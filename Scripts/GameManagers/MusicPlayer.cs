using DG.Tweening;
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
    }

    private void Start()
    {
        SetVolume(currentVolume);
    }

    public void Restart()
    {
        audioSource.Play();
    }

    public void LowerVolume(bool value)
    {
        float newVolume = value ? LOW_VOLUME : 0.5f;
        audioSource.volume = newVolume;
    }

    public void SetVolume(float value)
    {
        currentVolume = value;
        float newVolume = Utils.ParseVolume(value, curve);
        audioMixer.SetFloat("Volume", newVolume);
    }
}
