using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(MusicSliderReferences))]
public class MusicSlider : Slider
{
    private MusicSliderReferences sliderReferences;
    private AudioMixer audioMixer;
    private AudioMixerGroup musicGroup;

    protected override void Awake()
    {
        base.Awake();
        sliderReferences = GetComponent<MusicSliderReferences>();
        musicGroup = sliderReferences.musicGroup;
        audioMixer = musicGroup.audioMixer;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        float defaultVolume = 0.5f;
        float musicVolume = Prefs.HasKey(Prefs.KEY_TYPES.MUSIC) ? Mathf.Pow(10, Prefs.GetKey<float>(Prefs.KEY_TYPES.MUSIC) / 20f) : defaultVolume;
        value = musicVolume;
    }

    public void SetMusicVolume()
    {
        float volume = value;
        float newVolume = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat(musicGroup.name, newVolume);

        Prefs.SetKey(Prefs.KEY_TYPES.MUSIC, newVolume);
    }
}
