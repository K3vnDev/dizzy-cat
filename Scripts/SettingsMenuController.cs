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
        ChangeMusicVolume(MusicPlayerSingleton.Ins.volume);
        music_slider.value = MusicPlayerSingleton.Ins.volume;

        ChangeSFXVolume(SFXPlayer.Ins.audioMixerVol);
        sfx_slider.value = SFXPlayer.Ins.audioMixerVol;

        fullscreen_toggle.isOn = GameManager.Ins.fsActive;
    }

    public void OkayButton()
    {
        backgroundController.SetIsOnGrayBackground(false);
        okayButton.transform.localScale = Vector2.one;

        SFXPlayer.Ins.PlayButtonSound(SFXPlayer.ButtonSound.Exit, .1f);

        SwapMenuManager.Ins.ToMain();
    }

    public void ChangeMusicVolume(float vol)
    {
        float mainVolume = (curve.Evaluate(vol/100) * 80) - 80;
        musicMixer.SetFloat("Volume", mainVolume);
        music_voltext.text = vol.ToString();

        MusicPlayerSingleton.Ins.volume = vol;
        PlaySliderSound();
    }

    public void ChangeSFXVolume(float vol)
    {
        float mainVolume = (curve.Evaluate(vol / 100) * 80) - 80;
        sfxMixer.SetFloat("Volume", mainVolume);
        sfx_voltext.text = vol.ToString();

        SFXPlayer.Ins.audioMixerVol = vol;
        PlaySliderSound();
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
