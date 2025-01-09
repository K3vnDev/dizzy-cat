using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] BackgroundController backgroundController;
    [SerializeField] GameObject okayButton, mainMenu;
    [SerializeField] AudioMixer musicMixer, sfxMixer;
    [SerializeField] AudioClip sliderSound, fs_sound1, fs_sound2;
    [SerializeField] TextMeshProUGUI music_voltext, sfx_voltext;
    [SerializeField] AnimationCurve curve;
    [SerializeField] Slider music_slider, sfx_slider;
    [SerializeField] Toggle fullscreen_toggle;
    [SerializeField] float sliderSoundCoolDown;
    private float nextSliderSound = 0;

    private void Start()
    {
        ChangeMusicVolume(MusicPlayerSingleton.Instance.audioMixerVol);
        music_slider.value = MusicPlayerSingleton.Instance.audioMixerVol;

        ChangeSFXVolume(SFXPlayerSingleton.Instance.audioMixerVol);
        sfx_slider.value = SFXPlayerSingleton.Instance.audioMixerVol;

        fullscreen_toggle.isOn = GameManager.Ins.fsActive;
    }

    public void OkayButton()
    {
        backgroundController.GrayBackgroundOut();
        okayButton.transform.localScale = Vector2.one;
        SFXPlayerSingleton.Instance.PlaySound(
            SFXPlayerSingleton.Instance.GetButtonSound("exit"), .1f);

        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ChangeMusicVolume(float vol)
    {
        float mainVolume = (curve.Evaluate(vol/100) * 80) - 80;
        musicMixer.SetFloat("Volume", mainVolume);
        music_voltext.text = vol.ToString();

        MusicPlayerSingleton.Instance.audioMixerVol = vol;
        PlaySliderSound();
    }

    public void ChangeSFXVolume(float vol)
    {
        float mainVolume = (curve.Evaluate(vol / 100) * 80) - 80;
        sfxMixer.SetFloat("Volume", mainVolume);
        sfx_voltext.text = vol.ToString();

        SFXPlayerSingleton.Instance.audioMixerVol = vol;
        PlaySliderSound();
    }

    private void PlaySliderSound()
    {
        if (nextSliderSound < Time.time)
        {
            SFXPlayerSingleton.Instance.PlaySound(sliderSound, .1f);
            nextSliderSound = Time.time + sliderSoundCoolDown;
        }
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        SFXPlayerSingleton.Instance.PlaySound(
            fullscreen ? fs_sound1 : fs_sound2, .1f);

        GameManager.Ins.fsActive = fullscreen;
    }
}
