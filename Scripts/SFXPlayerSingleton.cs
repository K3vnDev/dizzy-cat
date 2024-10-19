using UnityEngine;

public class SFXPlayerSingleton : MonoBehaviour
{
    public static SFXPlayerSingleton Instance;
    private AudioSource audioSource;
    public float audioMixerVol = 85;

    [SerializeField] private AudioClip btnEnter, btnExit, btnSelect, btnPlay;

    private void Awake()
    {
        if (SFXPlayerSingleton.Instance == null)
        {
            SFXPlayerSingleton.Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void PlaySound(AudioClip sound, float pitchRange)
    {
        audioSource.pitch = 
            Random.Range(-pitchRange, pitchRange) + 1;

        audioSource.PlayOneShot(sound);
    }

    public AudioClip GetButtonSound(string sound) 
    {
        AudioClip audioClip = null;
        switch (sound)
        {
            case "enter":
                audioClip = btnEnter;
                break;
            case "exit":
                audioClip = btnExit;
                break;
            case "select":
                audioClip = btnSelect;
                break;
            case "play":
                audioClip = btnPlay;
                break;
        }
        return audioClip;
    }
}
