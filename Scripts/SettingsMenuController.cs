using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] BackgroundController backgroundController;
    [SerializeField] AudioClip sliderSound, fs_sound1, fs_sound2;
    [SerializeField] TextMeshProUGUI music_voltext, sfx_voltext;
    [SerializeField] AnimationCurve curve;
    [SerializeField] Slider music_slider, sfx_slider;
    [SerializeField] Toggle fullscreen_toggle;
    [SerializeField] float sliderSoundCoolDown;
    private float nextSliderSound = 0;

    private void Start()
    {
        ChangeMusicVolume(MusicPlayer.Ins.currentVolume);

        ChangeSFXVolume(SFXPlayer.Ins.currentVolume);

        fullscreen_toggle.isOn = GameManager.Ins.fsActive;
    }

    public void OkayButton()
    {
        backgroundController.SetIsOnGrayBackground(false);
        SFXPlayer.Ins.PlayButtonSound(SFXPlayer.ButtonSound.Exit, .1f);
        SwapMenuManager.Ins.ToMain();
    }

    public void ChangeMusicVolume(float vol)
    {
        MusicPlayer.Ins.SetVolume(vol);
        music_voltext.text = vol.ToString();

        MusicPlayer.Ins.currentVolume = vol;
        PlaySliderSound();

        music_slider.value = vol;
    }

    public void ChangeSFXVolume(float vol)
    {
        SFXPlayer.Ins.SetVolume(vol);
        sfx_voltext.text = vol.ToString();

        PlaySliderSound();

        sfx_slider.value = vol;
    }

    private void PlaySliderSound()
    {
        if (nextSliderSound < Time.time)
        {
            SFXPlayer.Ins.PlaySound(sliderSound, .1f);
            nextSliderSound = Time.time + sliderSoundCoolDown;
        }
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        SFXPlayer.Ins.PlaySound(
            fullscreen ? fs_sound1 : fs_sound2, .1f);

        GameManager.Ins.fsActive = fullscreen;
    }
}
